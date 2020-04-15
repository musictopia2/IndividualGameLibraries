using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using MinesweeperCP.Data;
using MinesweeperCP.ViewModels;
using SkiaSharp.Views.Forms;
namespace MinesweeperXF.Views
{
    public class MineSquareXF : GraphicsCommand
    {
        private readonly MineSquareCP _thisSquare;
        public MineSquareXF(MineSquareCP thisSquare)
        {
            _thisSquare = thisSquare;
            CommandParameter = thisSquare;
            this.SetName(nameof(MinesweeperMainViewModel.MakeMoveAsync));
        }

        public void StartUp()
        {
            _thisSquare.NeedsToRedrawEvent += Repaint;
            ThisDraw.PaintSurface += PaintSurface;

        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();
            _thisSquare.DrawSquare(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
        public void Repaint()
        {
            ThisDraw.InvalidateSurface();
        }
    }
}