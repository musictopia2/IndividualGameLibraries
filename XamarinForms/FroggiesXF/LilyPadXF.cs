using FroggiesCP;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace FroggiesXF
{
    public class LilyPadXF : ContentView
    {
        private readonly SKCanvasView _thisDraw;
        public LilyPadCP? ThisLily;
        public FroggiesViewModel? ThisMod;
        public void Redraw() => _thisDraw.InvalidateSurface();
        public LilyPadXF()
        {
            _thisDraw = new SKCanvasView();
            _thisDraw.PaintSurface += PaintSurface;
            _thisDraw.Touch += Touch;
            _thisDraw.EnableTouchEvents = true;
            Content = _thisDraw;
        }
        private async void Touch(object sender, SKTouchEventArgs e)
        {
            if (ThisMod == null)
                return;
            if (ThisMod.LilyClickedCommand!.CanExecute(ThisLily!) == false)
                return;
            await Task.Delay(10);
            ThisMod.LilyClickedCommand.Execute(ThisLily!);
        }
        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_thisDraw == null)
                return;
            ThisLily!.DrawImage(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}