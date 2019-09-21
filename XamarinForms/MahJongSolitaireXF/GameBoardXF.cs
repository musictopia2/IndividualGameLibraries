using BaseMahjongTilesCP;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using MahJongSolitaireCP;
using Microsoft.Extensions.Logging.Abstractions;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MahJongSolitaireXF
{
    public class GameBoardXF : ContentView, IHandle<MahJongSolitaireMainGameClass>,
        IHandle<UndoEventModel>, IHandle<StartNewGameEventModel>, IHandle<TileChosenEventModel>
    {
        private readonly Grid _thisGrid;
        private MahJongSolitaireTilesXF? GetControl(MahjongSolitaireTileInfo thisTile)
        {
            if (_thisGrid.Children.Count == 0)
                throw new BasicBlankException("Can't get control because grid had 0 children");
            foreach (var thisFirst in _thisGrid.Children)
            {
                var thisGraphics = (MahJongSolitaireTilesXF)thisFirst!;
                var thisFin = (MahjongSolitaireTileInfo)thisGraphics.BindingContext;
                if (thisGraphics != _newOne)
                {
                    if (thisFin.Deck == thisTile.Deck)
                    {
                        if (thisFin.Equals(thisTile) == false)
                        {
                            thisGraphics.BindingContext = thisTile;
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
            IsVisible = true;
            var tempList = message.GameBoard1!.GetPriorityBoards();
            bool isNew;
            if (_thisGrid.Children.Count > 1)
                isNew = false;
            else
                isNew = true;
            tempList.ForEach(thisBoard =>
            {
                thisBoard.TileList.ForEach(thisCard =>
                {
                    if (isNew == true)
                    {
                        MahJongSolitaireTilesXF graphicsCard = new MahJongSolitaireTilesXF();
                        graphicsCard.SendSize("", thisCard);
                        Binding thisBind = new Binding(nameof(MahJongSolitaireViewModel.TileSelectedCommand));
                        thisBind.Source = _thisMod;
                        graphicsCard.SetBinding(MahJongSolitaireTilesXF.CommandProperty, thisBind);
                        graphicsCard.CommandParameter = thisCard; // i think
                        _thisGrid.Children.Add(graphicsCard);
                    }
                    else
                    {
                        var newItem = GetControl(thisCard);
                        newItem!.BindingContext = null;
                        newItem.BindingContext = thisCard;
                        newItem.CommandParameter = thisCard; // i think
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
            CustomBasicList<MahJongSolitaireTilesXF> nextList = new CustomBasicList<MahJongSolitaireTilesXF>();
            MahJongSolitaireTilesXF graphicsCard;
            //CustomBasicList<MahJongSolitaireTilesXF> newList = new CustomBasicList<MahJongSolitaireTilesXF>();
            foreach (var thisCard in thisList!)
            {
                var newItem = GetControl(thisCard);
                if (newItem == null)
                {
                    graphicsCard = new MahJongSolitaireTilesXF();
                    graphicsCard.SendSize("", thisCard);
                    graphicsCard.Margin = new Thickness(0, 0, 0, 0);
                    Binding ThisBind = new Binding(nameof(MahJongSolitaireViewModel.TileSelectedCommand));
                    ThisBind.Source = _thisMod;
                    graphicsCard.SetBinding(MahJongSolitaireTilesXF.CommandProperty, ThisBind);
                    graphicsCard.CommandParameter = thisCard; // i think
                    nextList.Add(graphicsCard);
                }
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
            _newOne.IsVisible = false;
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
                thisTile.Visible = false; //for now, unable to show what you selected.  if i need this for tablet, rethink.
                if (_newOne.BindingContext == null)
                    _newOne.SendSize("", thisTile);
                else
                {
                    _newOne.BindingContext = null;
                    _newOne.BindingContext = thisTile; // to refresh just in case.
                }
                _newOne.IsVisible = true;
                _newOne.IsVisible = false; //because does not work right for positioning.
            }
            else
            {
                _newOne.IsVisible = false;
            }
        }
        //maybe something else is for hint (?)
        private readonly MahJongSolitaireViewModel _thisMod;
        private readonly MahJongSolitaireTilesXF _newOne;
        public GameBoardXF()
        {
            EventAggregator thisE = cons.Resolve<EventAggregator>(); //because this can't use used from unit testing.
            _thisMod = cons.Resolve<MahJongSolitaireViewModel>();
            thisE.Subscribe(this);
            _thisGrid = new Grid();
            Content = _thisGrid;
            _newOne = new MahJongSolitaireTilesXF();
            _newOne.IsVisible = false;
            _newOne.BindingContext = null;
            _thisGrid.Children.Add(_newOne);
        }
    }
}