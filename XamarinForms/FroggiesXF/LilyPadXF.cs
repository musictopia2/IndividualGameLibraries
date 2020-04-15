using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using FroggiesCP.Data;
using FroggiesCP.ViewModels;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace FroggiesXF
{
    public class LilyPadXF : GraphicsCommand
    {
        //private readonly SKCanvasView _thisDraw;
        private readonly LilyPadCP _lily;
        public void Redraw() => ThisDraw.InvalidateSurface();
        public LilyPadXF(LilyPadCP lily)
        {
            _lily = lily;
            this.SetName(nameof(FroggiesMainViewModel.MakeMoveAsync));
            CommandParameter = lily; //has to be this way now.
            ThisDraw.PaintSurface += PaintSurface;
        }
        
        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            if (ThisDraw == null)
                return;
            _lily.DrawImage(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}