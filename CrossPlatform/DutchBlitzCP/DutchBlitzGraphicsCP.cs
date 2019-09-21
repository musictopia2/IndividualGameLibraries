using BasicGameFramework.ColorCards;
using BasicGameFramework.GameGraphicsCP.Cards;
using SkiaSharp;
namespace DutchBlitzCP
{
    public class DutchBlitzGraphicsCP : BaseColorCardsCP
    {
        protected override SKColor BackColor => SKColors.Aqua;
        protected override SKColor BackFontColor => SKColors.DarkGreen;
        protected override string BackText => "Dutch Blitz";
        public override bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true;
            return (Value != "" && Color != EnumColorTypes.None);
        }
        public override void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            DrawValueCard(canvas, rect_Card, Value); //hopefully this simple.
        }
    }
}