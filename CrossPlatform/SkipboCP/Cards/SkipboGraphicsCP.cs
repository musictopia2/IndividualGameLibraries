using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.Cards;
using SkiaSharp;
namespace SkipboCP.Cards
{
    public class SkipboGraphicsCP : BaseColorCardsCP
    {
        protected override SKColor BackColor => SKColors.Red;

        protected override SKColor BackFontColor => SKColors.BlanchedAlmond;

        protected override string BackText => "Skipbo";

        public override bool CanStartDrawing()
        {
            return (Value != "" && Color != EnumColorTypes.None);
        }
        public override void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            DrawValueCard(canvas, rect_Card, Value); //hopefully this simple.
        }
    }
}
