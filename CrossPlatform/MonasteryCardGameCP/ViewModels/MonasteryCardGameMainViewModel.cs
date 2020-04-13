using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MonasteryCardGameCP.Data;
using MonasteryCardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace MonasteryCardGameCP.ViewModels
{
    [InstanceGame]
    public class MonasteryCardGameMainViewModel : BasicCardGamesVM<MonasteryCardInfo>
    {
        private readonly MonasteryCardGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly MonasteryCardGameVMData _model;
        private readonly BasicData _basicData;
        private readonly MonasteryCardGameGameContainer _gameContainer;

        public MonasteryCardGameMainViewModel(CommandContainer commandContainer,
            MonasteryCardGameMainGameClass mainGame,
            MonasteryCardGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            MonasteryCardGameGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _basicData = basicData;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            var player = _mainGame.PlayerList.GetSelf();
            mainGame.Aggregator.Subscribe(player); //hopefully this works now.

            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard();
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync += MainSets_SetClickedAsync;
            _model.PlayerHand1!.AutoSelect = HandObservable<MonasteryCardInfo>.EnumAutoType.SelectAsMany;
            _model.MainSets.SendEnableProcesses(this, () =>
            {
                if (_gameContainer!.AlreadyDrew == false)
                    return false;
                return _mainGame.SingleInfo!.FinishedCurrentMission;
            });
        }

        protected override Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync -= MainSets_SetClickedAsync;
            return base.TryCloseAsync();
        }
        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
            _model.TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
        private DeckRegularDict<MonasteryCardInfo> GetSelectCards()
        {
            var firstList = _model.PlayerHand1!.ListSelectedObjects();
            var newCol = _model.TempSets!.ListSelectedObjects();
            DeckRegularDict<MonasteryCardInfo> output = new DeckRegularDict<MonasteryCardInfo>();
            output.AddRange(firstList);
            output.AddRange(newCol);
            return output;
        }

        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return !_gameContainer.AlreadyDrew;
        }

        protected override bool CanEnablePile1()
        {
            return true;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            if (_mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message) == false)
            {
                await UIPlatform.ShowMessageAsync(message);
                return;
            }
            if (pickUp == true)
            {
                await _mainGame.PickupFromDiscardAsync();
                return;
            }
            await _gameContainer.SendDiscardMessageAsync(deck);
            await _mainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private string _missionChosen = "";
        [VM]
        public string MissionChosen
        {
            get { return _missionChosen; }
            set
            {
                if (SetProperty(ref _missionChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
            {
                await UIPlatform.ShowMessageAsync("You must click on an existing set in order to expand.");
                return;
            }
            if (_mainGame!.SingleInfo!.FinishedCurrentMission == false)
                throw new BasicBlankException("Should have been disabled because you did not finish the mission");
            var thisCol = GetSelectCards();
            if (thisCol.Count > 2)
            {
                await UIPlatform.ShowMessageAsync("You cannot select more than 2 cards");
                return;
            }
            if (thisCol.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("There are no cards selected");
                return;
            }
            var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
            bool doubles = thisSet.IsDoubleRun;
            if (doubles && thisCol.Count < 2)
            {
                await UIPlatform.ShowMessageAsync("There needs to be 2 cards selected because this is a double run of three");
                return;
            }
            if (doubles == false && thisCol.Count > 1)
            {
                await UIPlatform.ShowMessageAsync("Since its not a double run of three, then can only choose one card at a time");
                return;
            }
            int newpos = 0;
            foreach (var thisCard in thisCol)
            {
                newpos = thisSet.PositionToPlay(thisCard, section);
                if (newpos == 0)
                {
                    await UIPlatform.ShowMessageAsync("Sorry, cannot use the card to expand upon");
                    return;
                }
            }
            int x = 0;
            int nums = 0;
            foreach (var thisRummy in _model.MainSets.SetList)
            {
                x++;
                if (thisRummy.Equals(thisSet))
                {
                    nums = x;
                    break;
                }
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendExpandSet temps = new SendExpandSet();
                temps.SetNumber = nums;
                temps.Position = newpos;
                temps.CardData = await js.SerializeObjectAsync(thisCol.GetDeckListFromObjectList()); //i think.
                await _mainGame.Network!.SendAllAsync("expandset", temps);
            }
            await _mainGame.ExpandSetAsync(thisCol, nums, newpos);
        }
        [Command(EnumCommandCategory.Old)]
        public async Task MissionDetailAsync()
        {
            await UIPlatform.ShowMessageAsync("Mission 1:  2 sets of 3 in color" + Constants.vbCrLf + "Mission 2:  3 sets of 3" + Constants.vbCrLf + "Mission 3:  1 set of 4, 1 run or 4" + Constants.vbCrLf + "Mission 4:  1 run of 5 in suit" + Constants.vbCrLf + "Mission 5:  1 run of 6 in color" + Constants.vbCrLf + "Mission 6:  1 run of 8: " + Constants.vbCrLf + "Mission 7:  1 double run of 3" + Constants.vbCrLf + "Mission 8:  7 cards of the same suit" + Constants.vbCrLf + "Mission 9:  9 cards of even rank (2, 4, 6, 8, 10, 12) or 9 cards of odd rank (1, 3, 5, 7, 9, 11, 13)");
        }
        public bool CanSelectPossibleMission()
        {
            if (!_gameContainer!.AlreadyDrew)
                return false; //has to draw first.
            return !_mainGame.SingleInfo!.FinishedCurrentMission;
        }
        [Command(EnumCommandCategory.Game)]
        public void SelectPossibleMission(MissionList mission)
        {
            if (_basicData.IsXamarinForms)
            {
                MissionChosen = mission.Description; //has to do this way because xamarin forms is too slow.
            }
            _model.MissionChosen = mission.Description;
        }
        public bool CanCompleteChosenMission()
        {
            if (_mainGame!.SingleInfo!.FinishedCurrentMission)
                return false;
            return string.IsNullOrEmpty(MissionChosen) == false;
        }

        [Command(EnumCommandCategory.Game)]
        public async Task CompleteChosenMissionAsync()
        {
            bool rets = _mainGame!.DidCompleteMission(out CustomBasicList<InstructionInfo> tempList);
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you did not complete the mission");
                MissionChosen = "";
                return;
            }
            _mainGame.ProcessCurrentMission();
            CustomBasicList<string> mList = new CustomBasicList<string>();
            await tempList.ForEachAsync(async thisTemp =>
            {
                var thisCol = _model.TempSets.ObjectList(thisTemp.SetNumber).ToRegularDeckDict();
                if (thisCol.Count == 0)
                    throw new BasicBlankException("Cannot have 0 items");
                if (_mainGame.BasicData!.MultiPlayer)
                {
                    SendNewSet thisSend = new SendNewSet();
                    thisSend.Index = thisTemp.WhichOne;
                    var newCol = thisCol.GetDeckListFromObjectList();
                    thisSend.CardData = await js.SerializeObjectAsync(newCol);
                    thisSend.MissionCompleted = MissionChosen;
                    string results = await js.SerializeObjectAsync(thisSend);
                    mList.Add(results);
                }
                _model.TempSets.ClearBoard(thisTemp.SetNumber);
                _mainGame.CreateNewSet(thisCol, thisTemp.WhichOne);
            });
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendSeveralSetsAsync(mList, "finished");
            await _mainGame.FinishedAsync();
        }

    }
}