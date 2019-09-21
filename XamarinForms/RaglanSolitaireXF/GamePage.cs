using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
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
using RaglanSolitaireCP;
using SolitaireGraphicsXF;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace RaglanSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<RaglanSolitaireViewModel>
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
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            thisStack.Children.Add(otherStack);
            var autoBut = GetSmallerButton("Auto Make Move", nameof(RaglanSolitaireViewModel.AutoMoveCommand));
            MakeGameButtonSmaller(autoBut);
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(RaglanSolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesXF();
            var thisStock = new BaseHandXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            thisStock.Margin = new Thickness(100, 5, 5, 5);
            otherStack.Children.Add(thisMain);
            otherStack.Children.Add(thisStock);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(thisWaste);
            finalStack.Children.Add(tempGrid);
            finalStack.Children.Add(autoBut);
            finalStack.Children.Add(GameButton);
            Content = thisStack;
            await ThisMod!.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            thisWaste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            thisStock.LoadList(ThisMod.Stock1!, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RaglanSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
                    return .7f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }
    }
}