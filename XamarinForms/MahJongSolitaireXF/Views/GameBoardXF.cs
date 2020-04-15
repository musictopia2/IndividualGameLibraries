using BaseMahjongTilesCP;
using BasicGameFrameworkLibrary.CommandClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using MahJongSolitaireCP.EventModels;
using MahJongSolitaireCP.Logic;
using MahJongSolitaireCP.ViewModels;
using System;
using System.Reflection;
using Xamarin.Forms;
namespace MahJongSolitaireXF.Views
{
    public class GameBoardXF : ContentView, IHandle<MahJongSolitaireMainGameClass>, IHandle<TileChosenEventModel>
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
            return null;
        }


        private PlainCommand? _command;


        public void Handle(MahJongSolitaireMainGameClass message)
        {
            IsVisible = true;

            if (_command == null)
            {
                var mod = (MahJongSolitaireMainViewModel)BindingContext; //hopefully that works.
                Type type = mod.GetType();

                MethodInfo? method = type.GetMethod(nameof(MahJongSolitaireMainViewModel.SelectTileAsync));
                if (method == null)
                {
                    throw new BasicBlankException("The select tile was not found.  Rethink");
                }
                MethodInfo? fun = type.GetMethod(nameof(MahJongSolitaireMainViewModel.CanSelectTile));
                if (fun == null)
                {
                    throw new BasicBlankException("The canselect tile was not found.  Rethink");
                }
                _command = new PlainCommand(mod, method, fun, mod.CommandContainer);
            }

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
                        graphicsCard.Command = _command; //try this way.  hopefully that works.
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
                _newOne.IsVisible = false;
        }
        private readonly MahJongSolitaireTilesXF _newOne;
        public GameBoardXF(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _thisGrid = new Grid();
            Content = _thisGrid;
            _newOne = new MahJongSolitaireTilesXF();
            _newOne.IsVisible = false;
            _newOne.BindingContext = null;
            _thisGrid.Children.Add(_newOne);
        }
    }
}