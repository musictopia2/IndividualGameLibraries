using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
//i think this is the most common things i like to do
namespace MinesweeperCP.Data
{
    public class MineSquareCP : ObservableObject
    {
        public event NeedsToRedrawEventEventHandler? NeedsToRedrawEvent; // so will draw an individual one.

        public delegate void NeedsToRedrawEventEventHandler();

        private bool _isMine;
        public bool IsMine
        {
            get
            {
                return _isMine;
            }

            set
            {
                if (SetProperty(ref _isMine, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();// i think
            }
        }

        private int _neighborMines;
        public int NeighborMines
        {
            get
            {
                return _neighborMines;
            }

            set
            {
                if (SetProperty(ref _neighborMines, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        private bool _flagged;
        public bool Flagged
        {
            get
            {
                return _flagged;
            }

            set
            {
                if (SetProperty(ref _flagged, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        private bool _pressed;
        public bool Pressed
        {
            get
            {
                return _pressed;
            }

            set
            {
                if (SetProperty(ref _pressed, value) == true)
                    // code to run
                    NeedsToRedrawEvent?.Invoke();
            }
        }

        private bool _isFlipped;
        public bool IsFlipped
        {
            get
            {
                return _isFlipped;
            }

            set
            {
                if (SetProperty(ref _isFlipped, value) == true)
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
