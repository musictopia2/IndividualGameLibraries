using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
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
using BasicXFControlsAndPages.Converters;
using PokerCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PokerXF
{
    public class GamePage : SinglePlayerGamePage<PokerViewModel>
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
        private BaseDeckXF<PokerCardInfo, ts, DeckOfCardsXF<PokerCardInfo>>? _deckGPile;
        private readonly HandUI _thisPoker = new HandUI(); //so it can hook up properly.
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            SetDefaultStartOrientations(GameButton!);
            _deckGPile = new BaseDeckXF<PokerCardInfo, ts, DeckOfCardsXF<PokerCardInfo>>();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            BetUI thisBet = new BetUI();
            thisBet.Margin = new Thickness(40, 5, 10, 10);
            otherStack.Children.Add(thisBet);
            var tempButton = GetSmallerButton("New Round", nameof(PokerViewModel.NewRoundCommand));
            MakeGameButtonSmaller(tempButton);
            tempButton.HorizontalOptions = LayoutOptions.Start;
            tempButton.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(tempButton);
            otherStack.Children.Add(GameButton);
            thisStack.Children.Add(otherStack);
            SimpleLabelGridXF thisLabel = new SimpleLabelGridXF();
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
            OurContainer!.RegisterNonSavedClasses<PokerViewModel>(); //go ahead and use the custom processes for this.
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
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .7f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }

        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 50;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 110;
                return 150;
            }
        }
    }
}