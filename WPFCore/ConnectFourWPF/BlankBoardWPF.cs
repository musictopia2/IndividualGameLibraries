using ConnectFourCP;
using SkiaSharp.Views.WPF;
using System.Windows;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ConnectFourWPF
{
    public class BlankBoardWPF : UserControl
    {
        private readonly Grid _thisGrid;
        private readonly SKElement _thisDraw;
        private readonly ConnectFourGraphicsCP _cPBoard;
        public void Init()
        {
            _thisDraw.InvalidateVisual();
        }
        public BlankBoardWPF(GameBoardWPF board)
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += DrawPaint;
            _thisGrid = new Grid();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            _cPBoard = Resolve<ConnectFourGraphicsCP>();
            _thisGrid.Children.Add(_thisDraw);
            _thisGrid.Children.Add(board);
            Content = _thisGrid;
        }

        private void DrawPaint(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            _cPBoard.DrawBorders(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}
