using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace SnakesAndLaddersCP.GraphicsCP
{
    public class PieceGraphicsCP : BaseGraphicsCP
    {
        private int _number;
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (SetProperty(ref _number, value) == true)
                    // code to run
                    PaintUI?.DoInvalidate();
            }
        }
        private readonly SKPaint _borderPaint;
        public PieceGraphicsCP()
        {
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            NeedsToClear = true;
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (NeedsToClear == true)
                dc.Clear();// needs to always clear.
            if (Number == 0)
                return;
            SKColor thisColor;
            var tempColor = MainColor.ToSKColor();
            thisColor = new SKColor(tempColor.Red, tempColor.Green, tempColor.Blue, 150); // i think
            var thisPaint = MiscHelpers.GetSolidPaint(thisColor);
            var thisRect = GetMainRect();
            dc.DrawOval(thisRect, thisPaint);
            dc.DrawOval(thisRect, _borderPaint);
            var thisText = MiscHelpers.GetTextPaint(SKColors.White, thisRect.Height * 0.7f);
            dc.DrawCustomText(Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisText, thisRect, out _);
        }
    }
}