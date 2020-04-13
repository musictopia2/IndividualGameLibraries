using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using RookCP.Cards;
using RookCP.Data;
using RookCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RookCP.ViewModels
{
    [InstanceGame]
    public class RookMainViewModel : TrickCardGamesVM<RookCardInformation, EnumColorTypes>
    {
        private readonly RookMainGameClass _mainGame; //if we don't need, delete.
        private readonly RookVMData _model;
        private readonly IGamePackageResolver _resolver;

        public RookMainViewModel(CommandContainer commandContainer,
            RookMainGameClass mainGame,
            RookVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _model.Deck1.NeverAutoDisable = true;
            _model.Dummy1.SendEnableProcesses(this, (() =>
            {
                if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
                    return false;
                if (_mainGame.PlayerList.Count() == 3)
                    return false;
                return _mainGame.SaveRoot.DummyPlay;
            }));
        }
        public RookBiddingViewModel? BidScreen { get; set; }
        public RookNestViewModel? NestScreen { get; set; }
        public RookColorViewModel? ColorScreen { get; set; }
        protected override Task ActivateAsync()
        {
            ScreenChangeAsync();
            return base.ActivateAsync();
        }

        private async void ScreenChangeAsync()
        {
            if (_mainGame == null)
            {
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
            {
                await CloseNestScreenAsync();
                await CloseBiddingScreenAsync();
                await CloseColorScreenAsync();
                _model.TrickArea1.Visible = true; //i think.
                return;
            }
            _model.TrickArea1.Visible = false;
            if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Bidding)
            {
                await CloseNestScreenAsync();
                await CloseColorScreenAsync();
                await OpenBiddingAsync();
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumStatusList.SelectNest)
            {
                await CloseBiddingScreenAsync();
                await CloseColorScreenAsync();
                await OpenNestAsync();
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumStatusList.ChooseTrump)
            {
                await CloseBiddingScreenAsync();
                await CloseNestScreenAsync();
                await OpenColorAsync();
                return;
            }
            throw new BasicBlankException("Not supported.  Rethink");

        }
        private async Task CloseBiddingScreenAsync()
        {
            if (BidScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BidScreen);
            BidScreen = null;
        }
        private async Task CloseNestScreenAsync()
        {
            if (NestScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(NestScreen);
            NestScreen = null;
        }
        private async Task CloseColorScreenAsync()
        {
            if (ColorScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ColorScreen);
            ColorScreen = null;
        }
        private async Task OpenBiddingAsync()
        {
            if (BidScreen != null)
            {
                return;
            }
            BidScreen = _resolver.Resolve<RookBiddingViewModel>();
            await LoadScreenAsync(BidScreen);
        }
        private async Task OpenNestAsync()
        {
            if (NestScreen != null)
            {
                return;
            }
            NestScreen = _resolver.Resolve<RookNestViewModel>();
            await LoadScreenAsync(NestScreen);
        }
        private async Task OpenColorAsync()
        {
            if (ColorScreen != null)
            {
                return;
            }
            ColorScreen = _resolver.Resolve<RookColorViewModel>();
            await LoadScreenAsync(ColorScreen);
        }


        public override bool CanEndTurn()
        {
            return false;
        }

        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private EnumStatusList _gameStatus;
        [VM]
        public EnumStatusList GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (SetProperty(ref _gameStatus, value))
                {
                    ScreenChangeAsync();
                }

            }
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            if (_mainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest)
                return true;
            if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
                return !_mainGame.SaveRoot.DummyPlay;
            return false;
        }
    }
}