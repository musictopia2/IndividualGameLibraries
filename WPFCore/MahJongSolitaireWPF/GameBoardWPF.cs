using BaseMahjongTilesCP;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using MahJongSolitaireCP;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MahJongSolitaireWPF
{
    public class GameBoardWPF : UserControl, IHandle<MahJongSolitaireMainGameClass>,
        IHandle<UndoEventModel>, IHandle<StartNewGameEventModel>, IHandle<TileChosenEventModel>
    {
        private readonly Grid _thisGrid;
        private MahJongSolitaireTilesWPF? GetControl(MahjongSolitaireTileInfo thisTile)
        {
            if (_thisGrid.Children.Count == 0)
                throw new BasicBlankException("Can't get control because grid had 0 children");
            foreach (var thisFirst in _thisGrid.Children)
            {
                var thisGraphics = (MahJongSolitaireTilesWPF)thisFirst!;
                var thisFin = (MahjongSolitaireTileInfo)thisGraphics.DataContext;
                if (thisGraphics != _newOne)
                {
                    if (thisFin.Deck == thisTile.Deck)
                    {
                        if (thisFin.Equals(thisTile) == false)
                        {
                            thisGraphics.DataContext = thisTile;
                            thisGraphics.CommandParameter = thisTile;
                        }
                        return thisGraphics;
                    }
                }

            }
            return null;
        }
        public void Handle(MahJongSolitaireMainGameClass message)
        {
            Visibility = System.Windows.Visibility.Visible;
            var tempList = message.GameBoard1!.GetPriorityBoards();
            bool isNew;
            if (_thisGrid.Children.Count > 1)
                isNew = false;
            else
                isNew = true;
            tempList.ForEach(thisBoard =>
            {
                thisBoard.TileList.ForEach(ThisCard =>
                {
                    if (isNew == true)
                    {
                        MahJongSolitaireTilesWPF graphicsCard = new MahJongSolitaireTilesWPF();
                        graphicsCard.SendSize("", ThisCard);
                        graphicsCard.Margin = new Thickness(0, 0, 0, 0);
                        Binding thisBind = new Binding(nameof(MahJongSolitaireViewModel.TileSelectedCommand));
                        thisBind.Source = _thisMod;
                        graphicsCard.SetBinding(MahJongSolitaireTilesWPF.CommandProperty, thisBind);
                        graphicsCard.CommandParameter = ThisCard; // i think
                        _thisGrid.Children.Add(graphicsCard);
                    }
                    else
                    {
                        var newItem = GetControl(ThisCard);
                        newItem!.DataContext = null;
                        newItem.DataContext = ThisCard;
                        newItem.CommandParameter = ThisCard; // i think
                    }
                });
            });
        }

        public void Handle(UndoEventModel message)
        {
            foreach (var firstBoard in _thisMod.MainGame!.SaveRoot!.BoardList)
            {
                foreach (var firstCard in firstBoard.TileList)
                    GetControl(firstCard);// hopefully this simple.
            }
            var thisList = message.PreviousList;
            CustomBasicList<MahJongSolitaireTilesWPF> nextList = new CustomBasicList<MahJongSolitaireTilesWPF>();
            MahJongSolitaireTilesWPF graphicsCard;
            foreach (var thisCard in thisList!)
            {
                var newItem = GetControl(thisCard);
                if (newItem != null)
                {
                    _thisGrid.Children.Remove(newItem);
                }
                graphicsCard = new MahJongSolitaireTilesWPF();
                graphicsCard.SendSize("", thisCard);
                graphicsCard.Margin = new Thickness(0, 0, 0, 0);
                Binding ThisBind = new Binding(nameof(MahJongSolitaireViewModel.TileSelectedCommand));
                ThisBind.Source = _thisMod;
                graphicsCard.SetBinding(MahJongSolitaireTilesWPF.CommandProperty, ThisBind);
                graphicsCard.CommandParameter = thisCard; // i think
                nextList.Add(graphicsCard);
            }
            if (nextList.Count == 0)
                return;// there was no new items
            foreach (var ThisGR in nextList)
                _thisGrid.Children.Add(ThisGR);
            return;
        }
        public void Handle(StartNewGameEventModel message)
        {
            _thisGrid.Children.Clear();
            _newOne.Visibility = Visibility.Collapsed;
            _thisGrid.Children.Add(_newOne);
        }
        public void Handle(TileChosenEventModel message)
        {
            if (message.Deck > 0)
            {
                MahjongSolitaireTileInfo thisTile = new MahjongSolitaireTileInfo();
                thisTile.Populate(message.Deck);
                thisTile.Top = 715;
                thisTile.Left = 1000;
                if (_newOne.DataContext == null)
                    _newOne.SendSize("", thisTile);
                else
                {
                    _newOne.DataContext = null;
                    _newOne.DataContext = thisTile; // to refresh just in case.
                }
                _newOne.Visibility = Visibility.Visible;
            }
            else
            {
                _newOne.Visibility = Visibility.Collapsed;
            }
        }
        private readonly MahJongSolitaireViewModel _thisMod;
        private readonly MahJongSolitaireTilesWPF _newOne;
        public GameBoardWPF()
        {
            EventAggregator thisE = cons.Resolve<EventAggregator>(); //because this can't use used from unit testing.
            _thisMod = cons.Resolve<MahJongSolitaireViewModel>();
            thisE.Subscribe(this);
            _thisGrid = new Grid();
            Content = _thisGrid;
            _newOne = new MahJongSolitaireTilesWPF();
            _newOne.Visibility = Visibility.Collapsed;
            _newOne.DataContext = null;
            _thisGrid.Children.Add(_newOne);
        }
    }
}