using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using PyramidSolitaireCP;
using SolitaireGraphicsXF;
using System.Runtime.Serialization;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PyramidSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<PyramidSolitaireViewModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>? _deckGPile;
        private BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>? _discardGPile;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            var currentCard = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            var playerBoard = new CardBoardXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            var triangle1 = new TriangleXF();
            Grid grid = new Grid();
            3.Times(x => AddLeftOverColumn(grid, 33));
            AddAutoRows(grid, 1);
            AddLeftOverRow(grid, 1);
            AddControlToGrid(grid, triangle1, 0, 0);
            AddControlToGrid(grid, thisStack, 1, 0);
            Grid.SetColumnSpan(triangle1, 3);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;
            _discardGPile.HorizontalOptions = LayoutOptions.Start;
            _discardGPile.VerticalOptions = LayoutOptions.Start;
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            currentCard.HorizontalOptions = LayoutOptions.Start;
            currentCard.VerticalOptions = LayoutOptions.Start;
            currentCard.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Init(ThisMod!.Discard!, ts.TagUsed);
            currentCard.Init(ThisMod.CurrentPile!, ts.TagUsed);
            playerBoard.LoadList(ThisMod.PlayList1!, ts.TagUsed);
            triangle1.Init(ThisMod.GameBoard1!);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            otherStack.Children.Add(currentCard);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(GameButton);
            StackLayout finalStack = new StackLayout();
            AddControlToGrid(grid, finalStack, 0, 2);
            var playButton = GetSmallerButton("Play Selected Cards", nameof(PyramidSolitaireViewModel.PlaySelectedCardsCommand));
            MakeGameButtonSmaller(GameButton);
            finalStack.Children.Add(playerBoard);
            finalStack.Children.Add(playButton);
            playButton.HorizontalOptions = LayoutOptions.Start;
            playButton.VerticalOptions = LayoutOptions.Start;
            var thisLabel = new SimpleLabelGridXF();
            thisLabel.AddRow("Score", nameof(PyramidSolitaireViewModel.Score));
            thisStack.Children.Add(thisLabel.GetContent);
            _deckGPile.Init(ThisMod.DeckPile!, ts.TagUsed);
            Content = grid; //if not doing this, rethink.
            await ThisMod.StartNewGameAsync();
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<PyramidSolitaireViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .60f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.1f;
                return 1.5f;
            }
        }
    }
}