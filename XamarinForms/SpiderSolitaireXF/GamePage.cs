using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using SolitaireGraphicsXF;
using SpiderSolitaireCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SpiderSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<SpiderSolitaireViewModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        protected override bool UseSmallerButton => true;
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>? _deckGPile;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            ListChooserXF level1 = new ListChooserXF();
            var thisMain = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            if (ScreenUsed != EnumScreen.SmallPhone)
                level1.ItemWidth /= 2;
            else
            {
                level1.ItemWidth /= 4;
                level1.ItemHeight = 20;
            }
            //if (ScreenUsed == EnumScreen.SmallPhone)
            //    level1.ItemWidth =-10;
            thisMain.Margin = new Thickness(0, 50, 5, 5);
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(SpiderSolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesXF();
            thisStack.Margin = new Thickness(10, 5, 0, 0);
            otherStack.Children.Add(thisWaste);
            thisStack.Children.Add(level1);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(_deckGPile);
            otherStack.Children.Add(thisStack);
            thisStack = new StackLayout();
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
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .5f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.0f;
                return 1.4f;
            }
        }
    }
}