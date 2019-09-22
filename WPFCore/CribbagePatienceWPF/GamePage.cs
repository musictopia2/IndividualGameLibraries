using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using CribbagePatienceCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CribbagePatienceWPF
{
    public class GamePage : SinglePlayerWindow<CribbagePatienceViewModel>
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
        private BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>? _deckGPile;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            _deckGPile = new BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> startCard = new BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            startCard.Init(ThisMod!.StartPile!, ts.TagUsed);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> yourHand = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> cribHand = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
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
            thisStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(CribBut);
            thisStack.Children.Add(ContinueBut);
            StackPanel OtherStack = new StackPanel();
            OtherStack.Orientation = Orientation.Horizontal;
            thisStack = new StackPanel();
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
            OurContainer!.RegisterNonSavedClasses<CribbagePatienceViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<CribbageCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 2.0f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}