using MinesweeperCP;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
namespace MinesweeperWPF
{
    public class MineSquareWPF : UserControl
    {

        private readonly MineSquareCP _thisSquare;
        private readonly SKElement _thisDraw;
        private readonly MinesweeperViewModel _thisMod;

        public MineSquareWPF(MineSquareCP thisSquare, MinesweeperViewModel thisMod)
        {
            _thisDraw = new SKElement();
            _thisSquare = thisSquare;
            _thisSquare.NeedsToRedrawEvent += Repaint;
            _thisDraw.PaintSurface += PaintSurface;
            _thisMod = thisMod;
            MouseUp += MineSquareWPF_MouseUp;
            Content = _thisDraw;
        }

        private void MineSquareWPF_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_thisMod.SquareClickCommand!.CanExecute(_thisSquare) == false)
                return;
            _thisMod.SquareClickCommand.Execute(_thisSquare);
        }

        private void PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();
            _thisSquare.DrawSquare(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void Repaint()
        {
            _thisDraw.InvalidateVisual();
        }
    }
}
