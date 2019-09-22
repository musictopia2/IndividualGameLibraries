using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using PyramidSolitaireCP;
using SolitaireGraphicsWPFCore;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PyramidSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<PyramidSolitaireViewModel>
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
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _discardGPile = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            var currentCard = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            var playerBoard = new CardBoardWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            var triangle1 = new TriangleWPF();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            currentCard.HorizontalAlignment = HorizontalAlignment.Left;
            currentCard.VerticalAlignment = VerticalAlignment.Top;
            currentCard.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Init(ThisMod!.Discard!, ts.TagUsed);
            currentCard.Init(ThisMod.CurrentPile!, ts.TagUsed);
            playerBoard.LoadList(ThisMod.PlayList1!, ts.TagUsed);
            triangle1.Init(ThisMod.GameBoard1!);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            otherStack.Children.Add(currentCard);
            otherStack.Children.Add(triangle1);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(playerBoard);
            thisStack.Children.Add(GameButton);
            var playButton = GetGamingButton("Play Selected Cards", nameof(PyramidSolitaireViewModel.PlaySelectedCardsCommand));
            playButton.HorizontalAlignment = HorizontalAlignment.Left;
            playButton.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(playButton);
            var thisLabel = new SimpleLabelGrid();
            thisLabel.AddRow("Score", nameof(PyramidSolitaireViewModel.Score));
            thisStack.Children.Add(thisLabel.GetContent);
            _deckGPile.Init(ThisMod.DeckPile!, ts.TagUsed);
            Content = thisStack; //if not doing this, rethink.
            await ThisMod.StartNewGameAsync();
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<PyramidSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.3f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}