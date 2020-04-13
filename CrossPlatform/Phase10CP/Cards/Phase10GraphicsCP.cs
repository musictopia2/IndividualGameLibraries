using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.Cards;
using Phase10CP.Data;
using SkiaSharp;
namespace Phase10CP.Cards
{
    public class Phase10GraphicsCP : BaseColorCardsCP
    {

        private EnumCardCategory _cardCategory;

        public EnumCardCategory CardCategory
        {
            get { return _cardCategory; }
            set
            {
                if (SetProperty(ref _cardCategory, value))
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
