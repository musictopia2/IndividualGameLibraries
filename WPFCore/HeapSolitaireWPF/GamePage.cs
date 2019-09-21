using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using HeapSolitaireCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HeapSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<HeapSolitaireViewModel>
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
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            var thisMain = new BasicMultiplePilesWPF<HeapSolitaireCardInfo, ts, DeckOfCardsWPF<HeapSolitaireCardInfo>>();
            thisMain.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Children.Add(thisMain);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            CustomWasteUI thisWaste = new CustomWasteUI();
            otherStack.Children.Add(thisWaste);
            thisStack.Children.Add(otherStack);
            StackPanel finalStack = new StackPanel();
            SimpleLabelGrid scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(HeapSolitaireViewModel.Score));
            finalStack.Children.Add(scoresAlone.GetContent);
            finalStack.Children.Add(GameButton);
            otherStack.Children.Add(finalStack);
            Content = thisStack; //if not doing this, rethink.
            await ThisMod!.StartNewGameAsync();
            thisMain.Init(ThisMod.Main1!, ts.TagUsed);
            thisWaste.Init(ThisMod);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<HeapSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<HeapSolitaireCardInfo>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.8f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}