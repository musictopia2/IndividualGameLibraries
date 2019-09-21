using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace MinesweeperCP
{
    public class MineSquareCP : ObservableObject
    {
        public event NeedsToRedrawEventEventHandler? NeedsToRedrawEvent; // so will draw an individual one.

        public delegate void NeedsToRedrawEventEventHandler();

        private bool _IsMine;
        public bool IsMine
        {
            get
            {
                return _IsMine;
            }

            set
            {
                if (SetProperty(ref _IsMine, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();// i think
            }
        }

        private int _NeighborMines;
        public int NeighborMines
        {
            get
            {
                return _NeighborMines;
            }

            set
            {
                if (SetProperty(ref _NeighborMines, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        private bool _Flagged;
        public bool Flagged
        {
            get
            {
                return _Flagged;
            }

            set
            {
                if (SetProperty(ref _Flagged, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        private bool _Pressed;
        public bool Pressed
        {
            get
            {
                return _Pressed;
            }

            set
            {
                if (SetProperty(ref _Pressed, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        private bool _IsFlipped;
        public bool IsFlipped
        {
            get
            {
                return _IsFlipped;
            }

            set
            {
                if (SetProperty(ref _IsFlipped, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        public int Column { get; set; }
        public int Row { get; set; }

        private SKPaint? _redSolidBrush;
        private SKPaint? _redPenBrush;
        private SKPaint? _slateGrayPenBrush;
        private SKPaint? _blackPenBrush;
        private SKPaint? _darkGrayPenBrush;

        private void CreateBrushes()
        {
            _redSolidBrush = MiscHelpers.GetSolidPaint(SKColors.Red);
            _redPenBrush = MiscHelpers.GetStrokePaint(SKColors.Red, 1);
            _slateGrayPenBrush = MiscHelpers.GetStrokePaint(SKColors.SlateGray, 1);
            _blackPenBrush = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _darkGrayPenBrush = MiscHelpers.GetStrokePaint(SKColors.DarkGray, 1);
        }


        public MineSquareCP(int pColumn, int pRow)
        {
            Column = pColumn;
            Row = pRow;
            CreateBrushes();
        }

        public void DrawSquare(SKCanvas thisCanvas, float width, float height)
        {
            var bounds = SKRect.Create(0, 0, width, height);
            _redPenBrush!.StrokeWidth = bounds.Width / 20;
            _slateGrayPenBrush!.StrokeWidth = bounds.Width / 30;
            _blackPenBrush!.StrokeWidth = bounds.Width / 20;
            _darkGrayPenBrush!.StrokeWidth = bounds.Width / 30;
            MiscHelpers.DefaultFont = "Verdana";
            var thisPercs = MiscHelpers.EnumLinearGradientPercent.Angle45;
            if (IsFlipped == true)
            {
                if (IsMine == true || Flagged == true)
                {
                    var firstColor = new SKColor(255, 255, 255, 100);
                    var secondColor = new SKColor(0, 0, 0, 100);
                    var secondBrush = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, bounds, thisPercs);
                    thisCanvas.DrawOval(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Width / 2), bounds.Width / 4, bounds.Height / 4, _redSolidBrush);
                    // well see how the second part comes along (could be iffy)(?)
                    thisCanvas.DrawOval(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Width / 2), bounds.Width / 4, bounds.Height / 4, secondBrush);
                }
                else if (NeighborMines > 0)
                {
                    var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, (bounds.Height * 3) / 4);
                    thisCanvas.DrawCustomText(NeighborMines.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, bounds, out _); // i think
                }
                if (Flagged == true)
                {
                    thisCanvas.DrawLine(bounds.Location.X, bounds.Location.Y, bounds.Location.X + bounds.Width, bounds.Location.Y + bounds.Height, _blackPenBrush);
                    thisCanvas.DrawLine(bounds.Location.X + bounds.Width, bounds.Location.Y, bounds.Location.X, bounds.Location.Y + bounds.Height, _blackPenBrush);
                }
            }
            else
            {
                SKRect otherRect;
                otherRect = SKRect.Create(bounds.Location.X + (bounds.Width / 6), bounds.Location.Y + (bounds.Height / 6), (bounds.Width * 2) / 3, (bounds.Height * 2) / 3);
                SKColor firstColor;
                SKColor secondColor;
                SKPaint currentPaint;
                if (Pressed == true)
                {
                    currentPaint = _darkGrayPenBrush;
                    firstColor = new SKColor(0, 0, 0, 150);
                    secondColor = new SKColor(255, 255, 255, 150);
                }
                else
                {
                    currentPaint = _slateGrayPenBrush;
                    firstColor = new SKColor(255, 255, 255, 150);
                    secondColor = new SKColor(0, 0, 0, 150);
                }
                var tempPaint = MiscHelpers.GetLinearGradientPaint(firstColor, secondColor, bounds, thisPercs);
                thisCanvas.DrawRect(bounds, tempPaint);
                thisCanvas.DrawRect(otherRect, currentPaint);
                if (Flagged == true)
                {
                    SKPath ThisPath = new SKPath();
                    ThisPath.MoveTo(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 4));
                    ThisPath.LineTo(bounds.Location.X + ((bounds.Width * 3) / 4), bounds.Location.Y + ((bounds.Height * 3) / 8));
                    ThisPath.LineTo(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 2));
                    thisCanvas.DrawPath(ThisPath, _redSolidBrush);
                    thisCanvas.DrawLine(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 3) / 4), bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 4), _redPenBrush);
                }
            }
            thisCanvas.DrawRect(bounds, _slateGrayPenBrush);
        }
    }
}