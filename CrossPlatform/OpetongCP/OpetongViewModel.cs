using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.MainViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.Extensions;

namespace OpetongCP
{
    public class OpetongViewModel : BasicCardGamesVM<RegularRummyCard, OpetongPlayerItem, OpetongMainGameClass>
    {
        private string _Instructions = "";

        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public OpetongViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return true;
        }
        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }
        protected override Task ProcessDiscardClickedAsync()
        {
            throw new BasicBlankException("Discard should never be used this time.");
        }
        public TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>? TempSets;
        public MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, RummySet, SavedSet>? MainSets;
        public CardPool? Pool1;
        public BasicGameCommand? PlaySetCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
			Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<RegularRummyCard>.EnumAutoType.SelectAsMany;
            TempSets = new TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>();
            TempSets.HowManySets = 3;
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            MainSets = new MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, RummySet, SavedSet>(this);
            MainSets.SendEnableProcesses(this, () => false);
            Pool1 = new CardPool(this);
            PlaySetCommand = new BasicGameCommand(this, async items =>
            {
                int nums = MainGame!.FindValidSet();
                if (nums == 0)
                {
                    await ShowGameMessageAsync("Sorry, there is no valid set here");
                    return;
                }
                var thisCol = TempSets.ObjectList(nums).ToRegularDeckDict();
                TempSets.ClearBoard(nums);
                if (ThisData!.MultiPlayer)
                {
                    var tempCol = thisCol.GetDeckListFromObjectList();
                    await ThisNet!.SendAllAsync("newset", tempCol);
                }
                await MainGame.PlaySetAsync(thisCol);
            }, items => true, this, CommandContainer!);
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
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}