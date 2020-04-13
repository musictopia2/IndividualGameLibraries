using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace LifeBoardGameCP.Graphics
{
    [SingletonGame] //hopefully no need to reset (?)
    public class SpinnerCP
    {

        private readonly int _intendedArrowHeight = 120;
        private readonly int _intendedArrowWidth = 26;
        private readonly int _intendedBoardSize = 200;
        private int _actualArrowHeight;
        private int _actualArrowWidth;
        private int _actualBoardSize;
        private readonly BasicData _thisData;
        public SpinnerCP(BasicData thisdata)
        {
            _thisData = thisdata;
        }
        public float SuggestedBoardSize
        {
            get
            {
                if (_thisData.IsXamarinForms == false)
                    return 200 * 1.3f;
                return 200 * .9f; //now its easy because only large tablets are supported now.
            }
        }
        #region "Paint Processes"
        private bool _didPaint;
        private SKPaint? _bitPaint;
        private SKBitmap? _arrowImage;
        private SKBitmap? _highSpeed1;
        private SKBitmap? _highSpeed2;
        private SKBitmap? _boardBit;
        private void CreatePaints(int width)
        {
            Assembly thisA = Assembly.GetAssembly(GetType());
            _bitPaint = MiscHelpers.GetBitmapPaint();
            _arrowImage = ImageExtensions.GetSkBitmap(thisA, "arrows.png");
            _highSpeed1 = ImageExtensions.GetSkBitmap(thisA, "highspeed1.png");
            _highSpeed2 = ImageExtensions.GetSkBitmap(thisA, "highspeed2.png");
            _boardBit = ImageExtensions.GetSkBitmap(thisA, "spinner.png");
            _actualBoardSize = width; // try this way
            var diffs = _actualBoardSize / _intendedBoardSize;
            _actualArrowHeight = _intendedArrowHeight * diffs;
            _actualArrowWidth = _intendedArrowWidth * diffs;
            _didPaint = true;
        }
        #endregion
        #region "Draw Processes"
        public void DrawSpinnerBoard(SKCanvas canvas, int width, int height)
        {
            var thisRect = SKRect.Create(0, 0, width, height);
            if (_didPaint == false)
                CreatePaints(width);
            canvas.DrawBitmap(_boardBit, thisRect, _bitPaint);
        }
        public void DrawNormalArrow(SKCanvas canvas, int Position, int width) //somehow height was not necessary.  could put back if needed.
        {
            if (_didPaint == false)
                CreatePaints(width);
            canvas.Clear();
            canvas.Save();
            canvas.RotateDegrees(Position, _actualBoardSize / 2, _actualBoardSize / 2);
            float lefts;
            lefts = (_actualBoardSize / 2) - (_actualArrowWidth / 2);
            float Tops;
            Tops = (_actualBoardSize / 2) - (_actualArrowHeight / 2);
            var ThisRect = SKRect.Create(lefts, Tops, _actualArrowWidth, _actualArrowHeight); // has to see how to position this.
            canvas.DrawBitmap(_arrowImage, ThisRect, _bitPaint);
            canvas.Restore();
        }
        public void DrawHighSpeedArrow(SKCanvas canvas, int phase, int width)
        {
            if (_didPaint == false)
                CreatePaints(width);
            canvas.Clear();
            var ThisRect = SKRect.Create(0, 0, _actualBoardSize, _actualBoardSize);
            if ((phase % 2) == 0)
                canvas.DrawBitmap(_highSpeed1, ThisRect, _bitPaint);
            else
                canvas.DrawBitmap(_highSpeed2, ThisRect, _bitPaint);
        }
        #endregion
    }
}