using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using static BowlingDiceGameCP.BowlingGlobalDrawing;
namespace BowlingDiceGameWPF
{
    public class BlankBowlingBorderWPF : UserControl
    {
        private readonly SKElement _thisDraw;
        public BlankBowlingBorderWPF()
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            DrawFrame(e.Surface.Canvas, e.Info.Width, e.Info.Height, 2); // i think its this simple.
        }
    }
}