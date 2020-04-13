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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Messenging;
using Spades2PlayerCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.Cards;
//i think this is the most common things i like to do
namespace Spades2PlayerCP.ViewModels
{
    [InstanceGame]
    public class Spades2PlayerMainViewModel : TrickCardGamesVM<Spades2PlayerCardInformation, EnumSuitList>
    {
        private readonly Spades2PlayerMainGameClass _mainGame; //if we don't need, delete.
        private readonly Spades2PlayerVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly Spades2PlayerGameContainer _gameContainer; //if not needed, delete.

        public Spades2PlayerMainViewModel(CommandContainer commandContainer,
            Spades2PlayerMainGameClass mainGame,
            Spades2PlayerVMData viewModel, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            Spades2PlayerGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            GameStatus = _model.GameStatus;
        }
        
        public SpadesBeginningViewModel? BeginningScreen { get; set; }
        public SpadesBiddingViewModel? BiddingScreen { get; set; }
        protected override Task ActivateAsync()
        {
            ScreenChangeAsync();
            return base.ActivateAsync();
        }
        private async void ScreenChangeAsync()
        {
            if (_isClosed)
            {
                return; //because somehow its being held on to.
            }
            if (_model == null)
            {
                return;
            }
            if (_processing)
            {
                return;
            }
            _processing = true;
            if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.Normal)
            {
                await CloseBeginningScreenAsync();
                await CloseBiddingScreenAsync();
                _model.TrickArea1.Visible = true; //i think.
                _processing = false;
                return;
            }
            _model.TrickArea1.Visible = false;
            if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.Bidding)
            {
                await CloseBeginningScreenAsync();
                await OpenBiddingAsync();
                _processing = false;
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.ChooseCards)
            {
                await CloseBiddingScreenAsync();
                await OpenBeginningAsync();
                _processing = false;
                return;
            }
            throw new BasicBlankException("Not supported.  Rethink");

        }
        private async Task CloseBeginningScreenAsync()
        {
            if (BeginningScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BeginningScreen);
            BeginningScreen = null;
        }
        private async Task CloseBiddingScreenAsync()
        {
            if (BiddingScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BiddingScreen);
            BiddingScreen = null;
        }
        private async Task OpenBeginningAsync()
        {
            if (BeginningScreen != null)
            {
                return;
            }
            BeginningScreen = _resolver.Resolve<SpadesBeginningViewModel>();
            await LoadScreenAsync(BeginningScreen);
        }
        private async Task OpenBiddingAsync()
        {
            if (BiddingScreen != null)
            {
                return;
            }
            BiddingScreen = _resolver.Resolve<SpadesBiddingViewModel>();
            await LoadScreenAsync(BiddingScreen);
        }

        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return _mainGame!.SaveRoot!.GameStatus == EnumGameStatus.ChooseCards;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            _model.OtherPile!.Visible = false;
            await _gameContainer!.SendDiscardMessageAsync(_model.OtherPile!.GetCardInfo().Deck);
            await _mainGame!.DiscardAsync(_model.OtherPile.GetCardInfo().Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private int _roundNumber;
        [VM]
        public int RoundNumber
        {
            get { return _roundNumber; }
            set
            {
                if (SetProperty(ref _roundNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _processing;

        private EnumGameStatus _gameStatus;
        [VM]
        public EnumGameStatus GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (SetProperty(ref _gameStatus, value))
                {
                    //can decide what to do when property changes
                    //show screen changes.
                    ScreenChangeAsync();
                }

            }
        }
        private bool _isClosed = false;
        protected override Task TryCloseAsync()
        {
            _isClosed = true;
            return base.TryCloseAsync();
        }
    }
}