using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using BlackjackCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BlackjackXF
{
    public class GamePage : SinglePlayerGamePage<BlackjackViewModel>
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
        protected override async Task AfterGameButtonAsync()
        {
            BaseDeckXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> deckGPile = new BaseDeckXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>>();
            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 30);
            AddLeftOverColumn(thisGrid, 30);
            AddAutoColumns(thisGrid, 2);
            AddLeftOverRow(thisGrid, 40);
            AddLeftOverRow(thisGrid, 50);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            StackLayout thisStack = new StackLayout();
            thisStack.Spacing = 0;
            thisStack.Children.Add(deckGPile);
            deckGPile.Margin = new Thickness(5, 5, 5, 5);
            deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            thisStack.Children.Add(GameButton);
            SimpleLabelGridXF otherGrid = new SimpleLabelGridXF();
            otherGrid.AddRow("Wins", nameof(BlackjackViewModel.Wins));
            otherGrid.AddRow("Losses", nameof(BlackjackViewModel.Losses));
            otherGrid.AddRow("Draws", nameof(BlackjackViewModel.Draws));
            thisStack.Children.Add(otherGrid.GetContent);
            otherGrid = new SimpleLabelGridXF();
            otherGrid.AddRow("Human Points", nameof(BlackjackViewModel.HumanPoints));
            otherGrid.AddRow("Computer Points", nameof(BlackjackViewModel.ComputerPoints));
            thisStack.Children.Add(otherGrid.GetContent);
            thisGrid.Children.Add(thisStack);
            thisStack = new StackLayout();
            AddControlToGrid(thisGrid, thisStack, 1, 0);
            Grid.SetColumnSpan(thisStack, 2);
            var thisBut = GetGamingButton("Hit Me", nameof(BlackjackViewModel.HitCommand)); // hopefully margins set automatically (well find out)
            thisStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Stay", nameof(BlackjackViewModel.StayCommand));
            thisStack.Children.Add(thisBut);
            StackLayout finalStack = new StackLayout();
            thisBut = GetGamingButton("Use Ace As One (1)", nameof(BlackjackViewModel.AceCommand));
            thisBut.CommandParameter = BlackjackViewModel.EnumAceChoice.Low;
            var thisBind = new Binding(nameof(BlackjackViewModel.NeedsAceChoice));
            finalStack.SetBinding(IsVisibleProperty, thisBind); // since its going to this stack, will make all based on this one.
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Use Ace As Eleven (11)", nameof(BlackjackViewModel.AceCommand));
            thisBut.CommandParameter = BlackjackViewModel.EnumAceChoice.High;
            finalStack.Children.Add(thisBut);
            thisStack.Children.Add(finalStack);
            thisStack = new StackLayout();
            BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> humanHand = new BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>>();
            BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> computerHand = new BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>>();
            humanHand.HandType = HandViewModel<BlackjackCardInfo>.EnumHandList.Horizontal;
            computerHand.HandType = HandViewModel<BlackjackCardInfo>.EnumHandList.Horizontal;
            humanHand.Margin = new Thickness(3, 3, 3, 10);
            computerHand.Margin = new Thickness(3, 3, 3, 10);
            thisStack.Children.Add(computerHand);
            thisStack.Children.Add(humanHand);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            Content = thisGrid;
            await ThisMod.StartNewGameAsync();
            humanHand.LoadList(ThisMod.HumanStack!, ts.TagUsed);
            computerHand.LoadList(ThisMod.ComputerStack!, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<BlackjackViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<BlackjackCardInfo>>(true); //i think
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
                    return .75f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }
    }
}