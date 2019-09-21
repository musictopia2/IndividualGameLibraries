using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace BowlingDiceGameCP
{
    public static class BowlingGlobalDrawing
    {
        public static void DrawFrame(SKCanvas thisCanvas, float width, float height, float strokeWidth) // don't need to create a special control for this (i don't think)
        {
            SKPaint thisPaint;
            thisPaint = MiscHelpers.GetStrokePaint(SKColors.White, strokeWidth);
            var thisRect = SKRect.Create(0, 0, width, height);
            thisCanvas.DrawRect(thisRect, thisPaint);
        } // the purpose of doing it this way is it can work for phone.  because phone does not even have a border control.
    }
}