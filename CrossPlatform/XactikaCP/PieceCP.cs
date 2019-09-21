using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.MiscClasses;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace XactikaCP
{
    public class PieceCP : BaseGraphicsCP
    {
        private readonly SKPaint _redPaint;
        private readonly SKPaint _yellowPaint;
        private readonly SKPaint _borderPaint;
        private readonly SKPaint _whitePaint;
        public PieceCP()
        {
            OriginalSize = new SKSize(60, 138); // can be adjusted as needed
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            NeedsHighlighting = true;
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        private EnumShapes _ShapeUsed = EnumShapes.None;

        public EnumShapes ShapeUsed
        {
            get { return _ShapeUsed; }
            set
            {
                if (SetProperty(ref _ShapeUsed, value))
                {
                    //can decide what to do when property changes
                    if (PaintUI == null)
                        return;
                    PaintUI.DoInvalidate(); //i think
                }

            }
        }
        private int _HowMany;

        public int HowMany
        {
            get { return _HowMany; }
            set
            {
                if (SetProperty(ref _HowMany, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public float GetSuggestedHeight(float widthDesired)
        {
            var mults = widthDesired / 60;
            return 138 * mults;
        }
        protected override SKPaint GetFillPaint()
        {
            return _whitePaint;
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (ShapeUsed == (int)EnumShapes.None)
                return;
            dc.Clear(); // i think
            DrawSelector(dc); // i think
            var thisHeight = GetFontSize(40); // well see how this goes.
            var thisSize = new SKSize(thisHeight, thisHeight);
            var pointList = ImageHelpers.GetPoints(ShapeUsed, HowMany, Location, true, thisHeight); // can always be adjusted.   test on desktop first anyways.
            foreach (var thisPoint in pointList)
            {
                var thisRect = SKRect.Create(thisPoint, thisSize);
                if ((int)ShapeUsed == (int)EnumShapes.Balls)
                {
                    dc.DrawOval(thisRect, _redPaint);
                    dc.DrawOval(thisRect, _borderPaint);
                }
                else if ((int)ShapeUsed == (int)EnumShapes.Cones)
                {
                    ImageHelpers.DrawCone(dc, thisRect);
                }
                else if ((int)ShapeUsed == (int)EnumShapes.Cubes)
                {
                    ImageHelpers.DrawCube(dc, thisRect);
                }
                else if ((int)ShapeUsed == (int)EnumShapes.Stars)
                {
                    dc.DrawStar(thisRect, _yellowPaint, _borderPaint);
                }
            }
        }
    }
}