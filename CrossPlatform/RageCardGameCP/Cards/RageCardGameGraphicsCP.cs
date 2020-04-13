using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RageCardGameCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using static SkiaSharpGeneralLibrary.SKExtensions.MiscHelpers;
using static SkiaSharpGeneralLibrary.SKExtensions.TextExtensions;
namespace RageCardGameCP.Cards
{
    public class RageCardGameGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private EnumColor _color;

        public EnumColor Color
        {
            get { return _color; }
            set
            {
                if (SetProperty(ref _color, value))
                {
                    //can decide what to do when property changes
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
                    //can decide what to do when property changes
                }

            }
        }
        private EnumSpecialType _specialType = EnumSpecialType.Blank;

        public EnumSpecialType SpecialType
        {
            get { return _specialType; }
            set
            {
                if (SetProperty(ref _specialType, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        private bool _drew;

        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true;
            return SpecialType != EnumSpecialType.Blank;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint ThisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, ThisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            var FontSize = rect_Card.Height * .3f;
            var TextPaint = GetTextPaint(SKColors.White, FontSize);
            canvas.DrawBorderText("Rage", EnumLayoutOptions.Center, EnumLayoutOptions.Center, TextPaint, _blackBorder!, rect_Card);
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Red; //this is the color of the back card if we have nothing else to it.
            else if (SpecialType == EnumSpecialType.None)
            {
                switch (Color)
                {
                    case EnumColor.Blue:
                    case EnumColor.Red:
                    case EnumColor.Green:
                    case EnumColor.Yellow:
                    case EnumColor.Purple:
                    case EnumColor.Orange:
                        string tempColor = Color.ToColor();
                        return tempColor.ToSKColor();
                    default:
                        throw new BasicBlankException("Not Supported");
                }
            }
            else
                return SKColors.Black; //i think.  hopefully highlighting leaning towards black still works.
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _blackBorder;
        private SKPaint? _whitePaint;
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //i think its 55, 72.  if i am wrong, fix.
            _blackBorder = GetStrokePaint(SKColors.Black, 1);
            _whitePaint = GetSolidPaint(SKColors.White);
            SKColor thisColor = SKColors.Black; //if i can't have black for select, has to choose something else (well have to see what).
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 70); //can experiment as needed.
            _selectPaint = GetSolidPaint(otherColor);
            thisColor = SKColors.White;
            otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 150);
            _pDrewPaint = GetSolidPaint(otherColor);
        }
        private void DrawNumberCard(SKCanvas canvas, SKRect rect_Card)
        {
            float fontSize = rect_Card.Height * .62f;
            var textPaint = GetTextPaint(SKColors.White, fontSize);
            canvas.DrawBorderText(Value.ToString(), EnumLayoutOptions.Center, EnumLayoutOptions.Center, textPaint, _blackBorder!, rect_Card);
        }
        private void DrawSpecialCard(SKCanvas canvas)
        {
            var firstText = SpecialType switch
            {
                EnumSpecialType.Out => "X",
                EnumSpecialType.Change => "!",
                EnumSpecialType.Wild => "W",
                EnumSpecialType.Bonus => "+5",
                EnumSpecialType.Mad => "-5",
                _ => throw new BasicBlankException("Not Supported"),
            };
            var firstRect = MainGraphics!.GetActualRectangle(2, 2, 45, 20);
            float firstFontSize = firstRect.Height * .8f;
            SKPaint firstPaint = GetTextPaint(SKColors.White, firstFontSize);
            canvas.DrawCustomText(firstText, EnumLayoutOptions.Center, EnumLayoutOptions.Center, firstPaint, firstRect, out _);
            var secondRect = MainGraphics.GetActualRectangle(2, 22, 45, 20);
            float secondFontSize = secondRect.Height * .5f;
            SKPaint secondPaint = GetTextPaint(SKColors.Black, secondFontSize);
            canvas.DrawRect(secondRect, _whitePaint);
            canvas.DrawCustomText(SpecialType.ToString(), EnumLayoutOptions.Center, EnumLayoutOptions.Center, secondPaint, secondRect, out _);
            var thirdRect = MainGraphics.GetActualRectangle(2, 42, 45, 28);
            float thirdFontSize = thirdRect.Height * .6f;
            SKPaint thirdPaint = GetTextPaint(SKColors.Red, thirdFontSize);
            canvas.DrawRect(thirdRect, _whitePaint);
            canvas.DrawCustomText("Rage", EnumLayoutOptions.Center, EnumLayoutOptions.Center, thirdPaint, thirdRect, out _);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (SpecialType == EnumSpecialType.None)
                DrawNumberCard(canvas, rect_Card);
            else
                DrawSpecialCard(canvas);
            if (MainGraphics!.IsSelected == true && SpecialType != EnumSpecialType.None)
                canvas.DrawRect(rect_Card, _selectPaint); //try this too.
        }
    }
}
