using AndyCristinaGamePackageCP.ExtensionClasses;
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
using CalculationSolitaireCP;
using SolitaireGraphicsWPFCore;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CalculationSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<CalculationSolitaireViewModel>
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
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            var thisMain = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(CalculationSolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesWPF();
            thisWaste.Margin = new Thickness(10, 10, 5, 5);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            thisStack.Children.Add(_deckGPile);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(GameButton);
            otherStack.Children.Add(thisStack);
            var thisGrid = new Grid();
            AddAutoRows(thisGrid, 1);
            AddLeftOverRow(thisGrid, 1);
            AddControlToGrid(thisGrid, thisMain, 0, 0);
            AddControlToGrid(thisGrid, thisWaste, 1, 0);
            otherStack.Children.Add(thisGrid);
            Content = otherStack;
            await ThisMod.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            thisWaste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<CalculationSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
        float IProportionImage.Proportion => 2.3f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}