using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CousinRummyCP.Data;
using CousinRummyCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace CousinRummyCP.ViewModels
{
    [InstanceGame]
    public class CousinRummyMainViewModel : BasicCardGamesVM<RegularRummyCard>
    {
        private readonly CousinRummyMainGameClass _mainGame; //if we don't need, delete.
        private readonly CousinRummyVMData _model;
        private readonly CousinRummyGameContainer _gameContainer;

        public CousinRummyMainViewModel(CommandContainer commandContainer,
            CousinRummyMainGameClass mainGame,
            CousinRummyVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            CousinRummyGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = false;
            _model.PlayerHand1.AutoSelect = HandObservable<RegularRummyCard>.EnumAutoType.SelectAsMany;
            var player = _mainGame.PlayerList.GetSelf();
            mainGame.Aggregator.Subscribe(player); //hopefully this works now.
            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync += MainSets_SetClickedAsync;
            _model.MainSets.SendEnableProcesses(this, () =>
            {
                if (_mainGame!.OtherTurn > 0)
                    return false;
                return _mainGame.SingleInfo!.LaidDown;
            });
        }
        protected override Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync -= MainSets_SetClickedAsync;
            return base.TryCloseAsync();
        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return _mainGame!.OtherTurn == 0;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int counts = _model.PlayerHand1!.HowManySelectedObjects;
            int others = _model.TempSets!.HowManySelectedObjects;
            if (counts + others > 1)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you can only select one card to discard");
                return;
            }
            if (counts + others == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must select a card to discard");
                return;
            }
            int index;
            int deck;
            if (counts == 0)
            {
                index = _model.TempSets.PileForSelectedObject;
                deck = _model.TempSets.DeckForSelectedObjected(index);
            }
            else
            {
                deck = _model.PlayerHand1.ObjectSelected();
            }
            await _gameContainer!.SendDiscardMessageAsync(deck);
            await _mainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private string _phaseData = "";
        [VM]
        public string PhaseData
        {
            get { return _phaseData; }
            set
            {
                if (SetProperty(ref _phaseData, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _otherLabel = "";
        [VM]
        public string OtherLabel
        {
            get { return _otherLabel; }
            set
            {
                if (SetProperty(ref _otherLabel, value))
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
        private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
                return;
            var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
            bool rets;
            rets = _mainGame!.CanAddToSet(thisSet, out RegularRummyCard? thisCard, out string message);
            if (rets == false)
            {
                if (message != "")
                    await UIPlatform.ShowMessageAsync(message);
                return;
            }
            int nums = setNumber;
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendExpandedSet thiss = new SendExpandedSet();
                thiss.Deck = thisCard!.Deck;
                thiss.Number = nums;
                await _mainGame.Network!.SendAllAsync("expandrummy", thiss);
            }
            await _mainGame.AddToSetAsync(nums, thisCard!.Deck);
        }
        public bool CanPass => !CanEnablePile1();
        [Command(EnumCommandCategory.Game)]
        public async Task PassAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("pass");
            await _mainGame!.PassAsync();
        }

        public bool CanBuy => !CanEnablePile1();
        [Command(EnumCommandCategory.Game)]
        public async Task BuyAsync()
        {
            await _mainGame!.PickupFromDiscardAsync();
        }

        public bool CanFirstSets
        {
            get
            {
                if (_mainGame!.OtherTurn > 0)
                    return false;
                return !_mainGame.SingleInfo!.LaidDown;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task FirstSetsAsync()
        {
            if (_mainGame!.CanLayDownInitialSets() == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you do not have the required sets yet");
                return;
            }
            var thisCol = _mainGame.ListValidSets(true);
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisCol.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                    var thisStr = await js.SerializeObjectAsync(tempList);
                    newList.Add(thisStr);
                }
                _mainGame.CreateNewSet(thisTemp);
            });
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
            await _mainGame!.LaidDownInitialSetsAsync();
        }
        public bool CanOtherSets
        {
            get
            {
                if (_mainGame!.OtherTurn > 0)
                    return false;
                return _mainGame.SingleInfo!.LaidDown;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task OtherSetsAsync()
        {
            var thisCol = _mainGame!.ListValidSets(false);
            if (thisCol.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you do not have any more sets to put down");
                return;
            }
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisCol.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                    var thisStr = await js.SerializeObjectAsync(tempList);
                    newList.Add(thisStr);
                }
                _mainGame.CreateNewSet(thisTemp);
            });
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendSeveralSetsAsync(newList, "laydownothers");
            await _mainGame.LayDownOtherSetsAsync();
        }

    }
}