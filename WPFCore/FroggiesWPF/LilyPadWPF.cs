using BasicGamingUIWPFLibrary.GameGraphics.Base;
using FroggiesCP.Data;
using FroggiesCP.ViewModels;
using SkiaSharp.Views.WPF;
namespace FroggiesWPF
{
    public class LilyPadWPF : GraphicsCommand
    {
        private readonly SKElement _thisDraw;
        private readonly LilyPadCP _lily;

        //public LilyPadCP? ThisLily;

        //public FroggiesViewModel? ThisMod;

        public void Redraw() => _thisDraw.InvalidateVisual();

        public LilyPadWPF(LilyPadCP lily)
        {
            _thisDraw = new SKElement();
            _thisDraw.PaintSurface += PaintSurface;
            CommandParameter = lily; //has to be this way now.
            //MouseUp += LilyPadWPF_MouseUp;
            Name = nameof(FroggiesMainViewModel.MakeMoveAsync);
            Content = _thisDraw;
            _lily = lily;
        }

        //private void LilyPadWPF_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{

        //    if (Command == null || ThisLily == null)
        //    {
        //        return;
        //    }
        //    Command.Execute(ThisLily);
        //}

        private void PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            if (_thisDraw == null)
                return;
            _lily.DrawImage(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }
    }
}
