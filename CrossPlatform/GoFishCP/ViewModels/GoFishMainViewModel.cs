using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GoFishCP.Data;
using GoFishCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GoFishCP.ViewModels
{
    [InstanceGame]
    public class GoFishMainViewModel : BasicCardGamesVM<RegularSimpleCard>
    {
        private readonly GoFishMainGameClass _mainGame; //if we don't need, delete.
        private readonly GoFishVMData _model;
        private readonly IGamePackageResolver _resolver;

        public GoFishMainViewModel(CommandContainer commandContainer,
            GoFishMainGameClass mainGame,
            GoFishVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            GoFishGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _model.Deck1.NeverAutoDisable = false;
            _model.PlayerHand1.AutoSelect = HandObservable<RegularSimpleCard>.EnumAutoType.SelectAsMany;
            gameContainer.LoadAskScreenAsync = LoadAskScreenAsync;
        }


        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            if (_mainGame.SaveRoot.RemovePairs == false)
            {
                await LoadAskScreenAsync();
            }
        }

        private async Task LoadAskScreenAsync()
        {
            if (AskScreen != null)
            {
                return;
            }
            AskScreen = _resolver.Resolve<AskViewModel>();
            await LoadScreenAsync(AskScreen);
        }

        protected override async Task TryCloseAsync()
        {
            if (AskScreen != null)
            {
                await CloseSpecificChildAsync(AskScreen);
                AskScreen = null;
            }
            await base.TryCloseAsync();
        }


        public AskViewModel? AskScreen { get; set; }

        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return true; //otherwise, can't compile.
        }
        public override bool CanEndTurn()
        {
            return _mainGame!.SaveRoot!.RemovePairs == true || _mainGame.SaveRoot.NumberAsked == true;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            var thisList = _model.PlayerHand1!.ListSelectedObjects();
            if (thisList.Count != 2)
            {
                await UIPlatform.ShowMessageAsync("Must select 2 cards to throw away");
                return;
            }
            if (_mainGame!.IsValidMove(thisList) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendPair thisPair = new SendPair();
                thisPair.Card1 = thisList.First().Deck;
                thisPair.Card2 = thisList.Last().Deck;
                await _mainGame.Network!.SendAllAsync("processplay", thisPair);
            }
            await _mainGame.ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}