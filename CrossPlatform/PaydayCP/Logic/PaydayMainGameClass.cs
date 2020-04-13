using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Cards;
using PaydayCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace PaydayCP.Logic
{



    [SingletonGame]
    public class PaydayMainGameClass
        : BoardDiceGameClass<PaydayPlayerItem, PaydaySaveInfo, EnumColorChoice, int>, IMiscDataNM, IFinishStart
    {
        public PaydayMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            PaydayVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            PaydayGameContainer container,
            StandardRollProcesses<SimpleDice, PaydayPlayerItem> roller,
            GameBoardProcesses gameBoard,
            IMailProcesses mailProcesses,
            IDealProcesses dealProcesses,
            ILotteryProcesses lotteryProcesses,
            IYardSaleProcesses yardSaleProcesses,
            IBuyProcesses buyProcesses,
            IChoosePlayerProcesses playerProcesses,
            IDealBuyChoiceProcesses choiceProcesses,
            IMoveProcesses moveProcesses
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller)
        {
            _model = model;
            _command = command;
            _gameBoard = gameBoard;
            _mailProcesses = mailProcesses;
            _dealProcesses = dealProcesses;
            _lotteryProcesses = lotteryProcesses;
            _yardSaleProcesses = yardSaleProcesses;
            _buyProcesses = buyProcesses;
            _playerProcesses = playerProcesses;
            _choiceProcesses = choiceProcesses;
            _gameContainer = container;
            _gameContainer.OtherTurnProgressAsync = OtherTurnProgressAsync;
            _gameContainer.SpaceClickedAsync = _gameBoard.AnimateMoveAsync;
            _gameContainer.ResultsOfMoveAsync = moveProcesses.ResultsOfMoveAsync;
        }

        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;
        private readonly CommandContainer _command;
        private readonly GameBoardProcesses _gameBoard;
        private readonly IMailProcesses _mailProcesses;
        private readonly IDealProcesses _dealProcesses;
        private readonly ILotteryProcesses _lotteryProcesses;
        private readonly IYardSaleProcesses _yardSaleProcesses;
        private readonly IBuyProcesses _buyProcesses;
        private readonly IChoosePlayerProcesses _playerProcesses;
        private readonly IDealBuyChoiceProcesses _choiceProcesses;
        private bool _autoResume;

        public int OtherTurn
        {
            get
            {
                return SaveRoot!.PlayOrder.OtherTurn;
            }
            set
            {
                SaveRoot!.PlayOrder.OtherTurn = value;
            }
        }
        async Task IFinishStart.FinishStartAsync()
        {
            if (_autoResume == false && PlayerList.DidChooseColors())
            {
                return;
            }
            if (PlayerList.DidChooseColors() == false)
            {
                PrepStartTurn();
                return;
            }
            await _gameBoard.ReloadSavedStateAsync(); //could be iffy.  if so, has to think about what else can be done (?)
            if (SaveRoot!.GameStatus == EnumStatus.MakeMove)
            {
                _model!.Cup!.CanShowDice = true;
            }
            else
            {
                _model!.Cup!.CanShowDice = false;
            }
            if (SaveRoot.GameStatus != EnumStatus.ChooseLottery)
            {
                if (SaveRoot.CurrentMail != null && SaveRoot.CurrentMail.Deck > 0)
                {
                    _model.MailPile.AddCard(SaveRoot.CurrentMail);
                }
                if (SaveRoot.CurrentDeal != null && SaveRoot.CurrentDeal.Deck > 0)
                {
                    _model.DealPile.AddCard(SaveRoot.CurrentDeal);
                }
            }

            base.PrepStartTurn();
            _gameBoard.NewTurn(); //so you can see that part.
            _gameContainer.MonthLabel(_model);
            _dealProcesses.PopulateDeals();
            _mailProcesses.PopulateMails();
            //if (SaveRoot.GameStatus == EnumStatus.Starts)
            //{
            //    PrepStartTurn(); //i think.
            //}
            //else
            //{
                
            //}
        }
        public override Task FinishGetSavedAsync()
        {
            _autoResume = true;
            CanPrepTurnOnSaved = false;
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            SaveRoot.LoadMod(_model); //forgot this too.
            CardInformation thisCard;
            if (PlayerList.DidChooseColors() == true)
            {
                var tempList = SaveRoot.OutCards.ToRegularDeckDict();
                SaveRoot.OutCards.Clear();
                tempList.ForEach(temps =>
                {
                    thisCard = _gameContainer!.GetCard(temps.Deck);
                    SaveRoot.OutCards.Add(thisCard);
                });
            }
            PlayerList!.ForEach(thisPlayer =>
            {
                var nextList = thisPlayer.Hand.ToRegularDeckDict();
                DeckRegularDict<CardInformation> finList = new DeckRegularDict<CardInformation>();
                nextList.ForEach(temps =>
                {
                    thisCard = _gameContainer!.GetCard(temps.Deck);
                    finList.Add(thisCard);
                });
                thisPlayer.Hand.ReplaceRange(finList); //so we don't lose the data.
            });
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
            if (PlayerList.DidChooseColors() == false)
            {
                await base.ComputerTurnAsync();
                return;
            }

            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.75);
            }
            bool rets;
            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.Starts:
                case EnumStatus.RollCharity:
                case EnumStatus.RollLottery:
                case EnumStatus.RollRadio:
                    await Roller!.RollDiceAsync(); //i think
                    break;
                case EnumStatus.MakeMove:
                    await _gameBoard!.AnimateMoveAsync(SaveRoot.NumberHighlighted);
                    break;
                case EnumStatus.ChooseBuy:
                    await _buyProcesses.BuyerSelectedAsync(PaydayComputerAI.BuyerSelected(this));
                    break;
                case EnumStatus.ChooseDeal:
                    rets = PaydayComputerAI.PurchaseDeal(this);
                    if (rets == true)
                    {
                        _model!.PopUpChosen = "Yes";
                    }
                    else
                    {
                        _model!.PopUpChosen = "No";
                    }

                    await _dealProcesses.ChoseWhetherToPurchaseDealAsync(); //hopefully this simple.

                    //await ResumeAfterPopUpAsync();
                    break;
                case EnumStatus.ChooseLottery:
                    _model!.PopUpChosen = PaydayComputerAI.NumberChosen(_model).ToString();
                    await _lotteryProcesses.ProcessLotteryAsync();

                    //await ResumeAfterPopUpAsync();
                    break;
                case EnumStatus.ChoosePlayer:

                    _model!.PopUpChosen = PaydayComputerAI.PlayerChosen(_gameContainer, _model);
                    await _playerProcesses.ProcessChosenPlayerAsync();
                    //await ResumeAfterPopUpAsync();
                    break;
                case EnumStatus.DealOrBuy:
                    rets = PaydayComputerAI.LandDeal(this);
                    if (rets == true)
                    {
                        _model.PopUpChosen = "Deal";
                    }
                    else
                    {
                        _model.PopUpChosen = "Buy";
                    }

                    await _choiceProcesses.ChoseDealOrBuyAsync();

                    //await ResumeAfterPopUpAsync();
                    break;
                default:
                    throw new BasicBlankException($"Can't figure out what to do about {SaveRoot.GameStatus.ToString()}");
            }

        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            _autoResume = false;
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            SaveRoot.LotteryAmount = 0;
            SaveRoot.DealListLeft.Clear();
            SaveRoot.MailListLeft.Clear();
            SaveRoot.CurrentDeal = null;
            SaveRoot.CurrentMail = null;
            SaveRoot.LoadMod(_model); //we usually need this.
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.

            if (PlayerList.Count() == 2 || PlayerList.Count() == 3)
            {
                SaveRoot.MaxMonths = 4;
            }
            else if (PlayerList.Count() == 4)
            {
                SaveRoot.MaxMonths = 3;
            }
            else
            {
                SaveRoot.MaxMonths = 2;
            }


            await FinishUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {

                case "popupchosen":
                    _model!.PopUpChosen = content; //try this way.

                    if (SaveRoot.GameStatus == EnumStatus.ChoosePlayer)
                    {
                        await _playerProcesses.ProcessChosenPlayerAsync();
                        return;
                    }
                    if (SaveRoot.GameStatus == EnumStatus.ChooseLottery)
                    {
                        await _lotteryProcesses.ProcessLotteryAsync();
                        return;
                    }
                    if (SaveRoot.GameStatus == EnumStatus.DealOrBuy)
                    {
                        await _choiceProcesses.ChoseDealOrBuyAsync();
                        return;
                    }
                    if (SaveRoot.GameStatus == EnumStatus.ChooseDeal)
                    {
                        await _dealProcesses.ChoseWhetherToPurchaseDealAsync();
                        return;
                    }
                    throw new BasicBlankException("Wrong one for popup.  Rethink");
                case "finishyardsale":
                    await _yardSaleProcesses.FinishYardSaleAsync();
                    break;
                case "continuedealprocesses":
                    await _dealProcesses.ContinueDealProcessesAsync();
                    break;
                case "buyerselected":
                    await _buyProcesses.BuyerSelectedAsync(int.Parse(content));
                    break;
                case "reshuffledeallist":
                    CustomBasicList<int> tempList1 = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    DeckRegularDict<DealCard> finList1 = new DeckRegularDict<DealCard>();
                    tempList1.ForEach(x =>
                    {
                        DealCard thisCard = (DealCard)_gameContainer!.GetCard(x);
                        finList1.Add(thisCard);
                    });
                    await _dealProcesses.ReshuffleDealsAsync(finList1);
                    Check!.IsEnabled = true; //wait for another message.
                    break;
                case "reshufflemaillist":
                    CustomBasicList<int> tempList2 = await js.DeserializeObjectAsync<CustomBasicList<int>>(content);
                    DeckRegularDict<MailCard> finList2 = new DeckRegularDict<MailCard>();
                    tempList2.ForEach(x =>
                    {
                        MailCard thisCard = (MailCard)_gameContainer!.GetCard(x);
                        finList2.Add(thisCard);
                    });
                    await _mailProcesses.ReshuffleMailAsync(finList2); //mail does not require waiting for another message.
                    break;


                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                PrepStartTurn(); //if you did not choose colors, no need to prepstart because something else will do it.
                //code to run but only if you actually chose color.
                _gameBoard!.NewTurn();
                SaveRoot!.EndGame = false;
                SaveRoot.EndOfMonth = false;
                SaveRoot.GameStatus = EnumStatus.Starts;
                SaveRoot.RemainingMove = 0;
                OtherTurn = 0;
                _gameContainer.MonthLabel(_model);
                PlayerList!.ForEach(items => items.ChoseNumber = 0);
                SaveRoot.Instructions = "Roll the dice to start your turn";
                _dealProcesses.PopulateDeals();
                _mailProcesses.PopulateMails();
                _model!.Cup!.CanShowDice = false;
            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors() == false)
            {
                await base.ContinueTurnAsync();
                return;
            }

            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.None:
                    throw new BasicBlankException("Can't be none.  Rethink");
                case EnumStatus.ViewMail:
                case EnumStatus.EndingTurn:
                    SaveRoot.Instructions = "Turn is ending";
                    if (Test!.NoAnimations == false)
                    {
                        await Delay!.DelayMilli(250);
                    }
                    await EndTurnAsync();
                    return;
                case EnumStatus.MakeMove:
                    SaveRoot.Instructions = "Click on the highlighted space to make your move";
                    break;
                case EnumStatus.ChooseLottery:
                    _lotteryProcesses.LoadLotteryList();
                    break;
                case EnumStatus.RollCharity:
                    SaveRoot.Instructions = "Walk for charity.  Roll the dice and pay 100 times the amount you roll";
                    break;
                case EnumStatus.DealOrBuy:
                    SaveRoot.Instructions = "Please either choose to move to the nearest buy or the nearest deal space";
                    _model!.AddPopupLists(new CustomBasicList<string>() { "Buy", "Deal" });
                    break;
                case EnumStatus.ChooseDeal:
                    if (SaveRoot.CurrentDeal!.Deck == 0)
                        throw new BasicBlankException("Must have a deal to look at to decide whether to choose a deal or not");
                    _model.AddPopupLists(new CustomBasicList<string>() { "Yes", "No" });
                    break;
                case EnumStatus.RollRadio:
                    _model!.Cup!.CanShowDice = false;
                    break;
                case EnumStatus.ChoosePlayer:
                    _playerProcesses.LoadPlayerList();
                    break;
                default:
                    break;
            }
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList!.GetOtherPlayer();
                _model!.OtherLabel = SingleInfo.NickName;
            }
            else
            {
                SingleInfo = PlayerList!.GetWhoPlayer();
                _model!.OtherLabel = "None";
            }
            await base.ContinueTurnAsync();

        }
        protected override void GetPlayerToContinueTurn()
        {
            if (PlayerList.DidChooseColors() == false)
            {
                base.GetPlayerToContinueTurn();
                return;
            }
        }
        public override async Task MakeMoveAsync(int space)
        {
            await _gameBoard.AnimateMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.
                if (PlayerList.All(items => items.InGame == false) || Test!.ImmediatelyEndGame == true)
                {
                    await GameOverAsync();
                    return;
                }
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
                await StartNewTurnAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }

        public override async Task AfterChoosingColorsAsync()
        {

            if (BasicData!.MultiPlayer == true && BasicData.Client == true)
            {
                _command.ManuelFinish = true;
                Check!.IsEnabled = true;
                return; //has to wait to hear from host again.  when they hear back, will act like its resuming game.
                //hopefully no problem to not send the load message (?)
            }


            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            _mailProcesses.SetUpMail();
            _dealProcesses.SetUpDeal();
            await _gameBoard!.ResetBoardAsync();
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            SaveRoot!.GameStatus = EnumStatus.Starts; //i think needs to be here so the other player can know about it.
            if (BasicData.MultiPlayer == true)
            {
                await Network!.SendRestoreGameAsync(SaveRoot); //hopefully this works.
            }
            await StartNewTurnAsync(); //i think it will happen here now.
        }

        private async Task GameOverAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MoneyHas -= thisPlayer.Loans;
                thisPlayer.Loans = 0; //since the money has been reduced.
            });
            SingleInfo = PlayerList.OrderByDescending(items => items.MoneyHas).Take(1).Single();
            await ShowWinAsync();
        }
        public int CalculateDay()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            int currentDay = SingleInfo.DayNumber;
            int newDay = currentDay + Cup!.TotalDiceValue;
            if (newDay > 31 && currentDay < 31)
            {
                SaveRoot!.EndOfMonth = true;
                SaveRoot.RemainingMove = newDay - 31;
                if (SaveRoot.RemainingMove < 0)
                    SaveRoot.RemainingMove = 0;
                if (SingleInfo.CurrentMonth == SaveRoot.MaxMonths)
                {
                    SaveRoot.EndGame = true;
                    SaveRoot.RemainingMove = 0;
                }
                else
                {
                    SaveRoot.EndGame = false;
                }
                return 31;
            }
            if (newDay == 31)
            {
                if (SingleInfo.CurrentMonth == SaveRoot!.MaxMonths)
                    SaveRoot.EndGame = true;
            }
            if (currentDay == 31 && SaveRoot!.RemainingMove > 0)
                return SaveRoot.RemainingMove;
            if (currentDay == 31)
                return Cup.TotalDiceValue;
            return currentDay + Cup.TotalDiceValue;
        }
        private async Task ResultsOfDiceAsync()
        {
            switch (SaveRoot!.GameStatus)
            {
                case EnumStatus.Starts:
                    _gameBoard!.HighlightDay(CalculateDay());
                    SaveRoot.GameStatus = EnumStatus.MakeMove;
                    await ContinueTurnAsync();
                    break;
                case EnumStatus.RollRadio:
                    await ContinueRadioAsync();
                    break;
                case EnumStatus.RollCharity:
                    await ContinueCharityAsync();
                    break;
                case EnumStatus.RollLottery:
                    await _lotteryProcesses.RollLotteryAsync(); //i think.
                    break;
                default:
                    throw new BasicBlankException($"Don't know what to do with the results for status {SaveRoot.GameStatus.ToString()}");
            }
        }
        private async Task ContinueRadioAsync()
        {
            if (Cup!.TotalDiceValue != 3)
            {
                await OtherTurnProgressAsync();
                return;
            }
            SaveRoot!.Instructions = $"{SingleInfo!.NickName} has won $1,000 for being a radio contest winnner by rolling a 3.";
            SingleInfo.MoneyHas += 1000;
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(1);
            }
            SaveRoot.GameStatus = EnumStatus.EndingTurn;
            await ContinueTurnAsync();
        }

        private async Task ContinueCharityAsync()
        {
            decimal amounts = Cup!.ValueOfOnlyDice * 100;
            SaveRoot!.Instructions = $"{SingleInfo!.NickName} owes {amounts.ToCurrency(0)} for the walk for charity";
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(2);
            }
            _gameContainer.ProcessExpense(_gameBoard, amounts);
            await OtherTurnProgressAsync();
        }

        public override async Task AfterRollingAsync()
        {
            await ResultsOfDiceAsync();
        }

        private async Task OtherTurnProgressAsync()
        {
            bool rets;
            if (SaveRoot!.GameStatus == EnumStatus.RollRadio)
                rets = true;
            else
                rets = false;
            OtherTurn = await PlayerList!.CalculateOtherTurnAsync(rets);
            if (OtherTurn > 0)
                SingleInfo = PlayerList.GetOtherPlayer(); //i think.
            if (SaveRoot.GameStatus == EnumStatus.RollRadio)
            {
                SaveRoot.Instructions = $"{SingleInfo!.NickName} will roll for the radio contest";
                await ContinueTurnAsync();
                return;
            }
            if (OtherTurn == 0)
            {
                if (SaveRoot.GameStatus == EnumStatus.RollCharity)
                {
                    SaveRoot.GameStatus = EnumStatus.EndingTurn;
                    await ContinueTurnAsync();
                    return;
                }
                if (SaveRoot.GameStatus == EnumStatus.ChooseLottery)
                {
                    SingleInfo = PlayerList.GetWhoPlayer();
                    if (_lotteryProcesses.CanStartLotteryProcess() == false)
                    {
                        SaveRoot!.Instructions = "No lottery will be played because fewer than 2 players chose to participate";
                        if (Test!.NoAnimations == false)
                        {
                            await Delay!.DelaySeconds(1);
                        }
                        SaveRoot.GameStatus = EnumStatus.EndingTurn;
                        await ContinueTurnAsync();
                        return;
                    }
                    SaveRoot!.GameStatus = EnumStatus.RollLottery;
                    SingleInfo = PlayerList!.GetWhoPlayer();
                    SaveRoot.Instructions = $"{SingleInfo.NickName} will roll for the lottery";
                    await ContinueTurnAsync();
                    return;
                }
                throw new BasicBlankException($"Don't know what to do with {SaveRoot.GameStatus.ToString()}");
            }
            await ContinueTurnAsync();
        }
    }
}