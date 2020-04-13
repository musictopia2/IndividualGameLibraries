using BasicControlsAndWindowsCore.Helpers;
using CommonBasicStandardLibraries.Messenging;
using MinesweeperCP.Data;
using MinesweeperCP.Logic;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks;

namespace MinesweeperWPF.Views
{
    public class GameboardWPF : UserControl
    {
        private readonly Grid _thisGrid;

        //private readonly SKElement _thisDraw;
        private readonly MinesweeperMainGameClass _gameBoard;

        public GameboardWPF(MinesweeperMainGameClass gameBoard)
        {
            _thisGrid = new Grid();
            //_thisDraw = new SKElement();
            //_thisDraw.PaintSurface += PaintSurface;
            //_thisGrid.Children.Add(_thisDraw);
            //Grid.SetColumnSpan(_thisDraw, 9);
            //Grid.SetRowSpan(_thisDraw, 9);
            // needs to be 9 by 9
            int x;
            for (x = 1; x <= 9; x++)
            {
                GridHelper.AddLeftOverColumn(_thisGrid, 1);
                GridHelper.AddLeftOverRow(_thisGrid, 1); // to make them even.
            }
            Content = _thisGrid;
            _gameBoard = gameBoard;
        }

        private readonly CustomBasicList<MineSquareWPF> _mines = new CustomBasicList<MineSquareWPF>();
        public void Init()
        {
            var thisList = _gameBoard.GetSquares();
            _thisGrid.Children.Clear();
            GamePackageViewModelBinder.ManuelElements.Clear();
            foreach (var thisSquare in thisList)
            {
                MineSquareWPF thisGraphics = new MineSquareWPF(thisSquare);
                _mines.Add(thisGraphics);
                
                GamePackageViewModelBinder.ManuelElements.Add(thisGraphics);

                GridHelper.AddControlToGrid(_thisGrid, thisGraphics, thisSquare.Row - 1, thisSquare.Column - 1);
            }
            //probably after all is done recheck the bindings.
        }

        //bad news is it does not matter what i do, same issue.

        public async Task StartUpAsync()
        {
            await _mines.ForEachAsync(async x =>
            {
                await Task.CompletedTask;
                //await Task.Delay(20);
                x.StartUp();
            });
        }

        //private void PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        //{
        //    _gameBoard.DrawBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        //}

        //may not be needed anymore because the moment you go in, can show the proper information.
        //public void Handle(SubscribeGameBoardEventModel message)
        //{
        //    var thisList = _gameBoard.GetSquares();
        //    _thisGrid.Children.Clear(); //looks like i have to clear and redo.
        //    //_thisGrid.Children.Add(_thisDraw); //try this (?)
        //    foreach (var thisSquare in thisList)
        //    {
        //        MineSquareWPF thisGraphics = new MineSquareWPF(thisSquare);

        //        //looks like a serious problem.
        //        //because since you have to remove the list first, then commands could get hosed.
        //        //can take a risk.  however, if i run into a memory problem, will require rethinking.
        //        //this is the first game where it actually has to remove and repopulate.


        //        GridHelper.AddControlToGrid(_thisGrid, thisGraphics, thisSquare.Row - 1, thisSquare.Column - 1);
        //    }
        //    //probably after all is done recheck the bindings.
        //    _main.RefreshBindingsAsync(_aggregator);
        //}
    }
}
