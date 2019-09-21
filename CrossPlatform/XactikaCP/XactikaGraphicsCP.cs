using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace XactikaCP
{
    public class XactikaGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }


        private int _HowManyBalls;
        public int HowManyBalls
        {
            get
            {
                return _HowManyBalls;
            }

            set
            {
                if (SetProperty(ref _HowManyBalls, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _HowManyCubes;
        public int HowManyCubes
        {
            get
            {
                return _HowManyCubes;
            }

            set
            {
                if (SetProperty(ref _HowManyCubes, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _HowManyCones;
        public int HowManyCones
        {
            get
            {
                return _HowManyCones;
            }

            set
            {
                if (SetProperty(ref _HowManyCones, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _HowManyStars;
        public int HowManyStars
        {
            get
            {
                return _HowManyStars;
            }

            set
            {
                if (SetProperty(ref _HowManyStars, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (SetProperty(ref _Value, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }


        private bool _Drew;

        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown)
                return true;
            if (Value == 0)
                return true;
            return HowManyBalls > 0 || HowManyCones > 0 || HowManyCubes > 0 || HowManyStars > 0;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            var tempRect = SKRect.Create(rect_Card.Location.X + 3, rect_Card.Location.Y + 3, rect_Card.Width - 6, rect_Card.Height - 6);
            canvas.DrawRect(tempRect, _redPaint);
            var fontSize = tempRect.Height * 1.2f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, fontSize);
            canvas.DrawBorderText("X", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, tempRect);
        }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _blackBorder;
        private SKPaint? _redPaint;
        private SKPaint? _yellowPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(80, 100); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
        }
        private void DrawValue(SKCanvas thisCanvas, SKRect thisRect)
        {
            var fontSize = thisRect.Height * 1.1f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, fontSize);
            var pen = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            thisCanvas.DrawBorderText(Value.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, pen, thisRect); // value is already given
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            var firstHeight = MainGraphics!.GetFontSize(40);
            var firstWidth = MainGraphics.GetFontSize(16);
            var firstRect = SKRect.Create(MainGraphics.Location.X, MainGraphics.Location.Y, rect_Card.Width, firstHeight);
            DrawValue(canvas, firstRect);
            var otherLocation = new SKPoint(MainGraphics.Location.X, firstRect.Bottom);
            var pointList = ImageHelpers.GetPoints(EnumShapes.Balls, HowManyBalls, otherLocation, false, firstWidth);
            var testSize = new SKSize(firstWidth, firstWidth);
            foreach (var thisPoint in pointList)
            {
                var testRect = SKRect.Create(thisPoint, testSize);
                canvas.DrawOval(testRect, _redPaint);
                canvas.DrawOval(testRect, _blackBorder);
            }
            pointList = ImageHelpers.GetPoints(EnumShapes.Cones, HowManyCones, otherLocation, false, firstWidth);
            foreach (var thisPoint in pointList)
            {
                var testRect = SKRect.Create(thisPoint, testSize);
                ImageHelpers.DrawCone(canvas, testRect);
            }
            pointList = ImageHelpers.GetPoints(EnumShapes.Cubes, HowManyCubes, otherLocation, false, firstWidth);
            foreach (var thisPoint in pointList)
            {
                var testRect = SKRect.Create(thisPoint, testSize);
                ImageHelpers.DrawCube(canvas, testRect);
            }
            pointList = ImageHelpers.GetPoints(EnumShapes.Stars, HowManyStars, otherLocation, false, firstWidth);
            foreach (var thisPoint in pointList)
            {
                var testRect = SKRect.Create(thisPoint, testSize);

                canvas.DrawStar(testRect, _yellowPaint!, _blackBorder!);
            }
        }
    }
}