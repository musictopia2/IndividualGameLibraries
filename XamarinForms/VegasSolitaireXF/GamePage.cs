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
using VegasSolitaireCP;
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
using BasicXFControlsAndPages.Converters;
using BasicGameFramework.BasicGameDataClasses;

namespace VegasSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<VegasSolitaireViewModel>
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
            Grid thisGrid = new Grid();
            StackLayout thisStack = new StackLayout();
            AddAutoColumns(thisGrid, 2);
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var thisMain = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            otherStack.Children.Add(thisMain);
            thisStack.Children.Add(otherStack);
            var thisWaste = new SolitairePilesXF();
            thisStack.Children.Add(thisWaste);
            thisGrid.Children.Add(thisStack);
            thisStack = new StackLayout();
            thisStack.Margin = new Thickness(20, 5, 5, 5);
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            var scoresAlone = new SimpleLabelGridXF();
            CurrencyConverter curs = new CurrencyConverter();
            scoresAlone.AddRow("Score", nameof(VegasSolitaireViewModel.Money), curs);
            var tempGrid = scoresAlone.GetContent;
            var autoBut = GetSmallerButton("Auto Make Move", nameof(VegasSolitaireViewModel.AutoMoveCommand));
            MakeGameButtonSmaller(autoBut);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(autoBut);
            thisStack.Children.Add(GameButton);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.Init(ThisMod!.DeckPile!, ts.TagUsed);
            _discardGPile.Init(ThisMod.MainDiscardPile!, ts.TagUsed);
            Content = thisGrid;
            await ThisMod.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            thisWaste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain!.Piles, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<VegasSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
                    return .65f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.3f;
                return 1.7f;
            }
        }
    }
}