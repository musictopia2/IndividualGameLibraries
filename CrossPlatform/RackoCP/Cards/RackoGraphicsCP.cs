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
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
//i think this is the most common things i like to do
namespace RackoCP.Cards
{
    public class RackoGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }
            }
        }
        private int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true;
            return Value > 0;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            float fontSize = rect_Card.Height * .9f;
            SKPaint textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            canvas.DrawCustomText("Racko", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Card, out _);
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Red; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private int _maxs;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(200, 35); //change to what the original size is.
            RackoDeckCount temps = Resolve<RackoDeckCount>();
            _maxs = temps.GetDeckCount();
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            var maxSize = MainGraphics!.GetFontSize(40);
            double percs = Value / (double)_maxs;
            var maxLeft = rect_Card.Width - maxSize;
            var lefts = maxLeft * percs;
            var tempRect = SKRect.Create((float)lefts, 0, rect_Card.Width, rect_Card.Height);
            float fontSize = rect_Card.Height * .9f;
            SKPaint TextPaint = MiscHelpers.GetTextPaint(SKColors.Red, fontSize);
            canvas.DrawCustomText(Value.ToString(), TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Center, TextPaint, tempRect, out _);
        }
    }
}
