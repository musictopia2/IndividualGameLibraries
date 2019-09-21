using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.Extensions;
namespace Phase10CP
{
    public class Phase10ViewModel : BasicCardGamesVM<Phase10CardInformation, Phase10PlayerItem, Phase10MainGameClass>
    {
        public TempSetsViewModel<EnumColorTypes, EnumColorTypes, Phase10CardInformation>? TempSets; //has to be public so it can hook to ui.

        private string _CurrentPhase = "";

        public string CurrentPhase
        {
            get { return _CurrentPhase; }
            set
            {
                if (SetProperty(ref _CurrentPhase, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public Phase10ViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }

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
            bool rets = MainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message);
            if (rets == false)
            {
                if (message != "")
                {
                    PlayerHand1!.UnselectAllObjects();
                    TempSets!.UnselectAllCards(); //its best to just unselect everything if you can't discard.  that will solve some issues.
                    await ShowGameMessageAsync(message); //because on tablets, its possible it can't show message

                }
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
        public BasicGameCommand? CompletedPhaseCommand { get; set; }
        public MainSetsViewModel<EnumColorTypes, EnumColorTypes, Phase10CardInformation, PhaseSet, SavedSet>? MainSets;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            CompletedPhaseCommand = new BasicGameCommand(this, async items =>
            {
                bool rets;
                rets = MainGame!.DidCompletePhase(out int Manys);
                if (Manys == TempSets!.TotalObjects + MainGame.SingleInfo!.MainHandList.Count)
                {
                    await ShowGameMessageAsync("Cannot complete the phase.  Otherwise; there is no card to discard");
                    return;
                }
                if (rets == false)
                {
                    await ShowGameMessageAsync("Sorry, you did not complete the phase");
                    return;
                }
                var thisCol = MainGame.ListValidSets();
                CustomBasicList<string> newList = new CustomBasicList<string>();
                await thisCol.ForEachAsync(async thisTemp =>
                {
                    if (ThisData!.MultiPlayer == true)
                    {
                        SendNewSet thisSend = new SendNewSet();
                        thisSend.CardListData = await js.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                        thisSend.WhatSet = thisTemp.WhatSet;
                        string newStr = await js.SerializeObjectAsync(thisSend);
                        newList.Add(newStr);
                    }
                    MainGame.CreateNewSet(thisTemp);
                });
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendSeveralSetsAsync(newList, "phasecompleted");
                await MainGame.ProcessCompletedPhaseAsync();
            }, items =>
            {
                if (MainGame!.AlreadyDrew == false)
                    return false;
                return !MainGame.SaveRoot!.CompletedPhase;
            }, this, CommandContainer!);
            TempSets = new TempSetsViewModel<EnumColorTypes, EnumColorTypes, Phase10CardInformation>();
            TempSets.HowManySets = 5;
            PlayerHand1!.AutoSelect = HandViewModel<Phase10CardInformation>.EnumAutoType.SelectAsMany;
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            MainSets = new MainSetsViewModel<EnumColorTypes, EnumColorTypes, Phase10CardInformation, PhaseSet, SavedSet>(this);
            MainSets.SendEnableProcesses(this, () => MainGame!.AlreadyDrew);
            MainSets.SetClickedAsync += MainSets_SetClickedAsync;
        }
        private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
                return;
            //has to wait to do more of main before doing this.
            if (MainGame!.SaveRoot!.CompletedPhase == false)
            {
                await ShowGameMessageAsync("Sorry, the phase must be completed before expanding onto a set");
                return;
            }
            var thisSet = MainSets!.GetIndividualSet(setNumber);
            int position = section;
            bool rets;
            rets = MainGame.CanHumanExpand(thisSet, ref position, out Phase10CardInformation? thisCard, out string message);
            if (rets == false)
            {
                if (message != "")
                    await ShowGameMessageAsync(message);
                return;
            }
            if (ThisData!.MultiPlayer == true)
            {
                SendExpandedSet expands = new SendExpandedSet();
                expands.Deck = thisCard!.Deck;
                expands.Position = position;
                expands.Number = setNumber;
                await ThisNet!.SendAllAsync("expandrummy", expands);
            }
            await MainGame.ExpandHumanRummyAsync(setNumber, thisCard!.Deck, position);
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