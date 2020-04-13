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
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using CribbagePatienceCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using CribbagePatienceCP.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;

namespace CribbagePatienceWPF.Views
{
    public class CribbagePatienceMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _deckGPile;


        private readonly ScoreHandCribUI _hand1Score;
        private readonly ScoreHandCribUI _hand2Score;
        private readonly ScoreHandCribUI _cribScore;
        private readonly ScoreSummaryUI _score1;
        private readonly BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _yourHand;
        private readonly BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _cribHand;
        private readonly BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>> _startCard;
        private readonly Grid _grid;
        private void SetMargins(UserControl element)
        {
            element.Margin = new Thickness(5);
        }

        public CribbagePatienceMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();

            StackPanel stack = new StackPanel();

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _hand1Score = new ScoreHandCribUI();
            _hand2Score = new ScoreHandCribUI();
            _cribScore = new ScoreHandCribUI();
            _score1 = new ScoreSummaryUI();
            _yourHand = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _cribHand = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            SetMargins(_score1);
            SetMargins(_hand1Score);
            SetMargins(_hand2Score);
            SetMargins(_cribScore);
            SetMargins(_cribHand);
            SetMargins(_yourHand); //i think should be this too.
            Grid grid = new Grid();
            AddLeftOverRow(grid, 40);
            AddLeftOverRow(grid, 60);
            AddAutoRows(grid, 1);
            AddAutoColumns(grid, 1);
            AddLeftOverColumn(grid, 40);
            AddLeftOverColumn(grid, 40);
            AddControlToGrid(grid, stack, 2, 0);
            Grid.SetColumnSpan(stack, 3);
            stack.Orientation = Orientation.Horizontal;
            var cributton = GetGamingButton("To Crib", nameof(CribbagePatienceMainViewModel.CribAsync));
            var continueButton = GetGamingButton("Continue", nameof(CribbagePatienceMainViewModel.Continue));
            stack.Children.Add(cributton);
            stack.Children.Add(continueButton);
            StackPanel otherStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            stack = new StackPanel();
            otherStack.Children.Add(_deckGPile);
            _startCard = new BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            otherStack.Children.Add(_startCard);
            stack.Children.Add(otherStack);
            stack.Children.Add(_yourHand);
            AddControlToGrid(grid, stack, 1, 0);
            AddControlToGrid(grid, _cribHand, 0, 0);
            AddControlToGrid(grid, _score1, 0, 2);
            _grid = grid;

            Content = grid;


        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            CribbagePatienceMainViewModel model = (CribbagePatienceMainViewModel)DataContext;
            var hand1 = model.GetScoreHand(EnumHandCategory.Hand1);
            var hand2 = model.GetScoreHand(EnumHandCategory.Hand2);
            var crib = model.GetScoreHand(EnumHandCategory.Crib);
            _hand1Score.Init(hand1);
            _hand2Score.Init(hand2);
            _cribScore.Init(crib);
            _cribHand.LoadList(model.TempCrib, ts.TagUsed);
            _yourHand.LoadList(model.Hand1, ts.TagUsed);
            _score1.Init(model);
            return Task.CompletedTask;
        }

        

        Task IUIView.TryActivateAsync()
        {
            CribbagePatienceMainViewModel model = (CribbagePatienceMainViewModel)DataContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            if (model.StartPile == null)
            {
                throw new BasicBlankException("Start card was never set.  Rethink");
            }
            _startCard.Init(model.StartPile, ts.TagUsed);

            var (row, column) = model.GetRowColumn(EnumHandCategory.Hand1);
            AddControlToGrid(_grid, _hand1Score, row, column);
            var hand2D = model.GetRowColumn(EnumHandCategory.Hand2);
            AddControlToGrid(_grid, _hand2Score, hand2D.row, hand2D.column);
            var cribD = model.GetRowColumn(EnumHandCategory.Crib);
            AddControlToGrid(_grid, _cribScore, cribD.row, cribD.column);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
