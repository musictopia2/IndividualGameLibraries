using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using PokerCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PokerWPF
{
    public class GamePage : SinglePlayerWindow<PokerViewModel>
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
        private BaseDeckWPF<PokerCardInfo, ts, DeckOfCardsWPF<PokerCardInfo>>? _deckGPile;
        private readonly HandUI _thisPoker = new HandUI(); //so it can hook up properly.
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            _deckGPile = new BaseDeckWPF<PokerCardInfo, ts, DeckOfCardsWPF<PokerCardInfo>>();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            BetUI thisBet = new BetUI();
            thisBet.Margin = new Thickness(60, 5, 10, 10);
            otherStack.Children.Add(thisBet);
            var tempButton = GetGamingButton("New Round", nameof(PokerViewModel.NewRoundCommand));
            tempButton.HorizontalAlignment = HorizontalAlignment.Left;
            tempButton.VerticalAlignment = VerticalAlignment.Center;
            otherStack.Children.Add(tempButton);
            otherStack.Children.Add(GameButton);
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid thisLabel = new SimpleLabelGrid();
            CurrencyConverter thisConvert = new CurrencyConverter();
            thisLabel.AddRow("Money", nameof(PokerViewModel.Money), thisConvert);
            thisLabel.AddRow("Round", nameof(PokerViewModel.Round));
            thisLabel.AddRow("Winnings", nameof(PokerViewModel.Winnings), thisConvert);
            thisLabel.AddRow("Hand", nameof(PokerViewModel.HandLabel));
            thisStack.Children.Add(thisLabel.GetContent);
            _thisPoker.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Children.Add(_thisPoker);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            Content = thisStack; //if not doing this, rethink.
            await ThisMod.StartNewGameAsync();
            _thisPoker.Init(ThisMod);
            thisBet.Init(ThisMod);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<PokerViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<PokerCardInfo>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, aces are low.
            OurContainer.RegisterSingleton(_thisPoker);
            OurContainer.RegisterType<CustomProportion>();
        }
    }
    public class CustomProportion : IProportionImage, IWidthHeight
    {
        float IProportionImage.Proportion => 3.0f; //2.3 was standard size.  you can either increase or decrease as needed.

        int IWidthHeight.GetWidthHeight => 150;
    }
}