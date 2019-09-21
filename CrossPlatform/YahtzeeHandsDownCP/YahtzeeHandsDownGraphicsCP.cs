using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.GameGraphicsCP.Dice;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
namespace YahtzeeHandsDownCP
{
    public class YahtzeeHandsDownGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private EnumColor _Color = EnumColor.None;
        public EnumColor Color
        {
            get
            {
                return _Color;
            }

            set
            {
                if (SetProperty(ref _Color, value) == true)
                {
                    if (MainGraphics!.PaintUI != null)
                        MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private int _FirstValue;
        public int FirstValue
        {
            get
            {
                return _FirstValue;
            }

            set
            {
                if (SetProperty(ref _FirstValue, value) == true)
                {
                    if (MainGraphics!.PaintUI != null)
                        MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private int _SecondValue;
        public int SecondValue
        {
            get
            {
                return _SecondValue;
            }

            set
            {
                if (SetProperty(ref _SecondValue, value) == true)
                {
                    if (MainGraphics!.PaintUI != null)
                        MainGraphics.PaintUI.DoInvalidate();
                }
            }
        }
        private bool _IsWild;
        public bool IsWild
        {
            get
            {
                return _IsWild;
            }

            set
            {
                if (SetProperty(ref _IsWild, value) == true)
                {
                    if (MainGraphics!.PaintUI != null)
                        MainGraphics.PaintUI.DoInvalidate();
                }
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
                    MainGraphics!.PaintUI!.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown)
                return true;
            return Color != EnumColor.None;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint ThisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, ThisPaint);
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

            var firstRect = SKRect.Create(tempRect.Location.X, tempRect.Location.Y, tempRect.Width, tempRect.Height / 3);
            var secondRect = SKRect.Create(tempRect.Location.X, firstRect.Bottom, tempRect.Width, firstRect.Height);
            var thirdRect = SKRect.Create(tempRect.Location.X, secondRect.Bottom, tempRect.Width, secondRect.Height);
            var fontSize = firstRect.Height * 0.6f; // can experiment
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            canvas.DrawCustomText("Yahtzee", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            canvas.DrawCustomText("Hands", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
            canvas.DrawCustomText("Down", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thirdRect, out _);
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
        private SKPaint? _redPaint;
        private SKPaint? _bluePaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _whitePaint;
        private SKPaint? _thickBorder;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            SKColor thisColor = SKColors.Black;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 70); //can experiment as needed.
            _selectPaint = MiscHelpers.GetSolidPaint(otherColor);
            thisColor = SKColors.White;
            otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 150);
            _pDrewPaint = MiscHelpers.GetSolidPaint(otherColor);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 3);
        }
        private SKRect GetCenterDiceRect()
        {
            var firstRect = MainGraphics!.GetActualRectangle(5, 12, 45, 45);
            float width;
            float diffs = 0;
            if (firstRect.Height > firstRect.Width)
            {
                width = firstRect.Width;
                diffs = firstRect.Height - firstRect.Width;
            }
            else if (firstRect.Width > firstRect.Width)
            {
                width = firstRect.Height;
            }
            else
            {
                width = firstRect.Width;
            }
            return SKRect.Create(MainGraphics.Location.X + firstRect.Location.X, MainGraphics.Location.Y + firstRect.Location.Y + (diffs / 2), width, width);
        }
        private void DrawAllColors(SKCanvas thisCanvas, SKRect rect_Card)
        {
            var tempRect = GetStartingRect(rect_Card);
            var firstRect = SKRect.Create(tempRect.Location.X, tempRect.Location.Y, tempRect.Width / 3, tempRect.Height);
            var secondRect = SKRect.Create(firstRect.Right, tempRect.Location.Y, firstRect.Width, tempRect.Height);
            var thirdRect = SKRect.Create(secondRect.Right, tempRect.Location.Y, secondRect.Width, tempRect.Height);
            thisCanvas.DrawRect(firstRect, _redPaint);
            thisCanvas.DrawRect(secondRect, _bluePaint);
            thisCanvas.DrawRect(thirdRect, _yellowPaint);
            var FinalRect = GetCenterDiceRect();
            PrivateDrawDiceImage(thisCanvas, FirstValue, FinalRect);
        }
        private SKRect GetStartingRect(SKRect rect_Card)
        {
            var fontSize = MainGraphics!.GetFontSize(3);
            var tempRect = SKRect.Create(rect_Card.Location.X + fontSize, rect_Card.Location.Y + fontSize, rect_Card.Width - (fontSize * 2), rect_Card.Height - (fontSize * 2));
            return tempRect;
        }
        private void PrivateDrawDiceImage(SKCanvas thisCanvas, int value, SKRect thisRect)
        {
            StandardDiceGraphicsCP thisDice = new StandardDiceGraphicsCP();
            thisDice.NeedsToClear = false; // for this for sure
            thisDice.Location = thisRect.Location;
            thisDice.UseSmallerBorders();
            thisDice.ActualWidthHeight = thisRect.Height;
            thisDice.Value = value;
            thisDice.DrawDice(thisCanvas);
        }
        private SKPaint? GetColorUsed()
        {
            if (Color == EnumColor.Blue)
                return _bluePaint;
            if (Color == EnumColor.Red)
                return _redPaint;
            if (Color == EnumColor.Yellow)
                return _yellowPaint;
            throw new Exception("Color Not Found");
        }
        private void DrawWild(SKCanvas thisCanvas, SKRect rect_Card)
        {
            var tempRect = GetStartingRect(rect_Card);
            thisCanvas.DrawRect(tempRect, GetColorUsed()); // i think
            var otherRect = GetCenterDiceRect();
            var tempSize = MainGraphics!.GetFontSize(3);
            thisCanvas.DrawRoundRect(otherRect, tempSize, tempSize, _whitePaint);
            thisCanvas.DrawRoundRect(otherRect, tempSize, tempSize, _thickBorder);
            var fontSize = otherRect.Height * 0.38f; // can adjust as needed
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            thisCanvas.DrawCustomText("WILD", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, otherRect, out _);
        }
        private void Draw2Numbers(SKCanvas thisCanvas, SKRect rect_Card)
        {
            var tempRect = GetStartingRect(rect_Card);
            thisCanvas.DrawRect(tempRect, GetColorUsed());
            tempRect = MainGraphics!.GetActualRectangle(11, 5, 30, 30);
            float width;
            float diffs = 0;
            if (tempRect.Height > tempRect.Width)
            {
                width = tempRect.Width;
                diffs = tempRect.Height - tempRect.Width;
            }
            else if (tempRect.Width > tempRect.Height)
            {
                width = tempRect.Height;
            }
            else
            {
                width = tempRect.Width;
            }
            var firstRect = SKRect.Create(MainGraphics.Location.X + tempRect.Location.X, MainGraphics.Location.Y + tempRect.Location.Y + (diffs / 2), width, width);
            tempRect = MainGraphics.GetActualRectangle(11, 37, 30, 30);
            var secondRect = SKRect.Create(MainGraphics.Location.X + tempRect.Location.X, MainGraphics.Location.Y + tempRect.Location.Y, width, width);
            PrivateDrawDiceImage(thisCanvas, FirstValue, firstRect);
            PrivateDrawDiceImage(thisCanvas, SecondValue, secondRect);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (Color == EnumColor.Any)
            {
                DrawAllColors(canvas, rect_Card);
                return;
            }
            if (IsWild == true)
            {
                DrawWild(canvas, rect_Card);
                return;
            }
            if (SecondValue > 0)
            {
                Draw2Numbers(canvas, rect_Card);
                return;
            }
            var tempRect = GetStartingRect(rect_Card);
            canvas.DrawRect(tempRect, GetColorUsed());
            var OtherRect = GetCenterDiceRect();
            PrivateDrawDiceImage(canvas, FirstValue, OtherRect);
            if (Drew || MainGraphics!.IsSelected)
                DrawBorders(canvas, rect_Card);
        }
    }
    public class ChanceGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private int _Points;
        public int Points
        {
            get { return _Points; }
            set
            {
                if (SetProperty(ref _Points, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
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
        public bool NeedsToDrawBacks => false; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            return Points > 0;
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
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Red; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _darkSlateBluePaint;
        private SKPaint? _whitePaint;
        private SKPaint? _blackBorder;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            _darkSlateBluePaint = MiscHelpers.GetSolidPaint(SKColors.DarkSlateBlue);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (Points > 7 || Points < 1)
                throw new Exception("Points only go from 1 to 7");
            if (Points == 4 || Points == 6)
                throw new Exception("There should be no 4 or 6 points");
            var tempSize = MainGraphics!.GetFontSize(3);
            var tempRect = SKRect.Create(rect_Card.Location.X + tempSize, rect_Card.Location.Y + tempSize, rect_Card.Width - (tempSize * 2), rect_Card.Height - (tempSize * 2));
            canvas.DrawRect(tempRect, _darkSlateBluePaint);
            var firstRect = MainGraphics.GetActualRectangle(5, 12, 45, 45);
            canvas.DrawRect(firstRect, _whitePaint);
            canvas.DrawRect(firstRect, _blackBorder);
            firstRect = MainGraphics.GetActualRectangle(5, 12, 40, 30);
            var fontSize = MainGraphics.GetFontSize(30);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            canvas.DrawCustomText(Points.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _); // well see
            var secondRect = MainGraphics.GetActualRectangle(3, 37, 50, 20);
            fontSize = MainGraphics.GetFontSize(14);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            string thisText;
            if (Points == 1)
                thisText = "Point";
            else
                thisText = "Points";
            canvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
        }
    }
}