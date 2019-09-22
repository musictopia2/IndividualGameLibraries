using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CribbagePatienceCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CribbagePatienceXF
{
    public class GamePage : SinglePlayerGamePage<CribbagePatienceViewModel>
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
        private BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>? _deckGPile;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            _deckGPile = new BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> startCard = new BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            startCard.Init(ThisMod!.StartPile!, ts.TagUsed);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> yourHand = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> cribHand = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            var CribBut = GetGamingButton("To Crib", nameof(CribbagePatienceViewModel.CribCommand));
            var ContinueBut = GetGamingButton("Continue", nameof(CribbagePatienceViewModel.ContinueCommand));
            cribHand.Margin = new Thickness(5, 5, 5, 5);
            ScoreHandCribUI Hand1Score = new ScoreHandCribUI();
            ScoreHandCribUI Hand2Score = new ScoreHandCribUI();
            ScoreHandCribUI CribScore = new ScoreHandCribUI();
            ScoreSummaryUI Score1 = new ScoreSummaryUI();
            Score1.Margin = new Thickness(5, 5, 5, 5);
            Hand1Score.Margin = new Thickness(5, 5, 5, 5);
            Hand2Score.Margin = new Thickness(5, 5, 5, 5);
            CribScore.Margin = new Thickness(5, 5, 5, 5);
            Grid ThisGrid = new Grid();
            AddLeftOverRow(ThisGrid, 40);
            AddLeftOverRow(ThisGrid, 40);
            AddAutoRows(ThisGrid, 1);
            AddAutoColumns(ThisGrid, 1);
            AddLeftOverColumn(ThisGrid, 40);
            AddLeftOverColumn(ThisGrid, 40);
            AddControlToGrid(ThisGrid, thisStack, 2, 0);
            Grid.SetColumnSpan(thisStack, 3); // i think
            thisStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(CribBut);
            thisStack.Children.Add(ContinueBut);
            StackLayout OtherStack = new StackLayout();
            OtherStack.Orientation = StackOrientation.Horizontal;
            thisStack = new StackLayout();
            OtherStack.Children.Add(_deckGPile);
            OtherStack.Children.Add(startCard);
            thisStack.Children.Add(OtherStack);
            thisStack.Children.Add(yourHand);
            AddControlToGrid(ThisGrid, thisStack, 1, 0);
            AddControlToGrid(ThisGrid, cribHand, 0, 0);
            AddControlToGrid(ThisGrid, Score1, 0, 2);
            var (Row, Column) = ThisMod.GetRowColumn(EnumHandCategory.Hand1);
            var Hand2D = ThisMod.GetRowColumn(EnumHandCategory.Hand2);
            var CribD = ThisMod.GetRowColumn(EnumHandCategory.Crib);
            AddControlToGrid(ThisGrid, Hand1Score, Row, Column);
            AddControlToGrid(ThisGrid, Hand2Score, Hand2D.Row, Hand2D.Column);
            AddControlToGrid(ThisGrid, CribScore, CribD.Row, CribD.Column);
            Content = ThisGrid;
            await ThisMod.StartNewGameAsync();
            var Hand1CP = ThisMod.GetScoreHand(EnumHandCategory.Hand1); // has to be here.  because if autoresume, its a different one.  that will break the link.
            var Hand2CP = ThisMod.GetScoreHand(EnumHandCategory.Hand2);
            var CribCP = ThisMod.GetScoreHand(EnumHandCategory.Crib);
            _deckGPile.Init(ThisMod.DeckPile!, ts.TagUsed); // has to be here because could be a broken link problem
            Hand1Score.Init(Hand1CP);
            Hand2Score.Init(Hand2CP);
            CribScore.Init(CribCP);
            Score1.Init(ThisMod);
            cribHand.LoadList(ThisMod.TempCrib!, ts.TagUsed); // i think
            yourHand.LoadList(ThisMod.Hand1!, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<CribbagePatienceViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<CribbageCard>>(true); //i think
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
                    return .55f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.1f;
                return 1.5f;
            }
        }
    }
}