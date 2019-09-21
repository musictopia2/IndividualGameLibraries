using BasicGameFramework.ColorCards;
using BasicGameFramework.GameGraphicsCP.Cards;
using SkiaSharp;
namespace Phase10CP
{
    public class Phase10GraphicsCP : BaseColorCardsCP
    {

        private EnumCardCategory _CardCategory;

        public EnumCardCategory CardCategory
        {
            get { return _CardCategory; }
            set
            {
                if (SetProperty(ref _CardCategory, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }
        protected override SKColor BackColor => SKColors.Aqua;

        protected override SKColor BackFontColor => SKColors.Purple;

        protected override string BackText => "Phase 10";

        public override bool CanStartDrawing()
        {
            if (Color == EnumColorTypes.ZOther || Color == EnumColorTypes.None)
                return false; //this too.
            return CardCategory != EnumCardCategory.Blank;
        }
        public override void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            DrawValueCard(canvas, rect_Card, Value); //hopefully this simple.
        }
    }
}