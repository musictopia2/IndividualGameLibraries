using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace ThreeLetterFunCP
{
    public class TileCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public static string TagUsed => "tile"; //standard enough.
        public bool Drew { get; set; } = false; //not relevent this time.  if i am wrong, rethink.
        public bool NeedsToDrawBacks => false;

        private char _Letter;
        public char Letter
        {
            get
            {
                return _Letter;
            }

            set
            {
                if (SetProperty(ref _Letter, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private bool _IsMoved;
        public bool IsMoved
        {
            get
            {
                return _IsMoved;
            }

            set
            {
                if (SetProperty(ref _IsMoved, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        public bool CanStartDrawing()
        {
            return true;
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            throw new BasicBlankException("Should have shown draw backs as false so does not run");
        }

        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            var thisText = Letter.ToString().ToUpper();
            var thisColor = thisText.GetColorOfLetter();
            var fontSize = MainGraphics!.GetFontSize(21);
            var thisPaint = MiscHelpers.GetTextPaint(thisColor, fontSize);
            canvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, rect_Card, out _); // hopefully this simple
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }

        public SKColor GetFillColor()
        {
            if (IsMoved == true)
                return SKColors.Yellow;
            return SKColors.White;
        }
        private SKPaint? _selectPaint;

        public void Init()
        {
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
        }
    }
}