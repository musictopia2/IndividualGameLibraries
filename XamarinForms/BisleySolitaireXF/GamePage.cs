using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using BisleySolitaireCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BisleySolitaireXF
{
    public class GamePage : SinglePlayerGamePage<BisleySolitaireViewModel>
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
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var thisMain = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            thisMain.Spacing = 0;
            thisMain.Margin = new Thickness(2, 5, 5, 5);
            thisStack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(BisleySolitaireViewModel.AutoMoveCommand));
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(BisleySolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            otherStack.Children.Add(GameButton);
            otherStack.Children.Add(autoBut);
            otherStack.Children.Add(tempGrid);
            thisStack.Children.Add(thisMain);
            Content = thisStack;
            var miscDiscard = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            miscDiscard.Spacing = 0;
            var tempCard = new DeckOfCardsXF<SolitaireCard>();
            tempCard.SendSize(ts.TagUsed, new SolitaireCard());
            miscDiscard.Margin = new Thickness(2, (tempCard.ObjectSize.Height * -1) - 15, 0, 0);
            thisStack.Children.Add(miscDiscard);
            await ThisMod!.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            var tempMain = (CustomMain)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            miscDiscard.Init(tempWaste.Discards!, ts.TagUsed); //hopefully this works too.
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<BisleySolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
            OurContainer.RegisterType<WastePiles>(); //can't do automatically because we don't know if we will do it or not.
            OurContainer.RegisterType<CustomMain>();
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .6f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.1f;
                return 1.5f; //experiment.  originally was 1.7
            }
        }
    }
}