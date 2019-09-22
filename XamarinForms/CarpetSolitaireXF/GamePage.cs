using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CarpetSolitaireCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CarpetSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<CarpetSolitaireViewModel>
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
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            var thisMain = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            thisStack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(CarpetSolitaireViewModel.AutoMoveCommand));
            autoBut.HorizontalOptions = LayoutOptions.Start;
            autoBut.VerticalOptions = LayoutOptions.Start;
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(CarpetSolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var miscDiscard = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            _discardGPile.Init(ThisMod.MainDiscardPile!, ts.TagUsed);
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2);
            tempGrid.WidthRequest = 50; //sometimes i am forced to do this 
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(autoBut);
            thisStack.Children.Add(GameButton);
            AddControlToGrid(thisGrid, thisStack, 0, 0);
            thisStack = new StackLayout();
            thisStack.Children.Add(thisMain);
            thisStack.Children.Add(miscDiscard);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            Content = thisGrid;
            await ThisMod.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            miscDiscard.Init(tempWaste.Discards!, ts.TagUsed);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<CarpetSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
            OurContainer.RegisterType<WastePiles>(); //can't do automatically because we don't know if we will do it or not.
            OurContainer.RegisterType<MainPilesCP>();
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .75f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }
    }
}