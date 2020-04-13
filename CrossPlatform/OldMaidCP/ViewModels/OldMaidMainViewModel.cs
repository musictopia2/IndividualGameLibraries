using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using OldMaidCP.Data;
using OldMaidCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OldMaidCP.ViewModels
{
    [InstanceGame]
    public class OldMaidMainViewModel : BasicCardGamesVM<RegularSimpleCard>
    {
        private readonly OldMaidMainGameClass _mainGame; //if we don't need, delete.
        private readonly OldMaidVMData _model;
        private readonly IGamePackageResolver _resolver;

        public OldMaidMainViewModel(CommandContainer commandContainer,
            OldMaidMainGameClass mainGame,
            OldMaidVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            OldMaidGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _model.Deck1.NeverAutoDisable = false;
            _model.PlayerHand1.AutoSelect = HandObservable<RegularSimpleCard>.EnumAutoType.SelectAsMany;
            gameContainer.ShowOtherCardsAsync = LoadOpponentScreenAsync;
        }

        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            if (_mainGame.SaveRoot.RemovePairs == false)
            {
                await LoadOpponentScreenAsync();
            }
        }

        private async Task LoadOpponentScreenAsync()
        {
            if (OpponentScreen != null)
            {
                return;
            }
            OpponentScreen = _resolver.Resolve<OpponentCardsViewModel>();
            await LoadScreenAsync(OpponentScreen);
        }

        protected override async Task TryCloseAsync()
        {
            if (OpponentScreen != null)
            {
                await CloseSpecificChildAsync(OpponentScreen);
                OpponentScreen = null;
            }
            await base.TryCloseAsync();
        }


        public OpponentCardsViewModel? OpponentScreen { get; set; }

        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return true; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            var thisCol = _model.PlayerHand1!.ListSelectedObjects();
            if (thisCol.Count != 2)
            {
                await UIPlatform.ShowMessageAsync("Must select 2 cards to throw away");
                return;
            }
            if (_mainGame!.IsValidMove(thisCol) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal move");
            }
            await _mainGame.ProcessPlayAsync(thisCol.First().Deck, thisCol.Last().Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}