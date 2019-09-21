using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace FroggiesCP
{
    public class LilyPadCP : ObservableObject
    {
        private int _Row;
        public int Row
        {
            get
            {
                return _Row;
            }

            set
            {
                if (SetProperty(ref _Row, value) == true)
                {
                }
            }
        }

        private int _Column;
        public int Column
        {
            get
            {
                return _Column;
            }

            set
            {
                if (SetProperty(ref _Column, value) == true)
                {
                }
            }
        }

        private bool _HasFrog;
        public bool HasFrog
        {
            get
            {
                return _HasFrog;
            }

            set
            {
                if (SetProperty(ref _HasFrog, value) == true)
                {
                }
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }

            set
            {
                if (SetProperty(ref _IsSelected, value) == true)
                {
                }
            }
        }

        private bool _IsTarget;
        public bool IsTarget
        {
            get
            {
                return _IsTarget;
            }

            set
            {
                if (SetProperty(ref _IsTarget, value) == true)
                {
                }
            }
        }

        private readonly SKBitmap img_LilyPad;
        private readonly SKBitmap img_Frog;
        private readonly SKPaint BitPaint;

        private bool bln_StartedWithFrog;

        public void DidStartWithFrog()
        {
            bln_StartedWithFrog = true;
        }

        public bool StartedWithFrog()
        {
            return bln_StartedWithFrog;
        }
        public void DrawImage(SKCanvas ThisCanvas, float Width, float Height) // i think this is it.
        {
            var ThisRect = SKRect.Create(0, 0, Width, Height);
            ThisCanvas.Clear();
            ThisCanvas.DrawBitmap(img_LilyPad, ThisRect, BitPaint);
            if (HasFrog == true)
                ThisCanvas.DrawBitmap(img_Frog, ThisRect, BitPaint);
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
            img_LilyPad = ImageExtensions.GetSkBitmap(temps, "leaf.png");
            img_Frog = ImageExtensions.GetSkBitmap(temps, "frog.png");
            BitPaint = MiscHelpers.GetBitmapPaint();
        }
    }
}