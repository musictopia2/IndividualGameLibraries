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
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FourSuitRummyCP.Data;
using FourSuitRummyCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace FourSuitRummyCP.ViewModels
{
    [InstanceGame]
    public class FourSuitRummyMainViewModel : BasicCardGamesVM<RegularRummyCard>
    {
        private readonly FourSuitRummyMainGameClass _mainGame; //if we don't need, delete.
        private readonly FourSuitRummyVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly FourSuitRummyGameContainer _gameContainer;

        public FourSuitRummyMainViewModel(CommandContainer commandContainer,
            FourSuitRummyMainGameClass mainGame,
            FourSuitRummyVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            FourSuitRummyGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            var player = _mainGame.PlayerList.GetSelf();
            _mainGame.Aggregator.Subscribe(player);
            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.PlayerHand1.AutoSelect = HandObservable<RegularRummyCard>.EnumAutoType.SelectAsMany;
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            YourSetsScreen = _resolver.Resolve<PlayerSetsViewModel>();
            await LoadScreenAsync(YourSetsScreen);
            OpponentSetsScreen = _resolver.Resolve<PlayerSetsViewModel>();
            await LoadScreenAsync(OpponentSetsScreen);
        }

        public PlayerSetsViewModel? YourSetsScreen { get; set; }
        public PlayerSetsViewModel? OpponentSetsScreen { get; set; }

        protected override async Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            await CloseSpecificChildAsync(YourSetsScreen!);
            await CloseSpecificChildAsync(OpponentSetsScreen!);
            await base.TryCloseAsync();
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
        public bool CanPlaySets => _gameContainer.AlreadyDrew;

        [Command(EnumCommandCategory.Game)]
        public async Task PlaySetsAsync()
        {
            CustomBasicList<string> textList = new CustomBasicList<string>();
            var thisCol = _mainGame!.SetList();
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
            if (thisCol.Count == 0)
                return;
            if (_mainGame.Test!.DoubleCheck == true && thisCol.Count > 1)
            {
                throw new BasicBlankException("cannot have more than one for now for sets in beginning");
            }
            await thisCol.ForEachAsync(async thisInt =>
            {
                var temps = _model.TempSets.ObjectList(thisInt);
                var newCol = temps.ToRegularDeckDict();
                if (_mainGame.SingleInfo.MainSets!.CanAddSet(temps))
                {
                    if (_mainGame.BasicData!.MultiPlayer == true)
                    {
                        var tempList = newCol.GetDeckListFromObjectList();
                        var thisStr = await js.SerializeObjectAsync(tempList);
                        textList.Add(thisStr);
                    }
                    _model.TempSets.ClearBoard(thisInt);
                    _mainGame.AddSet(newCol);
                }
            });
            if (_gameContainer.Test.DoubleCheck == true && textList.Count > 1)
                throw new BasicBlankException("canno have more than one for now for sets for sending to players");
            if (_gameContainer.BasicData!.MultiPlayer == true && textList.Count > 0)
                await _gameContainer.Network!.SendSeveralSetsAsync(textList, "finishedsets");
            await _mainGame.ContinueTurnAsync(); //i think this may work.
        }

        protected override bool CanEnableDeck()
        {
            return !_gameContainer!.AlreadyDrew;
        }
        protected override bool CanEnablePile1()
        {
            return true; //otherwise, can't compile.
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
    }
}