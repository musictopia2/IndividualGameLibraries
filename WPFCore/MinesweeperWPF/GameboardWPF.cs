using BasicControlsAndWindowsCore.Helpers;
using CommonBasicStandardLibraries.Messenging;
using MinesweeperCP;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
namespace MinesweeperWPF
{
    public class GameboardWPF : UserControl, IHandle<SubscribeGameBoardEventModel>
    {
        private readonly Grid _thisGrid;
        private readonly MinesweeperViewModel _thisMod;
        private readonly SKElement _thisDraw;
        public GameboardWPF(MinesweeperViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisGrid = new Grid();
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += PaintSurface;
            _thisGrid.Children.Add(_thisDraw);
            Grid.SetColumnSpan(_thisDraw, 9);
            Grid.SetRowSpan(_thisDraw, 9);
            // needs to be 9 by 9
            int x;
            for (x = 1; x <= 9; x++)
            {
                GridHelper.AddLeftOverColumn(_thisGrid, 1);
                GridHelper.AddLeftOverRow(_thisGrid, 1); // to make them even.
            }
            EventAggregator thisE = _thisMod.MainContainer!.Resolve<EventAggregator>();
            thisE.Subscribe(this);
            Content = _thisGrid;
        }
        private void PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            _thisMod.GameBoard1!.DrawBoard(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Handle(SubscribeGameBoardEventModel message)
        {
            var thisList = _thisMod.GameBoard1!.GetSquares();
            _thisGrid.Children.Clear(); //looks like i have to clear and redo.
            foreach (var thisSquare in thisList)
            {
                MineSquareWPF ThisGraphics = new MineSquareWPF(thisSquare, _thisMod);
                GridHelper.AddControlToGrid(_thisGrid, ThisGraphics, thisSquare.Row - 1, thisSquare.Column - 1);
            }
        }
    }
}