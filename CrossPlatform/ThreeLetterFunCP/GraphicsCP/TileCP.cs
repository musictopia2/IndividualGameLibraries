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
using ThreeLetterFunCP.Logic;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.GraphicsCP
{
    public class TileCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        public static string TagUsed => "tile"; //standard enough.
        public bool Drew { get; set; } = false; //not relevent this time.  if i am wrong, rethink.
        public bool NeedsToDrawBacks => false;

        private char _letter;
        public char Letter
        {
            get
            {
                return _letter;
            }

            set
            {
                if (SetProperty(ref _letter, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private bool _isMoved;
        public bool IsMoved
        {
            get
            {
                return _isMoved;
            }

            set
            {
                if (SetProperty(ref _isMoved, value) == true)
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
