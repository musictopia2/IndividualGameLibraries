using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static BasicGameFramework.ChooserClasses.ListViewPicker;
using static LifeBoardGameCP.CardsModule;
namespace LifeBoardGameCP
{
    public class LifeBoardGameViewModel : SimpleBoardGameVM<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem, LifeBoardGameMainGameClass, int>
    {
        private EnumWhatStatus _GameStatus = EnumWhatStatus.None;
        public EnumWhatStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    if (value == EnumWhatStatus.NeedStealTile || value == EnumWhatStatus.NeedTradeSalary)
                        PopUpList!.Visible = true;
                    else
                        PopUpList!.Visible = false;
                }
            }
        }
        private int _CurrentSelected; //this is the space you selected so far when you have a choice between 2 spaces.
        public int CurrentSelected
        {
            get { return _CurrentSelected; }
            set
            {
                if (SetProperty(ref _CurrentSelected, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _GameDetails = "";
        public string GameDetails
        {
            get { return _GameDetails; }
            set
            {
                if (SetProperty(ref _GameDetails, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _PlayerChosen = "";
        public string PlayerChosen
        {
            get { return _PlayerChosen; }
            set
            {
                if (SetProperty(ref _PlayerChosen, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private EnumGender _GenderSelected;
        public EnumGender GenderSelected
        {
            get { return _GenderSelected; }
            set
            {
                if (SetProperty(ref _GenderSelected, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public async Task ShowCardAsync(LifeBaseCard thisCard)
        {
            ChosenPile!.Visible = true;
            HandList!.Visible = false;
            thisCard.IsUnknown = false;
            ChosenPile.AddCard(thisCard);
            if (ThisTest!.NoAnimations == false)
                await MainGame!.Delay!.DelaySeconds(.75);
            ChosenPile.Visible = false;
        }
        internal void ShowYourHouse()
        {
            if (ChosenPile!.Visible == true)
                return;
            ChosenPile.Visible = true;
            var thisCard = MainGame!.SingleInfo!.Hand.Single(items => items.CardCategory == EnumCardCategory.House);
            ChosenPile.AddCard(thisCard);
        }
        private bool ShowAllCareers => MainGame!.SingleInfo!.OptionChosen == EnumStart.College || _thisGlobal!.WasNight == true;
        private RandomGenerator Rs => MainGame!.Rs!;
        internal void LoadCareerList()
        {
            HandList!.Text = "Career List";
            var firstList = Rs.GenerateRandomList(9);
            bool canShowAll = ShowAllCareers;
            DeckRegularDict<CareerInfo> tempList = new DeckRegularDict<CareerInfo>();
            firstList.ForEach(thisItem =>
            {
                tempList.Add(GetCareerCard(thisItem));
            });
            if (canShowAll == false)
                tempList.KeepConditionalItems(items => items.DegreeRequired == false);
            var finList = tempList.GetLoadedCards(MainGame!.PlayerList!);
            HandList.HandList.ReplaceRange(finList);
            _thisGlobal!.MaxChosen = 1;
            if (_thisGlobal.WasNight)
            {
                MainGame.SaveRoot!.Instructions = "Pick 2 cards at random for your new career for night school.";
                _thisGlobal.MaxChosen = 2;
            }
            else if (MainGame.SingleInfo!.OptionChosen == EnumStart.College && _thisGlobal.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
            {
                MainGame.SaveRoot!.Instructions = "Pick 3 cards at random for your first career since you went to college.";
                _thisGlobal.MaxChosen = 3;
            }
            else if (_thisGlobal.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
                MainGame.SaveRoot!.Instructions = "Pick one card to choose your career.";
            else
                MainGame.SaveRoot!.Instructions = "Pick one card for your new career.";
            if (_thisGlobal.MaxChosen == 1)
                HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            else
                HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectAsMany;
            HandList.Visible = true;
        }
        internal void LoadStockList()
        {
            HandList!.Text = "List Of Stocks";
            var firstList = Rs.GenerateRandomList(36, 9, 28);
            DeckRegularDict<StockInfo> tempList = new DeckRegularDict<StockInfo>();
            firstList.ForEach(thisItem =>
            {
                tempList.Add(GetStockCard(thisItem));
            });
            var finList = tempList.GetLoadedCards(MainGame!.PlayerList!);
            HandList.HandList.ReplaceRange(finList);
            HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            HandList.Visible = true;
            MainGame.SaveRoot!.Instructions = "Choose a stock from the list";
        }
        internal void LoadHouseList()
        {
            HandList!.Text = "House List";
            if (MainGame!.SaveRoot!.HouseList.Count != 2)
                throw new BasicBlankException("The house list must have 2 items");
            HandList.HandList.ReplaceRange(MainGame.SaveRoot.HouseList);
            MainGame.SaveRoot.Instructions = "Choose a house to buy or spin to not purchase either house";
            HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            HandList.Visible = true;
        }
        internal void LoadSalaryList()
        {
            HandList!.Text = "List Of Salaries";
            MainGame!.SaveRoot!.Instructions = "Choose a salary";
            var firstList = Rs.GenerateRandomList(27, 9, 19);
            var tempList = firstList.GetSalaryList(MainGame.PlayerList!);
            var careerList = MainGame.SingleInfo!.GetCareerList();
            if (careerList.Count == 0)
                throw new BasicBlankException("Must have a career in order to load the salary list");
            CareerInfo thisCareer;
            if (careerList.Count == 1)
                thisCareer = careerList.Single();
            else if (careerList.First().Career == EnumCareerType.Teacher)
                thisCareer = careerList.Last();
            else
                thisCareer = careerList.First();
            tempList.KeepConditionalItems(items => items.WhatGroup == thisCareer.Scale1 || items.WhatGroup == thisCareer.Scale2);
            HandList.HandList.ReplaceRange(tempList);
            HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            HandList.Visible = true;
        }
        internal void LoadCurrentPlayerStocks()
        {
            if (MainGame!.SingleInfo!.FirstStock == 0 || MainGame.SingleInfo.SecondStock == 0)
                throw new BasicBlankException("Must have both stock items.  Otherwise; should do automatically");
            DeckRegularDict<LifeBaseCard> tempList = new DeckRegularDict<LifeBaseCard>
            {
                MainGame.SingleInfo.GetStockCard(MainGame.SingleInfo.FirstStock),
                MainGame.SingleInfo.GetStockCard(MainGame.SingleInfo.SecondStock)
            };
            HandList!.HandList.ReplaceRange(tempList);
            HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            HandList.Text = "List Of Stocks";
            HandList.Visible = true;
        }
        internal void LoadOtherPlayerSalaries()
        {
            var tempList = MainGame!.PlayerList!.AllPlayersExceptForCurrent();
            var newList = tempList.GetSalaryList();
            AddPlayerChoices(newList);
        }
        internal void LoadOtherPlayerTiles()
        {
            MainGame!.SaveRoot!.Instructions = "Choose a player to steal a life tile from";
            var tempList = MainGame.PlayerList!.AllPlayersExceptForCurrent();
            tempList.RemoveAllOnly(items => items.LastMove == EnumFinal.CountrySideAcres || items.TilesCollected == 0);
            var finList = tempList.Select(items => items.NickName).ToCustomBasicList();
            AddPlayerChoices(finList);
        }
        internal void AddPlayerChoices(CustomBasicList<string> thisList)
        {
            PlayerChosen = "";
            PopUpList!.LoadTextList(thisList); //i think.
            PopUpList.UnselectAll(); //just in case.
            PopUpList.Visible = true;
            if (thisList.Count == 1)
            {
                PlayerChosen = thisList.Single();// obviously should show it selected because no choice.
                PopUpList.SelectSpecificItem(1); //because there is only one.
            }
        }
        private bool CanDoBasics => MainGame!.DidChooseColors;
        public override bool CanEndTurn()
        {
            if (CanDoBasics == false)
                return false;
            if (Instructions == "Choose one career or end turn and keep your current career")
                return true;
            return _thisGlobal!.GameStatus == EnumWhatStatus.NeedToEndTurn || _thisGlobal.GameStatus == EnumWhatStatus.NeedTradeSalary || _thisGlobal.GameStatus == EnumWhatStatus.NeedNight;
        }
        public void RepaintGenders()
        {
            ThisE!.Publish(new NewTurnEventModel()); //i think.  hopefully this simple.
        }
        public void AdditionalCareerInstructions()
        {
            var tempList = HandList!.ListSelectedObjects();
            tempList.MakeAllObjectsKnown();
            HandList.HandList.ReplaceRange(tempList);
            HandList.Text = "Choose One Career";
            if (_thisGlobal!.WasNight)
                MainGame!.SaveRoot!.Instructions = "Choose one career or end turn and keep your current career";
            HandList.AutoSelect = HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly;
            HandList.UnselectAllObjects();
        }
        private bool NeedSubmit => GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedToChooseSpace || GameStatus == EnumWhatStatus.NeedTradeSalary || GameStatus == EnumWhatStatus.NeedChooseFirstCareer || GameStatus == EnumWhatStatus.NeedChooseSalary || GameStatus == EnumWhatStatus.NeedChooseStock || GameStatus == EnumWhatStatus.NeedNewCareer || GameStatus == EnumWhatStatus.NeedReturnStock || GameStatus == EnumWhatStatus.NeedStealTile || GameStatus == EnumWhatStatus.NeedTradeSalary;
        public LifeBoardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public HandViewModel<LifeBaseCard>? HandList;
        public PileViewModel<LifeBaseCard>? ChosenPile;
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand<EnumStart>? OpeningCommand { get; set; }
        public BasicGameCommand<EnumFinal>? RetirementCommand { get; set; }
        public BasicGameCommand? SpinCommand { get; set; }
        public BasicGameCommand? CarInsuranceCommand { get; set; }
        public BasicGameCommand<EnumGender>? GenderCommand { get; set; }
        public BasicGameCommand? AttendNightSchoolCommand { get; set; }
        public BasicGameCommand? HouseInsuranceCommand { get; set; }
        public BasicGameCommand? PurchaseStockCommand { get; set; }
        public BasicGameCommand? SellHouseCommand { get; set; }
        public BasicGameCommand? TradeLifeForSalaryCommand { get; set; }
        public BasicGameCommand? SubmitCommand { get; set; } //looks like this is needed too.
        public GameBoardProcesses? GameBoard1;
        private GlobalVariableClass? _thisGlobal;
        public ListViewPicker? PopUpList;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
            _thisGlobal = MainContainer.Resolve<GlobalVariableClass>();
            PopUpList = new ListViewPicker(this);
            PopUpList.IndexMethod = EnumIndexMethod.OneBased;
            PopUpList.ItemSelectedAsync += PopUpList_ItemSelectedAsync;
            PopUpList.SendEnableProcesses(this, () => CanDoBasics);
            HandList = new HandViewModel<LifeBaseCard>(this);
            HandList.Text = "None";
            HandList.Visible = false; //i think default needs to be not visible.
            HandList.IgnoreMaxRules = true; //try this way to attempt to fix the problem
            ChosenPile = new PileViewModel<LifeBaseCard>(ThisE!, this);
            ChosenPile.Visible = false;
            ChosenPile.CurrentOnly = true;
            GameBoard1 = MainContainer.Resolve<GameBoardProcesses>();
            LoadOtherOptions();
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            ThisE!.RepaintBoard();
        }
        private void LoadOtherOptions()
        {
            CarInsuranceCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.PurchaseCarInsuranceAsync();
            }, items =>
            {
                if (CanDoBasics == false)
                    return false;
                return GameStatus == EnumWhatStatus.NeedToSpin && !MainGame!.SingleInfo!.CarIsInsured;
            }, this, CommandContainer!);
            GenderCommand = new BasicGameCommand<EnumGender>(this, async gender =>
            {
                await MainGame!.SelectGenderAsync(gender);
            }, items =>
            {
                if (CanDoBasics == false)
                    return false;
                return GameStatus == EnumWhatStatus.NeedChooseGender;
            }, this, CommandContainer!);
            AttendNightSchoolCommand = new BasicGameCommand(this, async items => await MainGame!.AttendNightSchoolAsync(), items =>
            {
                if (CanDoBasics == false)
                    return false;
                return GameStatus == EnumWhatStatus.NeedNight;
            }, this, CommandContainer!);
            HouseInsuranceCommand = new BasicGameCommand(this, async items => await MainGame!.PurchaseHouseInsuranceAsync(), items =>
            {
                if (CanDoBasics == false)
                    return false;
                if (GameStatus != EnumWhatStatus.NeedToSpin)
                    return false;
                if (MainGame!.SingleInfo!.HouseIsInsured)
                    return false;
                return MainGame.SingleInfo.HouseName != "";
            }, this, CommandContainer!);
            PurchaseStockCommand = new BasicGameCommand(this, async items => await MainGame!.PurchasedStockAsync(), items =>
            {
                if (CanDoBasics == false)
                    return false;
                if (GameStatus != EnumWhatStatus.NeedToSpin)
                    return false;
                if (MainGame!.SingleInfo!.FirstStock > 0 || MainGame.SingleInfo.SecondStock > 0)
                    return false;
                return true;
            }, this, CommandContainer!);
            SellHouseCommand = new BasicGameCommand(this, async items => await MainGame!.WillSellHouseAsync(), items =>
            {
                if (CanDoBasics == false)
                    return false;
                return GameStatus == EnumWhatStatus.NeedSellBuyHouse;
            }, this, CommandContainer!);
            TradeLifeForSalaryCommand = new BasicGameCommand(this, async items => await MainGame!.TradedLifeForSalaryAsync(), items =>
            {
                if (CanDoBasics == false)
                    return false;
                if (GameStatus != EnumWhatStatus.NeedToSpin)
                    return false;
                return MainGame!.CanTradeForBig(true);
            }, this, CommandContainer!);
            SpinCommand = new BasicGameCommand(this, async items =>
            {
                CheckBoardErrors();
                await MainGame!.StartSpinningAsync();
            }, items =>
            {
                if (CanDoBasics == false)
                    return false;
                if (GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedToSpin || GameStatus == EnumWhatStatus.NeedFindSellPrice || GameStatus == EnumWhatStatus.NeedSellHouse || GameStatus == EnumWhatStatus.NeedSellBuyHouse)
                    return true;
                return false;
            }, this, CommandContainer!);
            SpaceCommand = new BasicGameCommand<int>(this, space =>
            {
                _thisGlobal!.CurrentSelected = space;
                GameDetails = MainGame!.SpaceDescription(space);
                ThisE!.RepaintBoard();
            }, space =>
            {
                return CanDoBasics;
            }, this, CommandContainer!);
            OpeningCommand = new BasicGameCommand<EnumStart>(this, async thisStart =>
            {
                if (thisStart == EnumStart.None)
                    throw new BasicBlankException("Must be college or career");
                await MainGame!.FirstOptionChosenAsync(thisStart);
            }, thisStart =>
            {
                if (CanDoBasics == false)
                    return false;
                return GameStatus == EnumWhatStatus.NeedChooseFirstOption;
            }, this, CommandContainer!);
            RetirementCommand = new BasicGameCommand<EnumFinal>(this, async thisRetire =>
            {
                await MainGame!.ChoseRetirementAsync(thisRetire);
            }, thisRetire =>
            {
                if (CanDoBasics == false)
                    return false;
                return GameStatus == EnumWhatStatus.NeedChooseRetirement;
            }, this, CommandContainer!);
            SubmitCommand = new BasicGameCommand(this, async items =>
            {
                if (GameStatus == EnumWhatStatus.NeedToChooseSpace)
                {
                    await MainGame!.MakeMoveAsync(_thisGlobal!.CurrentSelected);
                    return;
                }
                if (HandList!.AutoSelect == HandViewModel<LifeBaseCard>.EnumAutoType.SelectAsMany)
                {
                    AdditionalCareerInstructions();
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedNewCareer || GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
                {
                    await MainGame!.ChoseCareerAsync(HandList.ObjectSelected());
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedChooseSalary)
                {
                    await MainGame!.ChoseSalaryAsync(HandList.ObjectSelected());
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedReturnStock)
                {
                    await MainGame!.StockReturnedAsync(HandList.ObjectSelected());
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedChooseStock)
                {
                    await MainGame!.ChoseStockAsync(HandList.ObjectSelected());
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedChooseHouse)
                {
                    await MainGame!.ChoseHouseAsync(HandList.ObjectSelected());
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedStealTile)
                {
                    await MainGame!.TilesStolenAsync();
                    return;
                }
                if (GameStatus == EnumWhatStatus.NeedTradeSalary)
                {
                    await MainGame!.TradedSalaryAsync();
                    return;
                }
                throw new BasicBlankException("Not sure about status for submit.  Rethink");
            }, items =>
            {
                if (CanDoBasics == false)
                    return false;
                if (NeedSubmit == false)
                    return false;
                if (GameStatus == EnumWhatStatus.NeedToChooseSpace)
                    return _thisGlobal!.CurrentSelected > 0;
                if (HandList!.AutoSelect == HandViewModel<LifeBaseCard>.EnumAutoType.SelectOneOnly && HandList.Visible)
                    return HandList.ObjectSelected() > 0;
                if (GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
                {
                    if (MainGame!.SingleInfo!.OptionChosen == EnumStart.Career)
                        throw new BasicBlankException("Should have been one option alone.");
                    return HandList.HowManySelectedObjects == 3;
                }
                if (GameStatus == EnumWhatStatus.NeedNewCareer)
                {
                    if (MainGame!.SaveRoot!.WasNight)
                        return HandList.HowManySelectedObjects == 2;
                }
                return PlayerChosen != "";
            }, this, CommandContainer!);
        }
        public void CheckBoardErrors()
        {
            if (GameStatus == EnumWhatStatus.NeedChooseGender)
                throw new BasicBlankException("Should have been disabled because must choose gender");
            if (GameStatus == EnumWhatStatus.NeedChooseSalary)
                throw new BasicBlankException("Should have been disabled because must choose salary");
            if (GameStatus == EnumWhatStatus.NeedChooseStock)
                throw new BasicBlankException("Should have been disabled because must choose stock");
            if (GameStatus == EnumWhatStatus.NeedNewCareer)
                throw new BasicBlankException("Should have been disabled because must choose new career");
            if (GameStatus == EnumWhatStatus.NeedNight)
                throw new BasicBlankException("Should have been disabled because must choose night school or end turn");
            if (GameStatus == EnumWhatStatus.NeedReturnStock)
                throw new BasicBlankException("Should have been disabled because must return stock");
            if (GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
                throw new BasicBlankException("Should have been disabled because must choose first career");
            if (GameStatus == EnumWhatStatus.NeedStealTile)
                throw new BasicBlankException("Should have been disabled because must steal tile");
            if (GameStatus == EnumWhatStatus.NeedToEndTurn)
                throw new BasicBlankException("Should have been disabled because must end turn");
            if (GameStatus == EnumWhatStatus.NeedTradeSalary)
                throw new BasicBlankException("Should have been disabled because must trade salary");
        }
        private Task PopUpList_ItemSelectedAsync(int SelectedIndex, string SelectedText)
        {
            PlayerChosen = SelectedText;
            return Task.CompletedTask;
        }
    }
}