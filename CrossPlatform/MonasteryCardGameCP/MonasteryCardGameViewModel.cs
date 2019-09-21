using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace MonasteryCardGameCP
{
    public class MonasteryCardGameViewModel : BasicCardGamesVM<MonasteryCardInfo, MonasteryCardGamePlayerItem, MonasteryCardGameMainGameClass>
    {
        private string _MissionChosen = "";

        public string MissionChosen
        {
            get { return _MissionChosen; }
            set
            {
                if (SetProperty(ref _MissionChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public CustomBasicCollection<MissionList> CompleteMissions = new CustomBasicCollection<MissionList>();
        private DeckRegularDict<MonasteryCardInfo> GetSelectCards()
        {
            var firstList = PlayerHand1!.ListSelectedObjects();
            var newCol = TempSets!.ListSelectedObjects();
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            output.AddRange(firstList);
            output.AddRange(newCol);
            return output;
        }
        internal void PopulateMissions(CustomBasicList<MissionList> thisList)
        {
            MissionChosen = "";
            CompleteMissions.ReplaceRange(thisList);
        }
        public MonasteryCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
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
        public TempSetsViewModel<EnumSuitList, EnumColorList, MonasteryCardInfo>? TempSets;
        public MainSetsViewModel<EnumSuitList, EnumColorList, MonasteryCardInfo, RummySet, SavedSet>? MainSets;
        public BasicGameCommand<MissionList>? SelectPossibleMissionCommand { get; set; }
        public BasicGameCommand? CompleteChosenMissionCommand { get; set; }
        public Command? MissionDetailCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            TempSets = new TempSetsViewModel<EnumSuitList, EnumColorList, MonasteryCardInfo>();
            MainSets = new MainSetsViewModel<EnumSuitList, EnumColorList, MonasteryCardInfo, RummySet, SavedSet>(this);
            TempSets.HowManySets = 4;
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            MainSets.SetClickedAsync += MainSets_SetClickedAsync;
            PlayerHand1!.AutoSelect = HandViewModel<MonasteryCardInfo>.EnumAutoType.SelectAsMany;
            MainSets.SendEnableProcesses(this, () =>
            {
                if (MainGame!.AlreadyDrew == false)
                    return false;
                return MainGame.SingleInfo!.FinishedCurrentMission;
            });
            MissionDetailCommand = new Command(async items =>
            {
                await ShowGameMessageAsync("Mission 1:  2 sets of 3 in color" + Constants.vbCrLf + "Mission 2:  3 sets of 3" + Constants.vbCrLf + "Mission 3:  1 set of 4, 1 run or 4" + Constants.vbCrLf + "Mission 4:  1 run of 5 in suit" + Constants.vbCrLf + "Mission 5:  1 run of 6 in color" + Constants.vbCrLf + "Mission 6:  1 run of 8: " + Constants.vbCrLf + "Mission 7:  1 double run of 3" + Constants.vbCrLf + "Mission 8:  7 cards of the same suit" + Constants.vbCrLf + "Mission 9:  9 cards of even rank (2, 4, 6, 8, 10, 12) or 9 cards of odd rank (1, 3, 5, 7, 9, 11, 13)");
            }, items => true, this);
            SelectPossibleMissionCommand = new BasicGameCommand<MissionList>(this, thisMission =>
            {
                MissionChosen = thisMission.Description;
            }, items =>
            {
                if (!MainGame!.AlreadyDrew)
                    return false; //has to draw first.
                return !MainGame.SingleInfo!.FinishedCurrentMission;
            }, this, CommandContainer!);
            CompleteChosenMissionCommand = new BasicGameCommand(this, async items =>
            {
                bool rets = MainGame!.DidCompleteMission(out CustomBasicList<InstructionInfo> tempList);
                if (rets == false)
                {
                    await ShowGameMessageAsync("Sorry, you did not complete the mission");
                    MissionChosen = "";
                    return;
                }
                MainGame.ProcessCurrentMission();
                CustomBasicList<string> mList = new CustomBasicList<string>();
                await tempList.ForEachAsync(async thisTemp =>
                {
                    var thisCol = TempSets.ObjectList(thisTemp.SetNumber).ToRegularDeckDict();
                    if (thisCol.Count == 0)
                        throw new BasicBlankException("Cannot have 0 items");
                    if (ThisData!.MultiPlayer)
                    {
                        SendNewSet thisSend = new SendNewSet();
                        thisSend.Index = thisTemp.WhichOne;
                        var newCol = thisCol.GetDeckListFromObjectList();
                        thisSend.CardData = await js.SerializeObjectAsync(newCol);
                        thisSend.MissionCompleted = MissionChosen;
                        string results = await js.SerializeObjectAsync(thisSend);
                        mList.Add(results);
                    }
                    TempSets.ClearBoard(thisTemp.SetNumber);
                    MainGame.CreateNewSet(thisCol, thisTemp.WhichOne);
                });
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendSeveralSetsAsync(mList, "finished");
                await MainGame.FinishedAsync();
            }, items =>
            {
                if (MainGame!.SingleInfo!.FinishedCurrentMission)
                    return false;
                return string.IsNullOrEmpty(MissionChosen) == false;
            }, this, CommandContainer!);
        }
        private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
            {
                await ShowGameMessageAsync("You must click on an existing set in order to expand.");
                return;
            }
            if (MainGame!.SingleInfo!.FinishedCurrentMission == false)
                throw new BasicBlankException("Should have been disabled because you did not finish the mission");
            var thisCol = GetSelectCards();
            if (thisCol.Count > 2)
            {
                await ShowGameMessageAsync("You cannot select more than 2 cards");
                return;
            }
            if (thisCol.Count == 0)
            {
                await ShowGameMessageAsync("There are no cards selected");
                return;
            }
            var thisSet = MainSets!.GetIndividualSet(setNumber);
            bool doubles = thisSet.IsDoubleRun;
            if (doubles && thisCol.Count < 2)
            {
                await ShowGameMessageAsync("There needs to be 2 cards selected because this is a double run of three");
                return;
            }
            if (doubles == false && thisCol.Count > 1)
            {
                await ShowGameMessageAsync("Since its not a double run of three, then can only choose one card at a time");
                return;
            }
            int newpos = 0;
            foreach (var thisCard in thisCol)
            {
                newpos = thisSet.PositionToPlay(thisCard, section);
                if (newpos == 0)
                {
                    await ShowGameMessageAsync("Sorry, cannot use the card to expand upon");
                    return;
                }
            }
            int x = 0;
            int nums = 0;
            foreach (var thisRummy in MainSets.SetList)
            {
                x++;
                if (thisRummy.Equals(thisSet))
                {
                    nums = x;
                    break;
                }
            }
            if (ThisData!.MultiPlayer)
            {
                SendExpandSet temps = new SendExpandSet();
                temps.SetNumber = nums;
                temps.Position = newpos;
                temps.CardData = await js.SerializeObjectAsync(thisCol.GetDeckListFromObjectList()); //i think.
                await ThisNet!.SendAllAsync("expandset", temps);
            }
            await MainGame.ExpandSetAsync(thisCol, nums, newpos);
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