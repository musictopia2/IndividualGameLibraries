using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.EventModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace LifeBoardGameCP.Logic
{
    [SingletonGame]
    public class LifeBoardGameMainGameClass
        : SimpleBoardGameClass<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo, EnumColorChoice, int>, IMiscDataNM
    {
        public LifeBoardGameMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            LifeBoardGameVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            LifeBoardGameGameContainer container,
            IBoardProcesses boardProcesses,
            ISpinnerProcesses spinnerProcesses,
            ITwinProcesses twinProcesses,
            IHouseProcesses houseProcesses,
            IChooseStockProcesses chooseStockProcesses,
            IReturnStockProcesses returnStockProcesses,
            ICareerProcesses careerProcesses,
            IBasicSalaryProcesses basicSalaryProcesses,
            ITradeSalaryProcesses tradeSalaryProcesses,
            IStolenTileProcesses stolenTileProcesses,
            GameBoardProcesses gameBoard
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container)
        {
            _model = model;
            _boardProcesses = boardProcesses; //hopefully i won't regret this.  can always do delegates if necessary as well.
            _spinnerProcesses = spinnerProcesses;
            _twinProcesses = twinProcesses;
            _houseProcesses = houseProcesses;
            _chooseStockProcesses = chooseStockProcesses;
            _returnStockProcesses = returnStockProcesses;
            _careerProcesses = careerProcesses;
            _basicSalaryProcesses = basicSalaryProcesses;
            _tradeSalaryProcesses = tradeSalaryProcesses;
            _stolenTileProcesses = stolenTileProcesses;
            _gameBoard = gameBoard;
            _gameContainer = container;
        }
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly LifeBoardGameVMData _model;
        private readonly IBoardProcesses _boardProcesses;
        private readonly ISpinnerProcesses _spinnerProcesses;
        private readonly ITwinProcesses _twinProcesses;
        private readonly IHouseProcesses _houseProcesses;
        private readonly IChooseStockProcesses _chooseStockProcesses;
        private readonly IReturnStockProcesses _returnStockProcesses;
        private readonly ICareerProcesses _careerProcesses;
        private readonly IBasicSalaryProcesses _basicSalaryProcesses;
        private readonly ITradeSalaryProcesses _tradeSalaryProcesses;
        private readonly IStolenTileProcesses _stolenTileProcesses;
        private readonly GameBoardProcesses _gameBoard;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            SaveRoot.LoadMod(_model);
            if (PlayerList.DidChooseColors() && SaveRoot.GameStatus != EnumWhatStatus.NeedChooseGender)
            {
                //if choosing gender, rethink.
                _gameBoard.LoadSavedGame();
                PlayerList!.ForEach(thisPlayer =>
                {
                    var thisList = thisPlayer.Hand.ToRegularDeckDict();
                    thisPlayer.Hand.Clear();
                    thisList.ForEach(tempCard =>
                    {
                        if (tempCard.Deck <= 9)
                            thisPlayer.Hand.Add(CardsModule.GetCareerCard(tempCard.Deck));
                        else if (tempCard.Deck <= 18)
                            thisPlayer.Hand.Add(CardsModule.GetHouseCard(tempCard.Deck));
                        else if (tempCard.Deck <= 27)
                            thisPlayer.Hand.Add(CardsModule.GetSalaryCard(tempCard.Deck));
                        else if (tempCard.Deck <= 36)
                            thisPlayer.Hand.Add(CardsModule.GetStockCard(tempCard.Deck));
                        else
                            throw new BasicBlankException("Must be between 1 and 36");
                    });
                });
                if (SaveRoot.ChangePosition > 0)
                {

                    _gameContainer.SpinnerPosition = SaveRoot.ChangePosition;
                    _gameContainer.SpinnerRepaint();
                }
                if (SaveRoot.GameStatus == EnumWhatStatus.NeedToEndTurn)
                {
                    SingleInfo = PlayerList.GetWhoPlayer();
                    if (SingleInfo.Position > 0 && SingleInfo.LastMove == EnumFinal.None)
                    {
                        _model.GameDetails = _boardProcesses.GetSpaceDetails(SingleInfo.Position);
                    }
                }
            }
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelayMilli(500);
            }
            if (PlayerList.DidChooseColors() == false)
            {
                await base.ComputerTurnAsync(); //because this handles the choosing of colors.
                return;
            }
            if (SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
            {
                //choose gender computer choose.
                if (_gameContainer.ComputerChooseGenderAsync == null)
                {
                    //throw new BasicBlankException("No processes handling computer choosing gender");
                    return; //hopefully goes back again (?)
                }
                await _gameContainer.ComputerChooseGenderAsync.Invoke();
                return;
            }
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelayMilli(500);
            }
            switch (SaveRoot.GameStatus)
            {
                case EnumWhatStatus.NeedChooseFirstOption:
                    await _boardProcesses.OpeningOptionAsync(EnumStart.Career); //the computer will always choose career.
                    break;
                case EnumWhatStatus.NeedChooseFirstCareer:
                case EnumWhatStatus.NeedNewCareer:
                    await _careerProcesses.ChoseCareerAsync(_model.GetRandomCard);
                    break;
                case EnumWhatStatus.NeedChooseStock:
                    await _chooseStockProcesses.ChoseStockAsync(_model.GetRandomCard);
                    break;
                case EnumWhatStatus.NeedChooseSalary:
                    await _basicSalaryProcesses.ChoseSalaryAsync(_model.GetRandomCard);
                    break;
                case EnumWhatStatus.NeedToSpin:
                    if (SingleInfo!.CarIsInsured == false)
                    {
                        await _boardProcesses.PurchaseCarInsuranceAsync();
                        return;
                    }
                    if (SingleInfo.FirstStock == 0 && SingleInfo.SecondStock == 0)
                    {
                        await _boardProcesses.PurchaseStockAsync();
                        return;
                    }
                    if (SingleInfo.Salary < 80000 && _gameContainer.CanTradeForBig(true))
                    {
                        await _boardProcesses.Trade4TilesAsync();
                        return;
                    }
                    await _spinnerProcesses.StartSpinningAsync(); //hopefully this simple.
                    break;
                case EnumWhatStatus.NeedReturnStock:
                    if (_model.HandList.HandList.Count != 2)
                    {
                        throw new BasicBlankException("Must have 2 choices to return.  Otherwise; should have returned automatically");
                    }
                    await _returnStockProcesses.StockReturnedAsync(_model.GetRandomCard);
                    break;
                case EnumWhatStatus.NeedToChooseSpace:
                    int firstNumber = _gameBoard.FirstPossiblePosition;
                    int secondNumber = _gameBoard.SecondPossiblePosition;
                    if (firstNumber == 0)
                    {
                        throw new BasicBlankException("The first possible position cannot be 0. Check this out");
                    }
                    if (secondNumber == 0)
                    {
                        throw new BasicBlankException("The second possible position cannot be 0.  Otherwise, should have made move automatically");
                    }
                    CustomBasicList<int> posList = new CustomBasicList<int> { firstNumber, secondNumber };
                    int numberChosen;
                    if (Test.DoubleCheck)
                    {
                        numberChosen = secondNumber; //the problem only happens with second.
                    }
                    else
                    {
                        numberChosen = posList.GetRandomItem();
                    }
                    await MakeMoveAsync(numberChosen);
                    //await _boardProcesses.ComputerChoseSpaceAsync(posList.GetRandomItem());
                    break;
                case EnumWhatStatus.NeedNight:
                case EnumWhatStatus.NeedToEndTurn:
                    if (BasicData.MultiPlayer)
                    {
                        await Network!.SendEndTurnAsync();
                    }
                    await EndTurnAsync();
                    break;
                case EnumWhatStatus.NeedStealTile:
                    await _stolenTileProcesses.ComputerStealTileAsync();
                    break;
                case EnumWhatStatus.NeedChooseRetirement:
                    await _boardProcesses.RetirementAsync(EnumFinal.MillionaireEstates); //always choose millionaire for computer;
                    break;

                case EnumWhatStatus.NeedChooseHouse:
                case EnumWhatStatus.NeedSellBuyHouse:
                    await _spinnerProcesses.StartSpinningAsync(); //computer will never choose to sell or buy house.
                    break;
                case EnumWhatStatus.NeedTradeSalary:
                    await _tradeSalaryProcesses.ComputerTradeAsync();
                    break;
                default:
                    throw new BasicBlankException("Rethink for computer turn");
            }
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            SaveRoot.LoadMod(_model);
            //hopefully the erasing of colors is already handled.
            await FinishUpAsync(isBeginning);
        }
        //protected override async Task ShowHumanCanPlayAsync()
        //{
        //    await base.ShowHumanCanPlayAsync();
        //    if (BasicData.IsXamarinForms && (SaveRoot.GameStatus == EnumWhatStatus.NeedTradeSalary || SaveRoot.GameStatus == EnumWhatStatus.NeedStealTile))
        //    {
        //        await UIPlatform.ShowMessageAsync("Trying to show human can continue");
        //        if (_gameContainer.SubmitPlayerCommand == null)
        //        {
        //            //UIPlatform.ShowError("Nobody set up the submit player command.  Rethink");
        //            return;
        //        }
        //        _gameContainer.SubmitPlayerCommand.ReportCanExecuteChange(); //try this way.
        //        //await Task.Delay(200); //try delay to fix second bug.
        //        //await UIPlatform.ShowMessageAsync("Choose Player Or End Turn");
        //        //_gameContainer.Command.ManualReport();
        //    }
        //}
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                //put in cases here.
                case "spin":
                    SpinnerPositionData spin = await js.DeserializeObjectAsync<SpinnerPositionData>(content);
                    await _spinnerProcesses.StartSpinningAsync(spin);
                    return;
                case "gender":
                    EnumGender gender = await js.DeserializeObjectAsync<EnumGender>(content);
                    if (_gameContainer.SelectGenderAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling the selecting gender.  Rethink");
                    }
                    await _gameContainer.SelectGenderAsync.Invoke(gender);
                    return;
                case "firstoption":
                    EnumStart firsts = await js.DeserializeObjectAsync<EnumStart>(content);
                    await _boardProcesses.OpeningOptionAsync(firsts);
                    return;
                case "chosecareer":
                    await _careerProcesses.ChoseCareerAsync(int.Parse(content));
                    return;
                case "purchasecarinsurance":
                    await _boardProcesses.PurchaseCarInsuranceAsync();
                    return;
                case "purchasedstock":
                    await _boardProcesses.PurchaseStockAsync();
                    return;
                case "tradedlifeforsalary":
                    await _boardProcesses.Trade4TilesAsync(); //i think
                    return;
                case "tradedsalary":
                    await _tradeSalaryProcesses.TradedSalaryAsync(content);
                    return;
                case "stole":
                    await _stolenTileProcesses.TilesStolenAsync(content);
                    return;
                case "purchasedhouseinsurance":
                    await _boardProcesses.PurchaseHouseInsuranceAsync();
                    return;
                case "attendednightschool":
                    await _boardProcesses.AttendNightSchoolAsync();
                    return;
                case "choseretirement":
                    EnumFinal finals = await js.DeserializeObjectAsync<EnumFinal>(content);
                    await _boardProcesses.RetirementAsync(finals);
                    return;
                case "chosestock":
                    await _chooseStockProcesses.ChoseStockAsync(int.Parse(content));
                    return;
                case "chosesalary":
                    await _basicSalaryProcesses.ChoseSalaryAsync(int.Parse(content));
                    return;
                case "stockreturned":
                    await _returnStockProcesses.StockReturnedAsync(int.Parse(content));
                    return;
                case "chosehouse":
                    await _houseProcesses.ChoseHouseAsync(int.Parse(content));
                    return;
                case "willsellhouse":
                    await _boardProcesses.SellHouseAsync();
                    return;
                case "twins":
                    CustomBasicList<EnumGender> gList = await js.DeserializeObjectAsync<CustomBasicList<EnumGender>>(content);
                    await _twinProcesses.GetTwinsAsync(gList);
                    return;
                case "houselist":
                    CustomBasicList<int> tempList = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    SaveRoot!.HouseList.Clear();
                    tempList.ForEach(thisIndex => SaveRoot.HouseList.Add(CardsModule.GetHouseCard(thisIndex)));
                    await ContinueTurnAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override bool CanMakeMainOptionsVisibleAtBeginning
        {
            get
            {
                if (SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
                {
                    return false;
                }
                return base.CanMakeMainOptionsVisibleAtBeginning;
            }
        }
        public override async Task StartNewTurnAsync()
        {
            if (PlayerList.DidChooseColors() && SaveRoot.GameStatus != EnumWhatStatus.NeedChooseGender)
            {
                PrepStartTurn();
                //has to do both gender and colors for this part.
                _gameContainer.CurrentSelected = 0;
                if (SingleInfo!.OptionChosen == EnumStart.None)
                {
                    SaveRoot.GameStatus = EnumWhatStatus.NeedChooseFirstOption;
                }
                else if (_gameContainer.TeacherChooseSecondCareer)
                {
                    SaveRoot.GameStatus = EnumWhatStatus.NeedNewCareer;
                    _gameContainer.MaxChosen = 1;
                }
                else
                {
                    SaveRoot.GameStatus = EnumWhatStatus.NeedToSpin;
                }
                _gameBoard.NewTurn();
                _gameContainer.SpinnerRepaint(); //if nobody is handling it, then no problem.
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors() && SaveRoot.GameStatus != EnumWhatStatus.NeedChooseGender)
            {
                //can do extra things upon continue turn.  many board games require other things.
                switch (SaveRoot.GameStatus)
                {
                    case EnumWhatStatus.NeedChooseHouse:
                        if (SaveRoot.HouseList.Count == 0)
                        {
                            if (BasicData.MultiPlayer && BasicData.Client)
                            {
                                Check!.IsEnabled = true;
                                return; //because has to wait for host.
                            }
                            var list = _gameContainer.Random.GenerateRandomList(18, 9, 10);
                            SaveRoot.HouseList = list.GetHouseList(PlayerList);
                            if (BasicData.MultiPlayer)
                            {
                                await Network!.SendAllAsync("houselist", SaveRoot.HouseList.GetDeckListFromObjectList());
                            }
                            //not sure if anything else is needed (?)
                        }
                        _gameBoard.NumberRolled = 0;
                        _houseProcesses.LoadHouseList();
                        break;
                    case EnumWhatStatus.NeedToEndTurn:
                        _model!.Instructions = "View results and end turn";
                        break;
                    case EnumWhatStatus.NeedChooseStock:
                        _chooseStockProcesses.LoadStockList();
                        break;
                    case EnumWhatStatus.NeedSellBuyHouse:
                        _model!.Instructions = "Choose to sell house or spin to not sell house";
                        await _houseProcesses!.ShowYourHouseAsync();
                        _gameBoard.NumberRolled = 0;
                        break;
                    case EnumWhatStatus.NeedTradeSalary:
                        _model!.Instructions = $"Choose to trade your salary with someone on list or end turn to keep your current salary{Constants.vbCrLf} Your salary is{SingleInfo!.Salary.ToCurrency(0)}";
                        _tradeSalaryProcesses.LoadOtherPlayerSalaries();
                        break;
                    case EnumWhatStatus.NeedFindSellPrice:
                    case EnumWhatStatus.NeedSellHouse:
                        await _houseProcesses!.ShowYourHouseAsync();
                        _model!.Instructions = "Spin to find out the selling price for the house";
                        break;
                    case EnumWhatStatus.NeedChooseSalary:
                        await _basicSalaryProcesses.LoadSalaryListAsync();
                        break;
                    case EnumWhatStatus.NeedReturnStock:
                        _model!.Instructions = "Choose a stock to return";
                        _returnStockProcesses!.LoadCurrentPlayerStocks();
                        break;
                    case EnumWhatStatus.NeedStealTile:
                        _stolenTileProcesses.LoadOtherPlayerTiles();
                        break;
                    case EnumWhatStatus.NeedChooseRetirement:
                        _gameContainer.CurrentView = EnumViewCategory.EndGame;
                        _model!.Instructions = "Choose either Countryside Acres or Millionaire Estates for retirement";
                        break;
                    case EnumWhatStatus.NeedChooseFirstOption:
                        _model!.Instructions = "Choose college or career";
                        break;
                    case EnumWhatStatus.NeedToSpin:
                        _model!.Instructions = "Spin to take your turn";
                        break;
                    case EnumWhatStatus.NeedToChooseSpace:
                        _model!.Instructions = "Decide Which Space";
                        break;
                    case EnumWhatStatus.NeedChooseFirstCareer:
                    case EnumWhatStatus.NeedNewCareer:
                        _careerProcesses.LoadCareerList();
                        if (_model.HandList!.HandList.Count == 0)
                        {
                            SaveRoot.GameStatus = EnumWhatStatus.NeedChooseSalary;
                            await ContinueTurnAsync();
                            return;
                        }
                        break;
                    case EnumWhatStatus.NeedNight:
                        _model!.Instructions = "Decide whether to goto night school or not";
                        break;
                    default:
                        break;
                }
            }
            await base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            if (_gameContainer.CanSendMessage())
            {
                await Network!.SendMoveAsync(space);
            }
            IMoveProcesses move = MainContainer.Resolve<IMoveProcesses>();
            await move.DoAutomateMoveAsync(space);
            //throw new BasicBlankException("Somebody else is handling the make move.  If I am wrong, rethink");
        }

        private async Task FinishTilesAsync()
        {
            _model.Instructions = "None"; //hopefully this simple.
            if (SaveRoot.TileList.Count != 25)
                throw new BasicBlankException("Must have 25 tiles");
            PlayerList!.ForEach(thisPlayer =>
            {
                for (int x = 1; x <= thisPlayer.TilesCollected; x++)
                {
                    var thisTile = SaveRoot.TileList.First();
                    thisPlayer.MoneyEarned += thisTile.AmountReceived;
                    SaveRoot.TileList.RemoveFirstItem();
                }
                PopulatePlayerProcesses.FillInfo(thisPlayer);
            });
            SingleInfo = PlayerList.OrderByDescending(items => items.MoneyEarned).First();
            await ShowWinAsync();
        }
        private int WonSoFar(out int secondPlayer)
        {
            secondPlayer = 0;
            var tempList = PlayerList.Where(items => items.LastMove == EnumFinal.MillionaireEstates).OrderByDescending(items => items.NetIncome()).Take(3).ToCustomBasicList();
            if (tempList.Count == 0)
                return 0;
            if (tempList.Count == 1)
                return tempList.Single().Id;
            if (tempList.First().NetIncome() > tempList[1].NetIncome())
                return tempList.First().Id;
            if (tempList.Count > 2)
            {
                if (tempList.First().NetIncome() == tempList.Last().NetIncome())
                    return 0; //if 3 way tie, nobody gets.
                tempList.RemoveLastItem();
            }
            if (tempList.First().NetIncome() != tempList.Last().NetIncome())
                throw new BasicBlankException("Does not reconcile");
            secondPlayer = tempList.Last().Id;
            return tempList.First().Id;
        }
        private async Task EndGameProcessAsync()
        {
            int winPlayer = WonSoFar(out int secondPlayer);
            if (winPlayer > 0)
            {
                var thisPlayer = PlayerList![winPlayer];
                if (secondPlayer == 0)
                {
                    thisPlayer.TilesCollected += 4;
                }
                else
                {
                    thisPlayer.TilesCollected += 2;
                    thisPlayer = PlayerList[secondPlayer];
                    thisPlayer.TilesCollected += 2;
                }
            }
            await FinishTilesAsync();
        }
        private void CheckSalaries()
        {
            PlayerList.ForEach(player =>
            {
                if (player.Hand.Count(x => x.CardCategory == EnumCardCategory.Salary) > 1)
                {
                    throw new BasicBlankException($"Player {player.NickName} had more than one salary.  That is not correct.  Rethink");
                }
            });
        }
        public override async Task EndTurnAsync()
        {
            CheckSalaries();
            SaveRoot.WasNight = false;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.

            }
            if (WhoTurn > 0)
            {
                await StartNewTurnAsync();
            }
            else
            {
                await EndGameProcessAsync();
            }
        }
        protected override async Task LoadPossibleOtherScreensAsync()
        {
            await base.LoadPossibleOtherScreensAsync();
            if (SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
            {
                if (_gameContainer.ShowGenderAsync == null)
                {
                    throw new BasicBlankException("Nobody is handling showing gender.  Rethink");
                }
                await _gameContainer.ShowGenderAsync();
            }
        }
        public override async Task AfterChoosingColorsAsync()
        {
            //anything else that is needed after they finished choosing colors.
            _gameBoard.ResetBoard(); //has to be after choosing colors after all.  otherwise, the gender gets set to none then it gets hosed (wrong)
            SaveRoot.GameStatus = EnumWhatStatus.NeedChooseGender;
            //SaveRoot.ImmediatelyStartTurn = true;
            //hopefully no need to sendload because you already have what you need this time.
            //await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            await Aggregator.PublishAsync(new GenderEventModel());
            await EndTurnAsync();
            //maybe i had to do after doing the end turn process.  hopefully that makes things better.
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
            {
                if (_gameContainer.ComputerChooseGenderAsync == null)
                {
                    throw new BasicBlankException("The computer choose gender was not handed even after loading gender.  Rethink");
                }
                await _gameContainer.ComputerChooseGenderAsync.Invoke();
            }
        }
        internal async Task AfterChoosingGenderAsync()
        {
            //this is after you already chose gender.  somebody else is responsible for that portion.
            //think about any other part.
            if (SaveRoot.GameStatus != EnumWhatStatus.NeedChooseFirstOption)
            {
                throw new BasicBlankException("After choosing gender, something should have set needchoosefirstoption.  Rethink");
            }
            WhoTurn = WhoStarts;
            
            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            if (BasicData.MultiPlayer && BasicData.Client)
            {
                Check!.IsEnabled = true;
                return; //because has to wait for message from host to continue.
            }
            SaveRoot.ImmediatelyStartTurn = false;
            var list = _gameContainer.Random.GenerateRandomList(25);
            SaveRoot.TileList.Clear();
            list.ForEach(x => SaveRoot.TileList.Add(CardsModule.GetTileInfo(x)));
            if (LifeBoardGameGameContainer.StartCollegeCareer)
            {
                SingleInfo = PlayerList.GetWhoPlayer();
                SingleInfo.OptionChosen = EnumStart.College;
                SingleInfo.Position = 10;
                //PlayerList.ForEach(x =>
                //{
                //    x.OptionChosen = EnumStart.College;
                //    x.Position = 11;
                    
                //});
                //SaveRoot.GameStatus = EnumWhatStatus.NeedChooseFirstCareer; //i think
            }
            if (BasicData.MultiPlayer)
            {
                await Network!.SendRestoreGameAsync(SaveRoot);
            }
            
            await StartNewTurnAsync(); //hopefully this works (?)
        }
    }
}