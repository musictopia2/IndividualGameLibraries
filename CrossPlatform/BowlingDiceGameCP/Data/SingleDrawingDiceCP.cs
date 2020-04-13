using SkiaSharp;
using System.Reflection;
using static SkiaSharpGeneralLibrary.SKExtensions.MiscHelpers;
using ii = SkiaSharpGeneralLibrary.SKExtensions.ImageExtensions;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
namespace BowlingDiceGameCP.Data
{
    public class SingleDrawingDiceCP : BaseGraphicsCP
    {

        private bool _DidHit;
        public bool DidHit
        {
            get { return _DidHit; }
            set
            {
                if (SetProperty(ref _DidHit, value))
                {
                    PaintUI!.DoInvalidate();
                }
            }
        }
        private readonly SKPaint _thisPaint;
        private readonly SKBitmap _thisBit;
        private readonly SKPaint _bitPaint;
        public SingleDrawingDiceCP()
        {
            Assembly thisAssembly = Assembly.GetAssembly(GetType());
            _thisPaint = GetSolidPaint(SKColors.White);
            _thisBit = ii.GetSkBitmap(thisAssembly, "bowlingdice.png");
            _bitPaint = GetBitmapPaint();
        }
        public override void DrawImage(SKCanvas dc)
        {
            var thisRect = GetMainRect(); // i think
            dc.DrawRect(thisRect, _thisPaint);
            if (DidHit == true)
                return;
            dc.DrawBitmap(_thisBit, thisRect, _bitPaint);
        }
    }
}
