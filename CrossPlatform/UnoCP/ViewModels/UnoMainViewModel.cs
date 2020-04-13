using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using UnoCP.Cards;
using UnoCP.Data;
using UnoCP.Logic;

namespace UnoCP.ViewModels
{
    [InstanceGame]
    public class UnoMainViewModel : BasicCardGamesVM<UnoCardInformation>
    {
        private readonly UnoMainGameClass _mainGame; //if we don't need, delete.
        private readonly UnoVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly UnoGameContainer _gameContainer; //if not needed, delete.

        public SayUnoViewModel? SayUnoScreen { get; set; }

        public UnoMainViewModel(CommandContainer commandContainer,
            UnoMainGameClass mainGame,
            UnoVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            UnoGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _gameContainer.OpenSaidUnoAsync = LoadSayUnoAsync;
            _gameContainer.CloseSaidUnoAsync = CloseSayUnoAsync;
        }
        //anything else needed is here.
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
            {
                await LoadSayUnoAsync();
            }
        }
        private async Task LoadSayUnoAsync()
        {
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                return;
            }
            SayUnoScreen = _resolver.Resolve<SayUnoViewModel>();
            await LoadScreenAsync(SayUnoScreen);
        }
        private async Task CloseSayUnoAsync()
        {
            if (SayUnoScreen != null)
            {
                await CloseSpecificChildAsync(SayUnoScreen);
                SayUnoScreen = null;
            }
        }
        protected override bool CanEnableDeck()
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
                return false;
            if (_gameContainer.AlreadyDrew == true)
                return false;
            return CanDraw();
        }

        protected override bool CanEnablePile1()
        {
            return _mainGame!.SaveRoot!.GameStatus == EnumGameStatus.NormalPlay;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int deck = _model.PlayerHand1!.ObjectSelected();
            if (deck == 0)
            {
                await UIPlatform.ShowMessageAsync("You must select a card first");
                return;
            }
            if (_mainGame!.CanPlay(deck) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                _model.PlayerHand1.UnselectAllObjects();
                return;
            }
            await _mainGame.ProcessPlayAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private string _nextPlayer = "";
        [VM]
        public string NextPlayer
        {
            get
            {
                return _nextPlayer;
            }

            set
            {
                if (SetProperty(ref _nextPlayer, value) == true)
                {
                }
            }
        }
        private bool CanDraw()
        {
            return !_mainGame!.SingleInfo!.MainHandList.Any(x => _mainGame.CanPlay(x.Deck));
        }
        public override bool CanEndTurn()
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
                return false;
            if (CanDraw() == false)
                return false;// because you are able to play something.  if you have something to play, you must play it.  even if its what you drew.
            return _gameContainer.AlreadyDrew;
        }
    }
}