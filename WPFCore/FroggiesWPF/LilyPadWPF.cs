using FroggiesCP;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
namespace FroggiesWPF
{
    public class LilyPadWPF : UserControl
    {
        private readonly SKElement _thisDraw;
        public LilyPadCP? ThisLily;

        public FroggiesViewModel? ThisMod;

        public void Redraw() => _thisDraw.InvalidateVisual();

        public LilyPadWPF()
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += PaintSurface;
            MouseUp += LilyPadWPF_MouseUp;
            Content = _thisDraw;
        }

        private void LilyPadWPF_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ThisMod == null)
                return;
            if (ThisMod.LilyClickedCommand!.CanExecute(ThisLily!) == false)
                return;
            ThisMod.LilyClickedCommand.Execute(ThisLily!);
        }

        private void PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            if (_thisDraw == null)
                return;
            ThisLily!.DrawImage(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}