using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Dice;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace FillOrBustCP
{
    public class FillOrBustGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private bool _Drew;

        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }
        private EnumCardStatusList _Category;

        public EnumCardStatusList Category
        {
            get { return _Category; }
            set
            {
                if (SetProperty(ref _Category, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }

        private int _Value;

        public int Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }
        public bool NeedsToDrawBacks => true;

        public bool CanStartDrawing()
        {
            return Category != EnumCardStatusList.Unknown;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (Value > 0 & Category != EnumCardStatusList.None)
                throw new BasicBlankException("Since the value is greater than 0; then the card status must be none");
            if (Value == 0 & Category == EnumCardStatusList.None)
                return;
            if (Value != 0 & Value != 300 & Value != 400 & Value != 500 & Value != 1000 & Value != 2500)
                throw new BasicBlankException("The value must be 0, 300, 400, 500, 1000, 2500; not " + Value);
            if (Value == 2500)
            {
                DrawRevenge(canvas);
                return;
            }
            if (Value == 1000)
            {
                DrawFill(canvas);
                return;
            }
            if (Category == EnumCardStatusList.MustBust)
            {
                DrawMustBust(canvas, rect_Card);
                return;
            }
            if (Category == EnumCardStatusList.DoubleTrouble)
            {
                DrawDoubleTrouble(canvas);
                return;
            }
            if (Category == EnumCardStatusList.NoDice)
            {
                DrawNoDice(canvas);
                return;
            }
            if (Value > 0)
            {
                DrawBonus(canvas);
                return;
            }
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Red;
            if (Value == 2500)
                return SKColors.Maroon;
            if (Category == EnumCardStatusList.MustBust)
                return SKColors.Red;
            if (Value > 0)
                return SKColors.Aqua;
            if (Category == EnumCardStatusList.DoubleTrouble)
                return SKColors.Aqua;
            return SKColors.Maroon;
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _redFill;
        private SKPaint? _maroonFill;
        private SKPaint? _aquaPen;
        private SKPaint? _borderTextPaint;
        private Assembly? _thisAssembly;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(107, 135);
            _redFill = MiscHelpers.GetSolidPaint(SKColors.Red);
            _aquaPen = MiscHelpers.GetStrokePaint(SKColors.Aqua, 5);
            _maroonFill = MiscHelpers.GetSolidPaint(SKColors.Maroon);
            _borderTextPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _thisAssembly = Assembly.GetAssembly(this.GetType());
        }
        #region "Unique Graphics"
        private void DrawBonus(SKCanvas canvas) // okay now
        {
            var firstRect = MainGraphics!.GetActualRectangle(7, 7, 95, 25);
            var secondRect = MainGraphics.GetActualRectangle(7, 100, 95, 25);
            canvas.DrawRect(firstRect, _redFill);
            canvas.DrawRect(secondRect, _redFill);
            var firstText = MainGraphics.GetActualRectangle(0, 37, 107, 35);
            var secondTExt = MainGraphics.GetActualRectangle(0, 72, 107, 35);
            var thisPaint = MiscHelpers.GetTextPaint(SKColors.Maroon, MainGraphics.GetFontSize(35));
            canvas.DrawBorderText("Bonus", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, thisPaint, _borderTextPaint, firstText);
            canvas.DrawBorderText(Value.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, thisPaint, _borderTextPaint, secondTExt);
        }
        private void DrawFill(SKCanvas canvas) // okay now.
        {
            SKRect firstText;
            firstText = MainGraphics!.GetActualRectangle(0, 30, 107, 25);
            var firstPaint = MiscHelpers.GetTextPaint(SKColors.Red, MainGraphics.GetFontSize(40));
            var secondPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, MainGraphics.GetFontSize(35));
            var TempRect = MainGraphics.GetActualRectangle(15, 65, 77, 35);
            canvas.DrawRect(TempRect, _redFill);
            canvas.DrawBorderText("Fill", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, firstPaint, _borderTextPaint, firstText);
            canvas.DrawBorderText("1000", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, secondPaint, _borderTextPaint, TempRect);
        }
        private void DrawDoubleTrouble(SKCanvas canvas)
        {
            SKRect firstRect;
            SKRect lastRect;
            SKRect firstText;
            SKRect middleText;
            SKRect lastText;
            firstRect = MainGraphics!.GetActualRectangle(5, 5, 97, 20);
            lastRect = MainGraphics.GetActualRectangle(5, 109, 97, 20);
            var firstPaint = MiscHelpers.GetTextPaint(SKColors.LightPink, MainGraphics.GetFontSize(25));
            var middlePaint = MiscHelpers.GetTextPaint(SKColors.Maroon, MainGraphics.GetFontSize(25));
            firstText = MainGraphics.GetActualRectangle(0, 26, 107, 30);
            middleText = MainGraphics.GetActualRectangle(0, 56, 107, 30);
            lastText = MainGraphics.GetActualRectangle(0, 86, 107, 30);
            canvas.DrawRect(firstRect, _redFill);
            canvas.DrawRect(lastRect, _redFill);
            canvas.DrawBorderText("Double", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, firstPaint, _borderTextPaint, firstText);
            canvas.DrawBorderText("Trouble", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, middlePaint, _borderTextPaint, middleText);
            canvas.DrawBorderText("Double", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, firstPaint, _borderTextPaint, lastText);
        }
        private void DrawMustBust(SKCanvas canvas, SKRect thisRect)
        {
            SKRect firstRect;
            SKRect lastRect;
            firstRect = MainGraphics!.GetActualRectangle(7, 10, 93, 25);
            lastRect = MainGraphics.GetActualRectangle(7, 100, 93, 25);
            canvas.DrawRect(firstRect, _maroonFill);
            canvas.DrawRect(firstRect, _aquaPen);
            canvas.DrawRect(lastRect, _maroonFill);
            canvas.DrawRect(lastRect, _aquaPen);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, MainGraphics.GetFontSize(30));
            var textRect = MainGraphics.GetActualRectangle(0, 43, 107, 37);
            canvas.DrawBorderText("Must", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, _borderTextPaint, textRect);
            textRect = MainGraphics.GetActualRectangle(SKRect.Create(thisRect.Left, 72, 107, 37));
            canvas.DrawBorderText("Bust!", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, _borderTextPaint, textRect);
        }
        private void DrawRevenge(SKCanvas canvas)
        {
            SKRect rect_Piece;
            SKRect tempRect;
            tempRect = MainGraphics!.GetActualRectangle(4, 5, 109, 27);
            var ThisPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, MainGraphics.GetFontSize(19));
            ThisPaint.FakeBoldText = true;
            canvas.DrawCustomText("Vengeance", TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, ThisPaint, tempRect, out _);
            tempRect = MainGraphics.GetActualRectangle(-1, 27, 109, 27);
            canvas.DrawCustomText("2500", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, ThisPaint, tempRect, out _);
            rect_Piece = MainGraphics.GetActualRectangle(6, 59, 95, 80);
            canvas.DrawRect(rect_Piece, _redFill);
            var ThisImage = ImageExtensions.GetSkBitmap(_thisAssembly, "revenge.png");
            canvas.DrawBitmap(ThisImage, rect_Piece, MainGraphics.BitPaint);
        }
        private void DrawNoDice(SKCanvas canvas)
        {
            var textRect = MainGraphics!.GetActualRectangle(0, 10, 107, 44);
            var thisPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, MainGraphics.GetFontSize(27));
            canvas.DrawBorderText("No Dice", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, thisPaint, _borderTextPaint, textRect);
            var lastRect = MainGraphics.GetActualRectangle(23, 59, 60, 60);
            var thisPen = MiscHelpers.GetStrokePaint(SKColors.Red, 7); // i think 7 no matter what (?)
            canvas.DrawOval(lastRect, thisPen);
            StandardDiceGraphicsCP thisDice = new StandardDiceGraphicsCP();
            thisDice.Location = new SKPoint(lastRect.Left + (lastRect.Size.Width / 4), lastRect.Top + (lastRect.Size.Height / 4));
            thisDice.NeedsToClear = false; // its this instead.
            var thisSize = MainGraphics.GetActualSize(new SKSize(34, 34));
            thisDice.ActualWidthHeight = thisSize.Height;
            thisDice.UseSmallerBorders();
            thisDice.Value = 6;
            thisDice.DrawDice(canvas);
            canvas.DrawLine(lastRect.Left + 10, lastRect.Top + 10, lastRect.Right - 10, lastRect.Bottom - 10, thisPen);
        }
        #endregion
    }
}