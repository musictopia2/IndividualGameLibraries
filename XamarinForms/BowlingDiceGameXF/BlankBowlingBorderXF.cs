using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using static BowlingDiceGameCP.Logic.BowlingGlobalDrawing;
namespace BowlingDiceGameXF
{
    public class BlankBowlingBorderXF : ContentView
    {
        private readonly SKCanvasView _thisDraw;
        public BlankBowlingBorderXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += ThisDraw_PaintSurface;
            Content = _thisDraw;
        }
        private void ThisDraw_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            DrawFrame(e.Surface.Canvas, e.Info.Width, e.Info.Height, 2); // i think its this simple.
        }
    }
}