using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using XactikaCP.Data;
using XactikaCP.MiscImages;
namespace XactikaCP.Cards
{
    public class XactikaGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }


        private int _howManyBalls;
        public int HowManyBalls
        {
            get
            {
                return _howManyBalls;
            }

            set
            {
                if (SetProperty(ref _howManyBalls, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _howManyCubes;
        public int HowManyCubes
        {
            get
            {
                return _howManyCubes;
            }

            set
            {
                if (SetProperty(ref _howManyCubes, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _howManyCones;
        public int HowManyCones
        {
            get
            {
                return _howManyCones;
            }

            set
            {
                if (SetProperty(ref _howManyCones, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _howManyStars;
        public int HowManyStars
        {
            get
            {
                return _howManyStars;
            }

            set
            {
                if (SetProperty(ref _howManyStars, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (SetProperty(ref _value, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }


        private bool _drew;

        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
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
            canvas.DrawBorderText("X", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder!, tempRect);
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
