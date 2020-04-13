using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;

namespace TileRummyCP.Data
{
    public class TileCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private EnumColorType _color = EnumColorType.None;
        public EnumColorType Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (SetProperty(ref _color, value) == true)
                {
                    _tempColor = GetColor();
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        private SKColor GetColor()
        {
            switch (Color)
            {
                case EnumColorType.Black:
                    {
                        return SKColors.Black;
                    }

                case EnumColorType.Blue:
                    {
                        return SKColors.Blue;
                    }

                case EnumColorType.Orange:
                    {
                        return SKColors.Orange;
                    }

                case EnumColorType.Red:
                    {
                        return SKColors.Red;
                    }

                default:
                    {
                        throw new BasicBlankException("Color not found for none or other");
                    }
            }
        }
        private SKColor _tempColor;
        private int _number;
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (SetProperty(ref _number, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private bool _isJoker;
        public bool IsJoker
        {
            get
            {
                return _isJoker;
            }

            set
            {
                if (SetProperty(ref _isJoker, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private EnumDrawType _whatDraw;
        public EnumDrawType WhatDraw
        {
            get
            {
                return _whatDraw;
            }

            set
            {
                if (SetProperty(ref _whatDraw, value) == true)
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
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
            if (MainGraphics!.IsUnknown)
                return true;
            return Color != EnumColorType.None;
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
            else if (WhatDraw == EnumDrawType.FromSet)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _altDrewPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _altDrewPaint;
        private SKPaint? _blackPaint;
        private SKPaint? _blackBorder;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(60, 40); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            SKColor thisColor = SKColors.Purple; //we can change as needed.
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 40);
            _altDrewPaint = MiscHelpers.GetSolidPaint(otherColor);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (IsJoker == true)
            {
                var thisPen = MiscHelpers.GetStrokePaint(_tempColor, 2);
                var tempRect = MainGraphics!.GetActualRectangle(SKRect.Create(16, 6, 28, 28));
                canvas.DrawSmiley(tempRect, null!, thisPen, _blackPaint!);
                return;
            }
            var fontSize = rect_Card.Height * 0.9f; // can be adjusted obviously
            var paint = MiscHelpers.GetTextPaint(_tempColor, fontSize);
            canvas.DrawBorderText(Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, paint, _blackBorder!, rect_Card);
        }
    }
}
