using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace CousinRummyCP
{
    public class CousinRummyViewModel : BasicCardGamesVM<RegularRummyCard, CousinRummyPlayerItem, CousinRummyMainGameClass>
    {
        public CousinRummyViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return MainGame!.OtherTurn == 0;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            int counts = PlayerHand1!.HowManySelectedObjects;
            int others = TempSets!.HowManySelectedObjects;
            if (counts + others > 1)
            {
                await ShowGameMessageAsync("Sorry, you can only select one card to discard");
                return;
            }
            if (counts + others == 0)
            {
                await ShowGameMessageAsync("Sorry, you must select a card to discard");
                return;
            }
            int index;
            int deck;
            if (counts == 0)
            {
                index = TempSets.PileForSelectedObject;
                deck = TempSets.DeckForSelectedObjected(index);
            }
            else
            {
                deck = PlayerHand1.ObjectSelected();
            }
            await MainGame!.SendDiscardMessageAsync(deck);
            await MainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private string _PhaseData = "";

        public string PhaseData
        {
            get { return _PhaseData; }
            set
            {
                if (SetProperty(ref _PhaseData, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _OtherLabel = "";

        public string OtherLabel
        {
            get { return _OtherLabel; }
            set
            {
                if (SetProperty(ref _OtherLabel, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>? TempSets;
        public MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, PhaseSet, SavedSet>? MainSets;
        public BasicGameCommand? PassCommand { get; set; }
        public BasicGameCommand? BuyCommand { get; set; }
        public BasicGameCommand? InitSetsCommand { get; set; }
        public BasicGameCommand? OtherSetsCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = false; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<RegularRummyCard>.EnumAutoType.SelectAsMany;
            TempSets = new TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>();
            MainSets = new MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, PhaseSet, SavedSet>(this);
            MainSets.Text = "Main Sets";
            TempSets.HowManySets = 8; //no feature for autoselected.
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            MainSets.SetClickedAsync += MainSets_SetClickedAsync;
            MainSets.SendEnableProcesses(this, () =>
            {
                if (MainGame!.OtherTurn > 0)
                    return false;
                return MainGame.SingleInfo!.LaidDown;
            });
            PassCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("pass");
                await MainGame!.PassAsync();
            }, items => MainGame!.OtherTurn > 0, this, CommandContainer!);
            BuyCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.PickupFromDiscardAsync();
            }, items => MainGame!.OtherTurn > 0, this, CommandContainer!);
            InitSetsCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.CanLayDownInitialSets() == false)
                {
                    await ShowGameMessageAsync("Sorry; you do not have the required sets yet");
                    return;
                }
                var thisCol = MainGame.ListValidSets(true);
                CustomBasicList<string> newList = new CustomBasicList<string>();
                await thisCol.ForEachAsync(async thisTemp =>
                {
                    if (ThisData!.MultiPlayer == true)
                    {
                        var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                        var thisStr = await js.SerializeObjectAsync(tempList);
                        newList.Add(thisStr);
                    }
                    MainGame.CreateNewSet(thisTemp);
                });
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendSeveralSetsAsync(newList, "laiddowninitial");
                await MainGame.LaidDownInitialSetsAsync();
            }, items =>
            {
                if (MainGame!.OtherTurn > 0)
                    return false;
                return !MainGame.SingleInfo!.LaidDown;
            }, this, CommandContainer!);
            OtherSetsCommand = new BasicGameCommand(this, async items =>
            {
                var thisCol = MainGame!.ListValidSets(false);
                if (thisCol.Count == 0)
                {
                    await ShowGameMessageAsync("Sorry; you do not have any more sets to put down");
                    return;
                }
                CustomBasicList<string> newList = new CustomBasicList<string>();
                await thisCol.ForEachAsync(async thisTemp =>
                {
                    if (ThisData!.MultiPlayer == true)
                    {
                        var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                        var thisStr = await js.SerializeObjectAsync(tempList);
                        newList.Add(thisStr);
                    }
                    MainGame.CreateNewSet(thisTemp);
                });
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendSeveralSetsAsync(newList, "laydownothers");
                await MainGame.LayDownOtherSetsAsync();
            }, items =>
            {
                if (MainGame!.OtherTurn > 0)
                    return false;
                return MainGame.SingleInfo!.LaidDown;
            }, this, CommandContainer!);
        }
        private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
                return;
            var thisSet = MainSets!.GetIndividualSet(setNumber);
            bool rets;
            rets = MainGame!.CanAddToSet(thisSet, out RegularRummyCard? thisCard, out string message);
            if (rets == false)
            {
                if (message != "")
                    await ShowGameMessageAsync(message);
                return;
            }
            int nums = setNumber;
            if (ThisData!.MultiPlayer == true)
            {
                SendExpandedSet thiss = new SendExpandedSet();
                thiss.Deck = thisCard!.Deck;
                thiss.Number = nums;
                await ThisNet!.SendAllAsync("expandrummy", thiss);
            }
            await MainGame.AddToSetAsync(nums, thisCard!.Deck);
        }
        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = PlayerHand1!.ListSelectedObjects(true);
            TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
    }
}