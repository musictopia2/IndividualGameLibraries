using System;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace YahtzeeHandsDownCP.Cards
{
    public class ChanceGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private int _points;
        public int Points
        {
            get { return _points; }
            set
            {
                if (SetProperty(ref _points, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
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
