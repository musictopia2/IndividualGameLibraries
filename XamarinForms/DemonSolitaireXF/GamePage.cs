using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using DemonSolitaireCP;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BasicGameFramework.BasicEventModels;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BasicGameFramework.CommonInterfaces;
using AndyCristinaGamePackageCP.ExtensionClasses;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using AndyCristinaGamePackageCP.DataClasses;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using SolitaireGraphicsXF;
using BasicGameFramework.BasicGameDataClasses;

namespace DemonSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<DemonSolitaireViewModel>
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
        private BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>? _deckGPile;
        private BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>? _discardGPile;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            var thisMain = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(DemonSolitaireViewModel.Score));
            scoresAlone.AddRow("Starting Number", nameof(DemonSolitaireViewModel.StartingNumber));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesXF();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            _discardGPile.Init(ThisMod.MainDiscardPile!, ts.TagUsed);
            var heel1 = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            tempGrid.WidthRequest = 75;
            heel1.HorizontalOptions = LayoutOptions.Start;
            heel1.VerticalOptions = LayoutOptions.Start;
            heel1.Init(ThisMod.Heel1!, ts.TagUsed);
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 3);
            AddAutoRows(thisGrid, 2);
            AddControlToGrid(thisGrid, _deckGPile, 0, 0);
            AddControlToGrid(thisGrid, _discardGPile, 0, 1);
            AddControlToGrid(thisGrid, thisMain, 0, 2);
            AddControlToGrid(thisGrid, heel1, 1, 1);
            AddControlToGrid(thisGrid, thisWaste, 1, 2);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(GameButton);
            AddControlToGrid(thisGrid, thisStack, 1, 0);
            heel1.Margin = new Thickness(0, 5, 0, 0);
            thisWaste.Margin = new Thickness(0, 25, 0, 0);
            thisStack.Margin = new Thickness(0, 5, 5, 0);
            Content = thisGrid;
            await ThisMod.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            thisWaste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<DemonSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
                    return .75f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }
    }
}