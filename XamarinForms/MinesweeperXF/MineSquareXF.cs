using MinesweeperCP;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
namespace MinesweeperXF
{
    public class MineSquareXF : ContentView
    {
        private readonly MineSquareCP _thisSquare;
        private readonly SKCanvasView _thisDraw;
        private readonly MinesweeperViewModel _thisMod;
        public MineSquareXF(MineSquareCP thisSquare, MinesweeperViewModel thisMod)
        {
            _thisDraw = new SKCanvasView();
            _thisSquare = thisSquare;
            _thisSquare.NeedsToRedrawEvent += Repaint;
            _thisDraw.PaintSurface += PaintSurface;
            _thisDraw.EnableTouchEvents = true;
            _thisDraw.Touch += Touch;
            _thisMod = thisMod;
            Content = _thisDraw;
        }
        private void Touch(object sender, SKTouchEventArgs e)
        {
            if (_thisMod.SquareClickCommand!.CanExecute(_thisSquare) == false)
                return;
            _thisMod.SquareClickCommand.Execute(_thisSquare);
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();
            _thisSquare.DrawSquare(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Repaint()
        {
            _thisDraw.InvalidateSurface();
        }
    }
}