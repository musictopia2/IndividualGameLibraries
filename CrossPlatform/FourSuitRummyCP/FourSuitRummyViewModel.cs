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

namespace FourSuitRummyCP
{
    public class FourSuitRummyViewModel : BasicCardGamesVM<RegularRummyCard, FourSuitRummyPlayerItem, FourSuitRummyMainGameClass>
    {
        public FourSuitRummyViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !MainGame!.AlreadyDrew;
        }
        protected override bool CanEnablePile1()
        {
            return true; //otherwise, can't compile.
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
        public TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>? TempSets;
        public BasicGameCommand? PlaySetsCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
			Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            TempSets = new TempSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard>();
            TempSets.HowManySets = 5; //no feature for autoselected.
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            PlayerHand1!.AutoSelect = BasicGameFramework.DrawableListsViewModels.HandViewModel<RegularRummyCard>.EnumAutoType.SelectAsMany;
            PlaySetsCommand = new BasicGameCommand(this, async items =>
            {
                CustomBasicList<string> textList = new CustomBasicList<string>();
                var thisCol = MainGame!.SetList();
                MainGame.SingleInfo = MainGame.PlayerList!.GetWhoPlayer();
                if (thisCol.Count == 0)
                    return;
                if (ThisTest!.DoubleCheck == true && thisCol.Count > 1)
                {
                    throw new BasicBlankException("cannot have more than one for now for sets in beginning");
                }
                await thisCol.ForEachAsync(async thisInt =>
                {
                    var temps = TempSets.ObjectList(thisInt);
                    var newCol = temps.ToRegularDeckDict();
                    if (MainGame.SingleInfo.MainSets!.CanAddSet(temps))
                    {
                        if (ThisData!.MultiPlayer == true)
                        {
                            var tempList = newCol.GetDeckListFromObjectList();
                            var thisStr = await js.SerializeObjectAsync(tempList);
                            textList.Add(thisStr);
                        }
                        TempSets.ClearBoard(thisInt);
                        MainGame.AddSet(newCol);
                    }
                });
                if (ThisTest.DoubleCheck == true && textList.Count > 1)
                    throw new BasicBlankException("canno have more than one for now for sets for sending to players");
                if (ThisData!.MultiPlayer == true && textList.Count > 0)
                    await ThisNet!.SendSeveralSetsAsync(textList, "finishedsets");
                await MainGame.ContinueTurnAsync(); //i think this may work.
            }, items => MainGame!.AlreadyDrew, this, CommandContainer!);
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