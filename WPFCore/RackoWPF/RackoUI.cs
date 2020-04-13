using BasicControlsAndWindowsCore.Helpers;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using RackoCP.Cards;
using RackoCP.Data;
using RackoCP.ViewModels;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace RackoWPF
{
    public class RackoUI : BaseFrameWPF
    {
        private DeckObservableDict<RackoCardInformation>? _cardList;
        private StackPanel? _thisStack;
        private ICommand? _thisCommand;
        private Grid? FindControl(RackoCardInformation thisCard)
        {
            foreach (var thisCon in _thisStack!.Children)
            {
                var thisGrid = (Grid)thisCon!;
                if (thisGrid.DataContext.Equals(thisCard) == true)
                    return thisGrid;
            }
            return null;
        }
        public void Dispose()
        {
            if (_cardList == null)
            {
                return;
            }
            _cardList.CollectionChanged -= CardList_CollectionChanged;
        }
        public void Init(RackoMainViewModel viewModel, RackoVMData model, RackoGameContainer gameContainer)
        {
            gameContainer.SingleInfo = gameContainer!.PlayerList!.GetSelf();
            _cardList = gameContainer.SingleInfo.MainHandList;
            _thisCommand = viewModel.GetBasicGameCommand(nameof(RackoMainViewModel.PlayOnPileAsync)); //needs the view model for the command.
            Grid mainGrid = new Grid();
            IsEnabled = false;
            _thisStack = new StackPanel();
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
        private void PopulateControls(RackoGameContainer gameContainer)
        {
            _thisStack!.Children.Clear();
            int starts = gameContainer.PlayerList.Count() + 2;
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
                thisGrid.DataContext = thisCard; // i think
                GridHelper.AddPixelColumn(thisGrid, 100);
                GridHelper.AddAutoColumns(thisGrid, 1);
                var thisLabel = SharedUIFunctions.GetDefaultLabel();
                thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
                thisLabel.VerticalAlignment = VerticalAlignment.Center;
                thisLabel.Text = otherList[tempList.IndexOf(thisCard)].ToString();
                thisLabel.FontSize = 40;
                thisLabel.Margin = new Thickness(0, 3, 0, 0);
                GridHelper.AddControlToGrid(thisGrid, thisLabel, 0, 0);
                CardGraphicsWPF graphics = new CardGraphicsWPF();
                graphics.SendSize("", thisCard); //hopefully this simple.
                GridHelper.AddControlToGrid(thisGrid, graphics, 0, 1);
                RowClickerWPF custom = new RowClickerWPF();
                custom.Command = _thisCommand!;
                custom.CommandParameter = thisCard;
                //custom.Name = nameof(RackoMainViewModel.PlayOnPileAsync);
                //custom.Command = mainGame.ThisMod!.PlayOnPileCommand;
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
                CardGraphicsWPF nextControl = (CardGraphicsWPF)oldGrid!.Children[1];
                nextControl.DataContext = null;
                nextControl.DataContext = e.NewItems[0];
                //nextControl.CommandParameter = e.NewItems[0]!;
                var row = (RowClickerWPF)oldGrid!.Children[2];
                row.CommandParameter = e.NewItems[0]!;
                oldGrid.DataContext = e.NewItems[0]; // i think i forgot the 0
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
                    thisGrid.DataContext = tempList[x];
                    CardGraphicsWPF nextControl = (CardGraphicsWPF)thisGrid.Children[1];
                    nextControl.DataContext = null;
                    //nextControl.CommandParameter = tempList[x];
                    nextControl.DataContext = tempList[x];
                    var row = (RowClickerWPF)thisGrid!.Children[2];
                    row.CommandParameter = e.NewItems[0]!;
                    x += 1;
                }
            }
        }
        private void ThisCommand_CanExecuteChanged(object? sender, EventArgs e)
        {
            IsEnabled = _thisCommand!.CanExecute(null); //was forced to do this way now.
        }
        //public void Update(RackoMainGameClass mainGame)
        //{
        //    mainGame.SingleInfo = mainGame.PlayerList!.GetSelf();
        //    _cardList!.CollectionChanged -= CardList_CollectionChanged;
        //    _cardList = mainGame.SingleInfo.MainHandList;
        //    _cardList.CollectionChanged += CardList_CollectionChanged;
        //    PopulateControls(mainGame);
        //}
    }
}
