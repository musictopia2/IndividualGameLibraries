using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using FlorentineSolitaireCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace FlorentineSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<FlorentineSolitaireViewModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>? _deckGPile;
        private BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>? _discardGPile;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _discardGPile = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2);
            var thisMain = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            thisStack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(FlorentineSolitaireViewModel.AutoMoveCommand));
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(FlorentineSolitaireViewModel.Score));
            scoresAlone.AddRow("Starting Number", nameof(FlorentineSolitaireViewModel.StartingNumber));
            var tempGrid = scoresAlone.GetContent;
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(autoBut);
            var miscDiscard = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            _discardGPile.Init(ThisMod.MainDiscardPile!, ts.TagUsed);
            thisStack.Children.Add(GameButton);
            AddControlToGrid(thisGrid, thisStack, 0, 0);
            thisStack = new StackPanel();
            thisStack.Children.Add(thisMain);
            thisStack.Children.Add(miscDiscard);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            Content = thisGrid;
            await ThisMod.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            miscDiscard.Init(tempWaste.Discards!, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FlorentineSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
        float IProportionImage.Proportion => 2.9f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}