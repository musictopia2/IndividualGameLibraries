using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace CandylandCP.GraphicsCP
{
    public class CandylandPieceGraphicsCP : BaseGraphicsCP
    {
        private readonly SKPaint _borderBrush;
        public CandylandPieceGraphicsCP()
        {
            NeedsToClear = true;
            _borderBrush = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (NeedsToClear == true)
                dc.Clear();
            var bounds = GetMainRect();
            SKPath gp = new SKPath();
            gp.AddOval(SKRect.Create(bounds.Left, bounds.Top + bounds.Height * 4 / 5, bounds.Width, bounds.Height / 5));
            dc.DrawPath(gp, MainPaint);
            dc.DrawPath(gp, _borderBrush);
            gp = new SKPath();
            gp.AddLine(new SKPoint(bounds.Left + bounds.Width / 4, bounds.Top + bounds.Height * 9 / 10), new SKPoint(bounds.Left + bounds.Width * 3 / 8, bounds.Top + bounds.Height * 5 / 10), true);
            gp.ArcTo(SKRect.Create(bounds.Left, bounds.Top + bounds.Height * 3 / 8, bounds.Width / 4, bounds.Height * 2 / 16), 90, 180, false);
            gp.ArcTo(SKRect.Create(bounds.Left + bounds.Width / 4, bounds.Top, bounds.Width / 2, bounds.Height * 3 / 8), 110, 320, false); // was 320
            gp.ArcTo(SKRect.Create(bounds.Left + bounds.Width * 3 / 4, bounds.Top + bounds.Height * 3 / 8, bounds.Width / 4, bounds.Height * 2 / 16), -90, 180, false);
            gp.AddLine(new SKPoint(bounds.Left + bounds.Width * 5 / 8, bounds.Top + bounds.Height * 5 / 10), new SKPoint(bounds.Left + bounds.Width * 3 / 4, bounds.Top + bounds.Height * 9 / 10));
            gp.AddLine(new SKPoint(bounds.Left + bounds.Width * 9 / 16, bounds.Top + bounds.Height * 9 / 10), new SKPoint(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height * 7 / 10));
            gp.AddLine(new SKPoint(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height * 7 / 10), new SKPoint(bounds.Left + bounds.Width * 7 / 16, bounds.Top + bounds.Height * 9 / 10));
            gp.Close();
            dc.DrawPath(gp, MainPaint);
            dc.DrawPath(gp, _borderBrush);
        }
    }
}
