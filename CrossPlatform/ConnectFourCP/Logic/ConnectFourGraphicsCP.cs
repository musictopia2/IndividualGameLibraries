using BasicGameFrameworkLibrary.Attributes;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;

namespace ConnectFourCP.Logic
{
    [SingletonGame]
    public class ConnectFourGraphicsCP
    {
        private readonly SKPaint _borderPaint;
        public ConnectFourGraphicsCP()
        {
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.White, 10); // i think 10
        }
        public void DrawBorders(SKCanvas thisCanvas, float width, float height)
        {
            var thisRect = SKRect.Create(10, 10, width - 20, height - 20);
            thisCanvas.DrawRoundRect(thisRect, 10, 10, _borderPaint);
        }
    }
}