using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;
using FiveCrownsCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace FiveCrownsCP.ViewModels
{
    [InstanceGame]
    public class FiveCrownsMainViewModel : BasicCardGamesVM<FiveCrownsCardInformation>
    {
        private readonly FiveCrownsMainGameClass _mainGame; //if we don't need, delete.
        private readonly FiveCrownsVMData _model;
        private readonly FiveCrownsGameContainer _gameContainer; //if not needed, delete.

        public FiveCrownsMainViewModel(CommandContainer commandContainer,
            FiveCrownsMainGameClass mainGame,
            FiveCrownsVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            FiveCrownsGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = false;
            var player = _mainGame.PlayerList.GetSelf();
            mainGame.Aggregator.Subscribe(player); //hopefully this works now.
            _model.PlayerHand1.AutoSelect = HandObservable<FiveCrownsCardInformation>.EnumAutoType.SelectAsMany;
            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.MainSets.SendEnableProcesses(this, () => false); //always disabled this time.
        }



        [Command(EnumCommandCategory.OutOfTurn)]
        public void Back() //you can put back even if its not your turn.
        {
            var thisList = _model.TempSets!.ListSelectedObjects();
            thisList.ForEach(thisCard =>
            {
                thisCard.IsSelected = false;
                _model.TempSets.RemoveObject(thisCard.Deck);
            });
            //no private save here now.
            FiveCrownsPlayerItem thisPlayer = _mainGame!.PlayerList!.GetSelf();
            thisPlayer.MainHandList.AddRange(thisList);
            _mainGame.SortCards(); //i think.
        }

        public bool CanLayDownSets()
        {
            if (_gameContainer!.AlreadyDrew == false)
                return false;
            return !_mainGame.SaveRoot!.SetsCreated;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task LayDownSetsAsync()
        {
            if (_mainGame!.SaveRoot!.PlayerWentOut > 0)
            {
                bool lats = await _mainGame.CanLaterLayDownAsync();
                if (lats == false)
                    return;
                var thisCol1 = _mainGame.ListValidSets();
                await ProcessValidSetsAsync(thisCol1);
                return;
            }
            bool rets = _mainGame.HasInitialSet();
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you do not have the valid sets needed to go out");
                return;
            }
            var thisCol2 = _mainGame.ListValidSets();
            await ProcessValidSetsAsync(thisCol2);
        }
        protected override Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            return base.TryCloseAsync();
        }

        protected override bool CanEnableDeck()
        {
            return !_gameContainer!.AlreadyDrew;
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


        private int _upTo;
        [VM]
        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    //can decide what to do when property changes
                }

            }
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

        private async Task ProcessValidSetsAsync(CustomBasicList<TempInfo> thisCol)
        {
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisCol.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();

                    var thisStr = await js.SerializeObjectAsync(tempList);
                    newList.Add(thisStr);
                }
                _model.TempSets!.ClearBoard(thisTemp.SetNumber); // i think.
                _mainGame!.CreateSet(thisTemp.CardList);
            });
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendSeveralSetsAsync(newList, "finishedsets");
            await _mainGame!.FinishedSetsAsync();
        }
    }
}