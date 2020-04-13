using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace FroggiesCP.Data
{
    public class LilyPadCP : ObservableObject
    {
        private int _row;
        public int Row
        {
            get
            {
                return _row;
            }

            set
            {
                if (SetProperty(ref _row, value) == true)
                {
                }
            }
        }

        private int _column;
        public int Column
        {
            get
            {
                return _column;
            }

            set
            {
                if (SetProperty(ref _column, value) == true)
                {
                }
            }
        }

        private bool _hasFrog;
        public bool HasFrog
        {
            get
            {
                return _hasFrog;
            }

            set
            {
                if (SetProperty(ref _hasFrog, value) == true)
                {
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (SetProperty(ref _isSelected, value) == true)
                {
                }
            }
        }

        private bool _isTarget;
        public bool IsTarget
        {
            get
            {
                return _isTarget;
            }

            set
            {
                if (SetProperty(ref _isTarget, value) == true)
                {
                }
            }
        }

        private readonly SKBitmap _img_LilyPad;
        private readonly SKBitmap _img_Frog;
        private readonly SKPaint _bitPaint;

        private bool _bln_StartedWithFrog;

        public void DidStartWithFrog()
        {
            _bln_StartedWithFrog = true;
        }

        public bool StartedWithFrog()
        {
            return _bln_StartedWithFrog;
        }
        public void DrawImage(SKCanvas ThisCanvas, float Width, float Height) // i think this is it.
        {
            var ThisRect = SKRect.Create(0, 0, Width, Height);
            ThisCanvas.Clear();
            ThisCanvas.DrawBitmap(_img_LilyPad, ThisRect, _bitPaint);
            if (HasFrog == true)
                ThisCanvas.DrawBitmap(_img_Frog, ThisRect, _bitPaint);
            SKPaint? ThisPaint = null;
            if (IsSelected == true)
            {
                ThisPaint = MiscHelpers.GetStrokePaint(SKColors.Red, Width / 30);
            }
            else if (IsTarget == true)
            {
                ThisPaint = MiscHelpers.GetStrokePaint(SKColors.Lime, Width / 10);
                float[] Ins;
                Ins = new float[2];
                Ins[0] = Width / 20;
                Ins[1] = Width / 20;
                ThisPaint.PathEffect = SKPathEffect.CreateDash(Ins, 1); // not sure
            }
            if (IsSelected == true || IsTarget == true)
                ThisCanvas.DrawRoundRect(ThisRect, Width / 10, Width / 10, ThisPaint);
        }
        public LilyPadCP(int x, int y, bool pHasFrog)
        {
            Column = x;
            Row = y;
            HasFrog = pHasFrog;
            Assembly temps = Assembly.GetAssembly(GetType());
            _img_LilyPad = ImageExtensions.GetSkBitmap(temps, "leaf.png");
            _img_Frog = ImageExtensions.GetSkBitmap(temps, "frog.png");
            _bitPaint = MiscHelpers.GetBitmapPaint();
        }
    }
}
