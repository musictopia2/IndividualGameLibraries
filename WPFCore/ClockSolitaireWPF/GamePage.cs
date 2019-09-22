using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using ClockSolitaireCP;
using SolitaireGraphicsWPFCore;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace ClockSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<ClockSolitaireViewModel>
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
            MainClockWPF thisClock = new MainClockWPF();
            Grid thisGrid = new Grid();
            SimpleLabelGrid thisLabel = new SimpleLabelGrid();
            thisLabel.AddRow("Cards", nameof(ClockSolitaireViewModel.CardsLeft));
            thisStack.Children.Add(thisLabel.GetContent);
            thisStack.Children.Add(GameButton);
            thisGrid.Children.Add(thisStack);
            thisClock.Margin = new Thickness(5, 10, 0, 0);
            thisGrid.Children.Add(thisClock);
            Content = thisGrid;
            await ThisMod!.StartNewGameAsync();
            thisClock.LoadControls(ThisMod.Clock1!);
            ThisMod.Clock1!.SendSavedMessage();
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<ClockSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.5f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}