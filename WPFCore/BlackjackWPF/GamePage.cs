using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using BlackjackCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BlackjackWPF
{
    public class GamePage : SinglePlayerWindow<BlackjackViewModel>
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
            BaseDeckWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> deckGPile = new BaseDeckWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>>();
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 1);
            AddLeftOverColumn(thisGrid, 1); // i think
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            StackPanel thisStack = new StackPanel();
            thisStack.Children.Add(deckGPile);
            deckGPile.Margin = new Thickness(5, 5, 5, 5);
            deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            thisStack.Children.Add(GameButton);
            SimpleLabelGrid otherGrid = new SimpleLabelGrid();
            otherGrid.AddRow("Wins", nameof(BlackjackViewModel.Wins));
            otherGrid.AddRow("Losses", nameof(BlackjackViewModel.Losses));
            otherGrid.AddRow("Draws", nameof(BlackjackViewModel.Draws));
            thisStack.Children.Add(otherGrid.GetContent);
            thisGrid.Children.Add(thisStack);
            thisStack = new StackPanel();
            BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> humanHand = new BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>>();
            BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> computerHand = new BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>>();
            humanHand.HandType = HandViewModel<BlackjackCardInfo>.EnumHandList.Horizontal;
            computerHand.HandType = HandViewModel<BlackjackCardInfo>.EnumHandList.Horizontal;
            humanHand.Margin = new Thickness(3, 3, 3, 10);
            computerHand.Margin = new Thickness(3, 3, 3, 10);
            thisStack.Children.Add(computerHand);
            thisStack.Children.Add(humanHand);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            otherGrid = new SimpleLabelGrid();
            otherGrid.AddRow("Human Points", nameof(BlackjackViewModel.HumanPoints));
            otherGrid.AddRow("Computer Points", nameof(BlackjackViewModel.ComputerPoints));
            thisStack.Children.Add(otherGrid.GetContent);
            var thisBut = GetGamingButton("Hit Me", nameof(BlackjackViewModel.HitCommand)); // hopefully margins set automatically (well find out)
            StackPanel newStack = new StackPanel();
            newStack.Orientation = Orientation.Horizontal;
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Stay", nameof(BlackjackViewModel.StayCommand));
            finalStack.Children.Add(thisBut);
            newStack.Children.Add(finalStack);
            finalStack = new StackPanel();
            finalStack.Margin = new Thickness(50, 0, 0, 0);
            thisBut = GetGamingButton("Use Ace As One (1)", nameof(BlackjackViewModel.AceCommand));
            thisBut.CommandParameter = BlackjackViewModel.EnumAceChoice.Low;
            var thisBind = GetVisibleBinding(nameof(BlackjackViewModel.NeedsAceChoice));
            finalStack.SetBinding(VisibilityProperty, thisBind); // since its going to this stack, will make all based on this one.
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Use Ace As Eleven (11)", nameof(BlackjackViewModel.AceCommand));
            thisBut.CommandParameter = BlackjackViewModel.EnumAceChoice.High;
            finalStack.Children.Add(thisBut);
            newStack.Children.Add(finalStack);
            thisStack.Children.Add(newStack);
            Content = thisGrid;
            await ThisMod.StartNewGameAsync();
            humanHand.LoadList(ThisMod.HumanStack!, ts.TagUsed);
            computerHand.LoadList(ThisMod.ComputerStack!, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<BlackjackViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<BlackjackCardInfo>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.3f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}