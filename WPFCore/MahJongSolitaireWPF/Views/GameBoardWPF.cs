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
using MahJongSolitaireCP.Logic;
using CommonBasicStandardLibraries.Messenging;
using MahJongSolitaireCP.EventModels;
using BaseMahjongTilesCP;
using System.Windows;
using MahJongSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;
using System.Reflection;
//i think this is the most common things i like to do
namespace MahJongSolitaireWPF.Views
{
    public class GameBoardWPF : UserControl, IHandle<MahJongSolitaireMainGameClass>, IHandle<TileChosenEventModel>
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
        private PlainCommand? _command;

        public void Handle(MahJongSolitaireMainGameClass message)
        {
            Visibility = Visibility.Visible;
            if (_command == null)
            {
                var mod = (MahJongSolitaireMainViewModel)DataContext; //hopefully that works.
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
                        MahJongSolitaireTilesWPF graphicsCard = new MahJongSolitaireTilesWPF();
                        graphicsCard.SendSize("", thisCard);
                        graphicsCard.Margin = new Thickness(0, 0, 0, 0);
                        graphicsCard.Command = _command; //try this way.  hopefully that works.
                        graphicsCard.CommandParameter = thisCard; // i think
                        //taking risks.
                        _thisGrid.Children.Add(graphicsCard);
                    }
                    else
                    {
                        var newItem = GetControl(thisCard);
                        newItem!.DataContext = null;
                        newItem.DataContext = thisCard;
                        newItem.CommandParameter = thisCard; // i think
                    }
                });
            });
        }

        //not sure if we needed the undo event now.


        //public void Handle(UndoEventModel message)
        //{
        //    foreach (var firstBoard in _thisMod.MainGame!.SaveRoot!.BoardList)
        //    {
        //        foreach (var firstCard in firstBoard.TileList)
        //            GetControl(firstCard);// hopefully this simple.
        //    }
        //    var thisList = message.PreviousList;
        //    CustomBasicList<MahJongSolitaireTilesWPF> nextList = new CustomBasicList<MahJongSolitaireTilesWPF>();
        //    MahJongSolitaireTilesWPF graphicsCard;
        //    foreach (var thisCard in thisList!)
        //    {
        //        var newItem = GetControl(thisCard);
        //        if (newItem != null)
        //        {
        //            _thisGrid.Children.Remove(newItem);
        //        }
        //        graphicsCard = new MahJongSolitaireTilesWPF();
        //        graphicsCard.SendSize("", thisCard);
        //        graphicsCard.Margin = new Thickness(0, 0, 0, 0);
        //        Binding ThisBind = new Binding(nameof(MahJongSolitaireViewModel.TileSelectedCommand));
        //        ThisBind.Source = _thisMod;
        //        graphicsCard.SetBinding(MahJongSolitaireTilesWPF.CommandProperty, ThisBind);
        //        graphicsCard.CommandParameter = thisCard; // i think
        //        nextList.Add(graphicsCard);
        //    }
        //    if (nextList.Count == 0)
        //        return;// there was no new items
        //    foreach (var ThisGR in nextList)
        //        _thisGrid.Children.Add(ThisGR);
        //    return;
        //}
        //public void Handle(StartNewGameEventModel message)
        //{
        //    _thisGrid.Children.Clear();
        //    _newOne.Visibility = Visibility.Collapsed;
        //    _thisGrid.Children.Add(_newOne);
        //}
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
        //private readonly MahJongSolitaireViewModel _thisMod;
        private readonly MahJongSolitaireTilesWPF _newOne;
        public GameBoardWPF(IEventAggregator aggregator)
        {
            //_thisMod = cons.Resolve<MahJongSolitaireViewModel>();
            aggregator.Subscribe(this);
            _thisGrid = new Grid();
            Content = _thisGrid;
            _newOne = new MahJongSolitaireTilesWPF();
            _newOne.Visibility = Visibility.Collapsed;
            _newOne.DataContext = null;
            _thisGrid.Children.Add(_newOne);
        }
    }
}
