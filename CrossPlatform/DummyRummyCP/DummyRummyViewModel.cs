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
namespace DummyRummyCP
{
    public class DummyRummyViewModel : BasicCardGamesVM<RegularRummyCard, DummyRummyPlayerItem, DummyRummyMainGameClass>
    {
        public DummyRummyViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !MainGame!.AlreadyDrew;
        }
        protected override bool CanEnablePile1()
        {
            return true;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            if (MainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message) == false)
            {
                await ShowGameMessageAsync(message);
                return;
            }
            if (pickUp == true)
            {
                await MainGame.PickupFromDiscardAsync();
                return;
            }
            await MainGame.SendDiscardMessageAsync(deck);
            await MainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private int _UpTo;

        public int UpTo
        {
            get { return _UpTo; }
            set
            {
                if (SetProperty(ref _UpTo, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>? TempSets;
        public MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, DummySet, SavedSet>? MainSets;
        public OutOfTurnCommand? BackCommand { get; set; }
        public BasicGameCommand? LayDownSetsCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = false;
            BackCommand = new OutOfTurnCommand(this, items =>
            {
                //you have the ability to put back when you put into temp sets.
                var thisList = TempSets!.ListSelectedObjects();
                thisList.ForEach(thisCard =>
                {
                    thisCard.IsSelected = false;
                    TempSets.RemoveObject(thisCard.Deck);
                });
                //no private save here now.
                DummyRummyPlayerItem thisPlayer = MainGame!.PlayerList!.GetSelf();
                thisPlayer.MainHandList.AddRange(thisList);
                MainGame.SortCards(); //i think.

            }, this, CommandContainer!);
            LayDownSetsCommand = new BasicGameCommand(this, async items =>
            {

                if (MainGame!.SaveRoot!.PlayerWentOut > 0)
                {
                    bool lats = await MainGame.CanLaterLayDownAsync();
                    if (lats == false)
                        return;
                    var thisCol1 = MainGame.ListValidSets();
                    await ProcessValidSetsAsync(thisCol1);
                    return;
                }
                bool rets = MainGame.HasInitialSet();
                if (rets == false)
                {
                    await ShowGameMessageAsync("Sorry, you do not have the valid sets needed to go out");
                    return;
                }
                var thisCol2 = MainGame.ListValidSets();
                await ProcessValidSetsAsync(thisCol2);
            }, items =>
            {
                if (MainGame!.AlreadyDrew == false)
                    return false;
                return !MainGame.SaveRoot!.SetsCreated;
            }, this, CommandContainer!);
            TempSets = new TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>();
            MainSets = new MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, DummySet, SavedSet>(this);
            TempSets.HowManySets = 6; //no feature for autoselected.
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<RegularRummyCard>.EnumAutoType.SelectAsMany;
            MainSets.SendEnableProcesses(this, () => false); //always disabled this time.
        }
        private async Task ProcessValidSetsAsync(CustomBasicList<TempInfo> thisCol)
        {
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisCol.ForEachAsync(async thisTemp =>
            {
                if (ThisData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();

                    var thisStr = await js.SerializeObjectAsync(tempList);
                    newList.Add(thisStr);
                }
                TempSets!.ClearBoard(thisTemp.SetNumber); // i think.
                MainGame!.CreateSet(thisTemp.CardList);
            });
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendSeveralSetsAsync(newList, "finishedsets");
            await MainGame!.FinishedSetsAsync();
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