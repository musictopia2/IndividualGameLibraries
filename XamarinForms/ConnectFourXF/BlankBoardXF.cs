using ConnectFourCP;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ConnectFourXF
{
    public class BlankBoardXF : ContentView
    {
        private readonly Grid _thisGrid;
        private readonly SKCanvasView _thisDraw;
        private readonly ConnectFourGraphicsCP _cPBoard;
        public void Init()
        {
            _thisDraw.InvalidateSurface();
        }
        public BlankBoardXF(GameBoardXF board)
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += DrawPaint;
            _thisGrid = new Grid();
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            _cPBoard = Resolve<ConnectFourGraphicsCP>();
            _thisGrid.Children.Add(_thisDraw);
            _thisGrid.Children.Add(board);
            Content = _thisGrid;
        }
        private void DrawPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            _cPBoard.DrawBorders(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}