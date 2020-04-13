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
using BlackjackCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using BlackjackCP.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGamingUIWPFLibrary.Helpers;

namespace BlackjackWPF.Views
{
    public class BlackjackMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> _deckGPile;
        private readonly BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> _humanHand;
        private readonly BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> _computerHand;
        public BlackjackMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>>();

            StackPanel stack = new StackPanel();
            stack.Children.Add(_deckGPile);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;


            Grid grid = new Grid();
            AddAutoColumns(grid, 1);
            AddLeftOverColumn(grid, 1); // i think
            SimpleLabelGrid otherGrid = new SimpleLabelGrid();
            otherGrid.AddRow("Wins", nameof(BlackjackMainViewModel.Wins));
            otherGrid.AddRow("Losses", nameof(BlackjackMainViewModel.Losses));
            otherGrid.AddRow("Draws", nameof(BlackjackMainViewModel.Draws));
            stack.Children.Add(otherGrid.GetContent);
            grid.Children.Add(stack);

            stack = new StackPanel();
            BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> humanHand = new BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>>();
            BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>> computerHand = new BaseHandWPF<BlackjackCardInfo, ts, DeckOfCardsWPF<BlackjackCardInfo>>();
            humanHand.HandType = HandObservable<BlackjackCardInfo>.EnumHandList.Horizontal;
            computerHand.HandType = HandObservable<BlackjackCardInfo>.EnumHandList.Horizontal;
            humanHand.Margin = new Thickness(3, 3, 3, 10);
            computerHand.Margin = new Thickness(3, 3, 3, 10);
            _humanHand = humanHand;
            _computerHand = computerHand;
            stack.Children.Add(computerHand);
            stack.Children.Add(humanHand);
            AddControlToGrid(grid, stack, 0, 1);
            otherGrid = new SimpleLabelGrid();
            //GamePackageViewModelBinder.ManuelElements.Clear();
            otherGrid.AddRow("Human Points", nameof(BlackjackMainViewModel.HumanPoints));
            otherGrid.AddRow("Computer Points", nameof(BlackjackMainViewModel.ComputerPoints));
            stack.Children.Add(otherGrid.GetContent);
            var thisBut = GetGamingButton("Hit Me", nameof(BlackjackMainViewModel.HitAsync)); // hopefully margins set automatically (well find out)
            StackPanel newStack = new StackPanel();
            //GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            newStack.Orientation = Orientation.Horizontal;
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Stay", nameof(BlackjackMainViewModel.StayAsync));
            //GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            finalStack.Children.Add(thisBut);
            newStack.Children.Add(finalStack);
            finalStack = new StackPanel();
            finalStack.Margin = new Thickness(50, 0, 0, 0);
            thisBut = GetGamingButton("Use Ace As One (1)", nameof(BlackjackMainViewModel.AceAsync));
            thisBut.CommandParameter = BlackjackMainViewModel.EnumAceChoice.Low;
            var thisBind = GetVisibleBinding(nameof(BlackjackMainViewModel.NeedsAceChoice));
            finalStack.SetBinding(VisibilityProperty, thisBind); // since its going to this stack, will make all based on this one.
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Use Ace As Eleven (11)", nameof(BlackjackMainViewModel.AceAsync));
            thisBut.CommandParameter = BlackjackMainViewModel.EnumAceChoice.High;
            finalStack.Children.Add(thisBut);
            newStack.Children.Add(finalStack);
            stack.Children.Add(newStack);
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            BlackjackMainViewModel model = (BlackjackMainViewModel)DataContext;
            _humanHand.LoadList(model.HumanStack!, ts.TagUsed);
            _computerHand.LoadList(model.ComputerStack!, ts.TagUsed);
            return Task.CompletedTask;
            //return this.RefreshBindingsAsync(_aggregator);
        }

        

        Task IUIView.TryActivateAsync()
        {
            BlackjackMainViewModel model = (BlackjackMainViewModel)DataContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //maybe i have to unsubscribe too.
            return Task.CompletedTask;
        }
    }
}
