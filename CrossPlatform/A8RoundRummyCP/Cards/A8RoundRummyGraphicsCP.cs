using A8RoundRummyCP.Data;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
namespace A8RoundRummyCP.Cards
{
    public class A8RoundRummyGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private EnumCardType _cardType = EnumCardType.None;
        public EnumCardType CardType
        {
            get
            {
                return _cardType;
            }

            set
            {
                if (SetProperty(ref _cardType, value) == true)
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }
            }
        }
        private EnumCardShape _shape = EnumCardShape.Blank;
        public EnumCardShape Shape
        {
            get
            {
                return _shape;
            }

            set
            {
                if (SetProperty(ref _shape, value) == true)
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }
            }
        }
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (SetProperty(ref _value, value) == true)
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }
            }
        }
        private EnumColor _color = EnumColor.Blank;
        public EnumColor Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (SetProperty(ref _color, value) == true)
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
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
                    MainGraphics!.PaintUI?.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true;
            return CardType != EnumCardType.None;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            var firstRect = MainGraphics!.GetActualRectangle(0, 1, 54, 23);
            var fontSize = firstRect.Height * 0.65f; // can adjust as needed
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, fontSize);
            var secondRect = MainGraphics.GetActualRectangle(0, 25, 54, 23);
            var thirdRect = MainGraphics.GetActualRectangle(0, 49, 54, 23);
            canvas.DrawCustomText("8", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            canvas.DrawCustomText("Round", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
            canvas.DrawCustomText("Rummy", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thirdRect, out _);
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Blue; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _blackBorders;
        private SKPaint? _thickBorders;
        private SKPaint? _blackPaint;
        private SKPaint? _lightBluePaint;
        private SKPaint? _redPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(107, 135); //change to what the original size is.
            _blackBorders = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _lightBluePaint = MiscHelpers.GetSolidPaint(SKColors.LightBlue);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _thickBorders = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            switch (CardType)
            {
                case EnumCardType.Regular:
                    DrawRegular(canvas);
                    break;
                case EnumCardType.Wild:
                    DrawWild(canvas);
                    break;
                case EnumCardType.Reverse:
                    DrawReverse(canvas);
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
        }
        private void DrawReverse(SKCanvas canvas) // hopefully this simple.
        {
            var firstRect = MainGraphics!.GetActualRectangle(0, 1, 54, 23);
            var fontSize = firstRect.Height * 0.55f; // can adjust as needed
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Green, fontSize);
            var secondRect = MainGraphics.GetActualRectangle(0, 25, 54, 23);
            var thirdRect = MainGraphics.GetActualRectangle(0, 49, 54, 23);
            canvas.DrawCustomText("Reverse", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            canvas.DrawCustomText("And New", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
            canvas.DrawCustomText("Hand", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thirdRect, out _);
        }
        private void DrawWild(SKCanvas canvas)
        {
            var smileRect = MainGraphics!.GetActualRectangle(8, 5, 40, 40);
            var textRect = MainGraphics.GetActualRectangle(0, 40, 55, 30);
            var fontSize = textRect.Height * 0.7f; // can always adjust
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize); // it can be different shape too though.
            canvas.DrawSmiley(smileRect, null!, _thickBorders!, _blackPaint!);
            canvas.DrawCustomText("WILD", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, textRect, out _);
        }
        private void DrawRegular(SKCanvas canvas)
        {
            var shapeRect = MainGraphics!.GetActualRectangle(13, 5, 30, 30);
            SKPaint solidPaint;
            if ((int)Color == (int)EnumColor.Blue)
                solidPaint = _lightBluePaint!;
            else if ((int)Color == (int)EnumColor.Red)
                solidPaint = _redPaint!;
            else
                throw new Exception("Not Supported");
            if ((int)Shape == (int)EnumCardShape.Circle)
            {
                canvas.DrawOval(shapeRect, solidPaint);
                canvas.DrawOval(shapeRect, _thickBorders);
            }
            else if ((int)Shape == (int)EnumCardShape.Square)
            {
                canvas.DrawRect(shapeRect, solidPaint);
                canvas.DrawRect(shapeRect, _thickBorders);
            }
            else if ((int)Shape == (int)EnumCardShape.Triangle)
                canvas.DrawTriangle(shapeRect, solidPaint, _thickBorders!);
            else
                throw new Exception("Not Supported");
            var textRect = MainGraphics.GetActualRectangle(0, 35, 55, 35);
            var fontSize = textRect.Height * 1.1f; // can adjust as needed
            var textPaint = MiscHelpers.GetTextPaint(solidPaint.Color, fontSize);
            canvas.DrawBorderText(Value.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorders!, textRect);
        }
    }
}
