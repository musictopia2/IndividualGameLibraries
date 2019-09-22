using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
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
using SolitaireGraphicsWPFCore;
using SpiderSolitaireCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SpiderSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<SpiderSolitaireViewModel>
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
            ListChooserWPF level1 = new ListChooserWPF();
            level1.ItemHeight = 70;
            var thisMain = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            thisMain.Margin = new Thickness(0, 50, 5, 5);
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(SpiderSolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesWPF();
            thisStack.Margin = new Thickness(10, 5, 0, 0);
            otherStack.Children.Add(thisWaste);
            thisStack.Children.Add(level1);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(_deckGPile);
            otherStack.Children.Add(thisStack);
            thisStack = new StackPanel();
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(thisMain);
            otherStack.Children.Add(thisStack);
            Content = otherStack;
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            await ThisMod.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            thisWaste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            level1.LoadLists(ThisMod.Levels1!);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SpiderSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
        float IProportionImage.Proportion => 1.9f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}