using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.EventModels;
using LifeBoardGameCP.Logic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LifeBoardGameCP.Data
{
    [SingletonGame] //this one will not be reset.
    [AutoReset]
    public class LifeBoardGameGameContainer : BasicGameContainer<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>
    {
        public LifeBoardGameGameContainer(
            BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            RandomGenerator random,
            LifeBoardGameVMData model
            ) : base(basicData,
                test,
                gameInfo,
                delay,
                aggregator,
                command,
                resolver,
                random)
        {
            _model = model;
            //go ahead and load up the spacelist right away here.
            LoadSpaceProcesses.PopulateSpaces(this);
            command.ExecutingChanged += Command_ExecutingChanged;
            Pos = new PositionPieces(); //hopefully this simple.
        }

        private void Command_ExecutingChanged()
        {
            Aggregator.RepaintBoard(); //hopefully this simple.
        }
        public static bool StartCollegeCareer { get; set; }
        internal GameSpace? CountrySpace { get; set; }
        internal GameSpace? MillionSpace { get; set; }
        internal GameSpace? ExtraSpace { get; set; }
        internal PositionPieces Pos { get; set; }
        public EnumViewCategory CurrentView { get; set; } = EnumViewCategory.StartGame;
        public CustomBasicList<SpaceInfo> SpaceList { get; set; } = new CustomBasicList<SpaceInfo>();

        public int SpinnerPosition { get; set; }
        public int HighSpeedPhase { get; set; }

        public void SpinnerRepaint()
        {
            Aggregator.Publish(new PaintSpinEventModel());
        }

        //public ISpinnerCanvas? SpinnerCanvas;
        private readonly LifeBoardGameVMData _model;

        public int CurrentSelected
        {
            get
            {
                return _model.CurrentSelected;
            }
            set
            {
                _model.CurrentSelected = value;
            }
        }
        public EnumWhatStatus GameStatus
        {
            get
            {
                return SaveRoot!.GameStatus;
            }
            set
            {
                SaveRoot!.GameStatus = value;
            }
        }
        public CustomBasicList<int> SpinList
        {
            get
            {
                return SaveRoot!.SpinList;
            }
            set
            {
                SaveRoot!.SpinList = value;
            }
        }
        public CustomBasicList<TileInfo> TileList
        {
            get
            {
                return SaveRoot!.TileList;
            }
            set
            {
                SaveRoot!.TileList = value;
            }
        }
        public bool WasMarried
        {
            get
            {
                return SaveRoot!.WasMarried;
            }
            set
            {
                SaveRoot!.WasMarried = value;
            }
        }
        public bool WasNight
        {
            get
            {
                return SaveRoot!.WasNight;
            }
            set
            {
                SaveRoot!.WasNight = value;
            }
        }
        public int MaxChosen
        {
            get
            {
                return SaveRoot!.MaxChosen;
            }
            set
            {
                SaveRoot!.MaxChosen = value;
            }
        }
        public bool EndAfterSalary
        {
            get
            {
                return SaveRoot!.EndAfterSalary;
            }
            set
            {
                SaveRoot!.EndAfterSalary = value;
            }
        }
        public bool EndAfterStock
        {
            get
            {
                return SaveRoot!.EndAfterStock;
            }
            set
            {
                SaveRoot!.EndAfterStock = value;
            }
        }
        public int GetNumberSpun(int position)
        {
            if (position >= 3 && position <= 33)
                return 1;
            if (position >= 40 && position <= 69)
                return 2;
            if (position >= 76 && position <= 105)
                return 3;
            if (position >= 112 && position <= 141)
                return 4;
            if (position >= 148 && position <= 177)
                return 5;
            if (position >= 184 && position <= 213)
                return 6;
            if (position >= 220 && position <= 249)
                return 7;
            if (position >= 256 && position <= 285)
                return 8;
            if (position >= 292 && position <= 321)
                return 9;
            if (position >= 328 && position <= 357)
                return 10;
            return 0;
        }
        //that way the main game class can invoke it.  they are not responsible for the gender processes.
        public Func<EnumGender, Task>? SelectGenderAsync { get; set; }
        public Func<Task>? ComputerChooseGenderAsync { get; set; }
        public Func<Task>? ShowGenderAsync { get; set; }
        //since gender is a special case, decided to do this way for gender (delegates).
        //but not so for the others.


        //there are many things i can dump here which other specialized classes can use.
        internal bool AllHasCareer
        {
            get
            {
                foreach (var thisPlayer in PlayerList!)
                {
                    var tempList = thisPlayer.GetCareerList();
                    if (tempList.Count == 0)
                        return false;
                }
                return true;
            }
        }
        internal bool TeacherChooseSecondCareer
        {
            get
            {
                var tempList = SingleInfo!.GetCareerList();
                if (tempList.Count == 2)
                    return false; //already has it.
                if (tempList.Count == 0)
                    return false;
                if (tempList.Single().Career != EnumCareerType.Teacher)
                    return false;
                return AllHasCareer;
            }
        }
        //hopefully this simple here too.
        internal int WhoHadCareerName(EnumCareerType thisCareer)
        {
            foreach (var thisPlayer in PlayerList!)
            {
                var tempList = thisPlayer.GetCareerList();
                if (tempList.Any(items => items.Career == thisCareer))
                    return thisPlayer.Id;
            }
            return 0;
        }
        public int PlayerHas100000 => 19.PlayerHasCard(PlayerList!);
        public SalaryInfo Get100000Card => CardsModule.GetSalaryCard(19);

        public bool CanTradeForBig(bool isAthlete)
        {
            int player = PlayerHas100000;
            if (player == WhoTurn)
                return false; //because you already have it.
            int whoHadAthlete = WhoHadCareerName(EnumCareerType.Athlete);
            int whoHadEntertainer = WhoHadCareerName(EnumCareerType.Entertainer);
            if (whoHadAthlete != WhoTurn && isAthlete)
                return false; //because you are not an athlete
            if (whoHadEntertainer != WhoTurn && isAthlete == false)
                return false; //because you are not an entertainer.
            if (isAthlete)
            {
                return SingleInfo!.TilesCollected > 3;
            }
            if (SaveRoot!.SpinList.Count < 2)
                return false;
            if (Test!.DoubleCheck == true)
                return true; //to test the entertainer part.
            var tempList = SaveRoot.SpinList.Skip(SaveRoot.SpinList.Count - 2).ToCustomBasicList();
            if (tempList.Count != 2)
                throw new BasicBlankException("Must have 2 items");
            if (tempList.First() < 8)
                return false;
            return tempList.First() == tempList.Last();
        }
        public bool CanSpin => GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedToSpin || GameStatus == EnumWhatStatus.NeedFindSellPrice || GameStatus == EnumWhatStatus.NeedSellHouse || GameStatus == EnumWhatStatus.NeedSellBuyHouse;

        public async Task ShowCardAsync(LifeBaseCard card)
        {
            if (HideCardAsync == null)
            {
                throw new BasicBlankException("Nobody is hiding the card.  Rethink");
            }
            card.IsUnknown = false;
            _model.SinglePile.AddCard(card);
            await Aggregator.PublishAsync(new ShowCardEventModel(SaveRoot.GameStatus));
            if (Test.NoAnimations == false)
            {
                await Delay.DelaySeconds(.75);
            }
            await HideCardAsync.Invoke();
        }
        public async Task ShowHouseAsync()
        {
            await Aggregator.PublishAsync(new ShowCardEventModel(SaveRoot.GameStatus)); //just this.
        }

        #region Screen Delegates

        //internal Func<EnumWhatStatus, Task>? ClosePreviousScreenAsync { get; set; }
        /// <summary>
        /// showing card means it has to hide the hand.
        /// </summary>
        //internal Func<EnumWhatStatus, Task>? ShowCardAsync { get; set; }
        internal Func<Task>? HideCardAsync { get; set; }
        internal Func<bool>? CardVisible { get; set; }
        #endregion

        internal void AddPlayerChoices(CustomBasicList<string> thisList)
        {
            _model.PlayerChosen = "";
            _model.PlayerPicker.LoadTextList(thisList);
            _model.PlayerPicker.UnselectAll();
            if (thisList.Count == 1)
            {
                _model.PlayerChosen = thisList.Single();// obviously should show it selected because no choice.
                _model.PlayerPicker.SelectSpecificItem(1); //because there is only one.
            }
        }

        public void TakeOutExpense(decimal whatAmount)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            do
            {
                if (SingleInfo.MoneyEarned > whatAmount)
                    break;
                SingleInfo.MoneyEarned += 20000;
                SingleInfo.Loans += 20000;
            } while (true);
            SingleInfo.MoneyEarned -= whatAmount;
        }
        internal void ObtainLife(LifeBoardGamePlayerItem thisPlayer)
        {
            thisPlayer.TilesCollected++;
        }


        //has to do trade salary first.


        internal bool WasBig { get; set; }
        internal async Task TradeForBigAsync()
        {
            int whichPlayer = PlayerHas100000;
            if (whichPlayer == 0)
            {
                var thisSalary = SingleInfo!.GetSalaryCard();
                SingleInfo!.Hand.RemoveSpecificItem(thisSalary);
                SingleInfo.Hand.Add(Get100000Card);
                SingleInfo.Salary = 100000;
                PopulatePlayerProcesses.FillInfo(SingleInfo);
                if (GameStatus == EnumWhatStatus.LastSpin)
                {
                    IMoveProcesses move = Resolver.Resolve<IMoveProcesses>();
                    await move.PossibleAutomateMoveAsync();
                    return;
                }
                await ContinueTurnAsync!.Invoke(); //iffy.
                return;
            }
            WasBig = true;
            ITradeSalaryProcesses tradeSalary = Resolver.Resolve<ITradeSalaryProcesses>();
            tradeSalary.LoadOtherPlayerSalaries();
            var thisPlayer = PlayerList![whichPlayer];
            WasBig = false;
            await tradeSalary.TradedSalaryAsync(thisPlayer.NickName);
        }
        internal void ProcessCommission()
        {
            int whoHad = WhoHadCareerName(EnumCareerType.SalesPerson);
            if (whoHad > 0)
            {
                CollectMoney(whoHad, 5000);
            }
        }
        public void CollectMoney(int player, decimal moneyCollected)
        {
            var thisPlayer = PlayerList![player];
            thisPlayer.MoneyEarned += moneyCollected;
            PopulatePlayerProcesses.FillInfo(thisPlayer);
        }

    }
}