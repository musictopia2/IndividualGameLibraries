using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using HuseHeartsCP.Cards;
using HuseHeartsCP.Data;
using HuseHeartsCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HuseHeartsCP.ViewModels
{
    [InstanceGame]
    public class HuseHeartsMainViewModel : TrickCardGamesVM<HuseHeartsCardInformation, EnumSuitList>
    {
        private readonly HuseHeartsMainGameClass _mainGame; //if we don't need, delete.
        private readonly HuseHeartsVMData _model;
        private readonly IGamePackageResolver _resolver;

        public HuseHeartsMainViewModel(CommandContainer commandContainer,
            HuseHeartsMainGameClass mainGame,
            HuseHeartsVMData viewModel,
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
            _model.Blind1.SendEnableProcesses(this, () => false); //you can't even enable this one.
            _model.Dummy1.SendEnableProcesses(this, () =>
            {
                if (_mainGame!.SaveRoot!.GameStatus != EnumStatus.Normal)
                    return false;
                return _model!.TrickArea1!.FromDummy;
            });
            GameStatus = _model.GameStatus;
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            ChangeScreenAsync();
        }
        public MoonViewModel? MoonScreen { get; set; }
        public PassingViewModel? PassingScreen { get; set; }

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
        private EnumStatus _gameStatus;
        [VM]
        public EnumStatus GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (SetProperty(ref _gameStatus, value))
                {
                    ChangeScreenAsync();
                }

            }
        }
        private async Task LoadMoonAsync()
        {
            if (MoonScreen != null)
            {
                return;
            }
            MoonScreen = _resolver.Resolve<MoonViewModel>();
            await LoadScreenAsync(MoonScreen);
        }
        private async void ChangeScreenAsync()
        {
            if (_model == null)
            {
                return;
            }
            _model.TrickArea1.Visible = GameStatus == EnumStatus.Normal; //try this way (?)
            //hopefully does not need new view model this time (option is open if necessary though).
            if (GameStatus == EnumStatus.ShootMoon)
            {
                await LoadMoonAsync();
                return;
            }
            await CloseMoonAsync();
            if (GameStatus == EnumStatus.Passing)
            {
                await LoadPassingAsync();
                return;
            }
            if (PassingScreen != null)
            {
                await CloseSpecificChildAsync(PassingScreen);
                PassingScreen = null;
            }
        }
        private async Task CloseMoonAsync()
        {
            if (MoonScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(MoonScreen);
            MoonScreen = null;
        }
        private async Task LoadPassingAsync()
        {
            if (PassingScreen != null)
            {
                return;
            }
            PassingScreen = _resolver.Resolve<PassingViewModel>();
            await LoadScreenAsync(PassingScreen);
        }

        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            if (_mainGame!.SaveRoot!.GameStatus == EnumStatus.Passing)
                return true;
            if (_mainGame.SaveRoot.GameStatus == EnumStatus.Normal)
            {
                if (_model!.TrickArea1!.FromDummy == true)
                    return false;
                return true;
            }
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
    }
}