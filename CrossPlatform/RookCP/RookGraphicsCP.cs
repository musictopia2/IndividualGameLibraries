using BasicGameFramework.GameGraphicsCP.Cards;
using SkiaSharp;
namespace RookCP
{
    public class RookGraphicsCP : BaseColorCardsCP
    {
        protected override SKColor BackColor => SKColors.Aqua;

        protected override SKColor BackFontColor => SKColors.DarkOrange;

        protected override string BackText => "Rook";

        public override bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true;
            return Value != ""; //i think.
        }

        public override void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            DrawValueCard(canvas, rect_Card, Value); //hopefully this simple.
        }
    }
}