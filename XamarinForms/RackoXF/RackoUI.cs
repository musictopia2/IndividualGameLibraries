using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using RackoCP.Cards;
using RackoCP.ViewModels;
using RackoCP.Data;

namespace RackoXF
{
    public class RackoUI : BaseFrameXF
    {
        private DeckObservableDict<RackoCardInformation>? _cardList;
        private StackLayout? _thisStack;
        private ICommand? _thisCommand;
        private Grid? FindControl(RackoCardInformation thisCard)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var thisGrid = (Grid)thisCon!;
                if (thisGrid.BindingContext.Equals(thisCard) == true)
                    return thisGrid;
            }
            return null;
        }
        public void Init(RackoMainViewModel viewModel, RackoVMData model, RackoGameContainer gameContainer)
        {
            gameContainer.SingleInfo = gameContainer!.PlayerList!.GetSelf();
            _cardList = gameContainer.SingleInfo.MainHandList;
            _thisCommand = viewModel.GetBasicGameCommand(nameof(RackoMainViewModel.PlayOnPileAsync));
            Grid mainGrid = new Grid();
            _thisStack = new StackLayout();
            Text = "Your Card List";
            var thisRect = ThisFrame.GetControlArea();
            _thisStack.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            if (gameContainer.SingleInfo.MainHandList.Count != 10)
                throw new BasicBlankException("Must have 10 cards before i can init.  Rethink now.");
            _thisCommand!.CanExecuteChanged += ThisCommand_CanExecuteChanged;
            _cardList.CollectionChanged += CardList_CollectionChanged;
            PopulateControls(gameContainer);
            mainGrid.Children.Add(ThisDraw);
            mainGrid.Children.Add(_thisStack);
            Content = mainGrid;
        }
        public void Dispose()
        {
            if (_cardList == null)
            {
                return;
            }
            _thisCommand!.CanExecuteChanged -= ThisCommand_CanExecuteChanged;
            _cardList!.CollectionChanged -= CardList_CollectionChanged;
        }
        private void PopulateControls(RackoGameContainer mainGame)
        {
            _thisStack!.Children.Clear();
            int starts = mainGame.PlayerList.Count() + 2;
            int diffs = starts;
            var tempList = _cardList!.ToRegularDeckDict();
            tempList.Reverse();
            int x;
            CustomBasicList<int> otherList = new CustomBasicList<int>();
            for (x = 1; x <= 10; x++)
            {
                otherList.Add(starts);
                starts += diffs;
            }
            otherList.Reverse();
            foreach (var thisCard in tempList)
            {
                Grid thisGrid = new Grid();
                thisGrid.BindingContext = thisCard; // i think
                GridHelper.AddPixelColumn(thisGrid, 100);
                GridHelper.AddAutoColumns(thisGrid, 1);
                var thisLabel = GetDefaultLabel();
                thisLabel.HorizontalOptions = LayoutOptions.Center;
                thisLabel.VerticalOptions = LayoutOptions.Center;
                thisLabel.Text = otherList[tempList.IndexOf(thisCard)].ToString();
                thisLabel.FontSize = 40;
                thisLabel.Margin = new Thickness(0, 3, 0, 0);
                GridHelper.AddControlToGrid(thisGrid, thisLabel, 0, 0);
                CardGraphicsXF ThisGraphics = new CardGraphicsXF();
                ThisGraphics.SendSize("", thisCard); //hopefully this simple.
                GridHelper.AddControlToGrid(thisGrid, ThisGraphics, 0, 1);
                RowClickerXF custom = new RowClickerXF();
                custom.Command = _thisCommand!;
                custom.CommandParameter = thisCard;
                GridHelper.AddControlToGrid(thisGrid, custom, 0, 0);
                Grid.SetColumnSpan(custom, 2); // so it spans the entire control.
                _thisStack.Children.Add(thisGrid);
            }
        }
        private void CardList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems.Count > 1)
                    throw new BasicBlankException("Not sure when there are more than one to replace");
                var oldCard = (RackoCardInformation)e.OldItems[0]!;
                var oldGrid = FindControl(oldCard);
                CardGraphicsXF nextControl = (CardGraphicsXF)oldGrid!.Children[1];
                nextControl.BindingContext = null;
                nextControl.BindingContext = e.NewItems[0];
                oldGrid.BindingContext = e.NewItems[0]; // i think i forgot the 0
                var row = (RowClickerXF)oldGrid!.Children[2];
                row.CommandParameter = e.NewItems[0]!;
                oldGrid.BindingContext = e.NewItems[0]; // i think i forgot the 0
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (_cardList!.Count == 0)
                    return; //because there could be more eventually.
                var tempList = _cardList.ToRegularDeckDict();
                tempList.Reverse();
                if (_thisStack!.Children.Count != 10)
                    throw new BasicBlankException("Not Supported");
                int x = 0;
                foreach (var thisCon in _thisStack.Children)
                {
                    var thisGrid = (Grid)thisCon!;
                    thisGrid.BindingContext = tempList[x];
                    CardGraphicsXF nextControl = (CardGraphicsXF)thisGrid.Children[1];
                    nextControl.BindingContext = null;
                    nextControl.BindingContext = tempList[x];
                    var row = (RowClickerXF)thisGrid!.Children[2];
                    row.CommandParameter = e.NewItems[0]!;
                    x += 1;
                }
            }
        }
        private void ThisCommand_CanExecuteChanged(object? sender, EventArgs e)
        {
            IsEnabled = _thisCommand!.CanExecute(null); //was forced to do this way now.
        }
        
    }
}
