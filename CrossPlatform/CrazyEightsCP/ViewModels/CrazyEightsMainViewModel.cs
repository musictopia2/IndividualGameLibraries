using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CrazyEightsCP.Data;
using CrazyEightsCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CrazyEightsCP.ViewModels
{
    [InstanceGame]
    public class CrazyEightsMainViewModel : BasicCardGamesVM<RegularSimpleCard>
    {
        private readonly CrazyEightsMainGameClass _mainGame; //if we don't need, delete.
        private readonly IGamePackageResolver _resolver;
        private readonly CrazyEightsVMData _model;
        public CrazyEightsMainViewModel(CommandContainer commandContainer,
            CrazyEightsMainGameClass mainGame,
            CrazyEightsVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _resolver = resolver;
            _model = viewModel;
            viewModel.Deck1.NeverAutoDisable = true;
        }
        //anything else needed is here.
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            CheckSuitScreens();
        }
        protected override bool CanEnableDeck()
        {
            return SuitScreen == null;
        }

        public ChooseSuitViewModel? SuitScreen { get; set; }

        private async void CheckSuitScreens()
        {
            if (ChooseSuit == true)
            {
                if (SuitScreen != null)
                {
                    return;
                }
                SuitScreen = _resolver.Resolve<ChooseSuitViewModel>();
                await LoadScreenAsync(SuitScreen);
                return;
            }
            if (SuitScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(SuitScreen);
            SuitScreen = null;
        }

        protected override bool CanEnablePile1()
        {
            return SuitScreen == null;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int deck = _model.PlayerHand1!.ObjectSelected();
            if (deck == 0)
            {
                await UIPlatform.ShowMessageAsync("You must select a card first");
                return;
            }
            if (_mainGame.IsValidMove(deck) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                _model.PlayerHand1.UnselectAllObjects();
                return;
            }
            await _mainGame.PlayCardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private bool _chooseSuit;
        [VM]
        public bool ChooseSuit
        {
            get { return _chooseSuit; }
            set
            {
                if (SetProperty(ref _chooseSuit, value))
                {
                    //can decide what to do when property changes
                    CheckSuitScreens();
                }

            }
        }
    }
}