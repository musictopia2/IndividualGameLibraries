using BasicGameFramework.GameGraphicsCP.Cards;
using SkiaSharp;
namespace FlinchCP
{
    public class FlinchGraphicsCP : BaseColorCardsCP
    {
        protected override SKColor BackColor => SKColors.Blue;
        protected override SKColor BackFontColor => SKColors.Red;
        protected override string BackText => "Flinch";
        public override SKColor GetFillColor()
        {
            return SKColors.Blue; //just doing it this way since its not working otherwise.
        }
        public override bool CanStartDrawing()
        {
            return (Value != "");
        }
        public override void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            DrawValueCard(canvas, rect_Card, Value); //hopefully this simple.
        }
    }
}