using BasicGameFramework.ColorCards;
using BasicGameFramework.GameGraphicsCP.Cards;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
namespace UnoCP
{
    public class UnoGraphicsCP : BaseColorCardsCP
    {
        protected override SKColor BackColor => SKColors.Red;

        protected override SKColor BackFontColor => SKColors.White;

        protected override string BackText => "Uno";

        private EnumCardTypeList _WhichType = EnumCardTypeList.None;
        public EnumCardTypeList WhichType
        {
            get
            {
                return _WhichType;
            }

            set
            {
                if (SetProperty(ref _WhichType, value) == true)
                    // code to run
                    MainGraphics!.PaintUI?.DoInvalidate();
            }
        }

        private int _Draw;
        public int Draw
        {
            get
            {
                return _Draw;
            }

            set
            {
                if (SetProperty(ref _Draw, value) == true)
                    // code to run
                    MainGraphics!.PaintUI?.DoInvalidate();
            }
        }

        private int _Number;
        public int Number
        {
            get
            {
                return _Number;
            }

            set
            {
                if (SetProperty(ref _Number, value) == true)
                    // code to run
                    MainGraphics!.PaintUI?.DoInvalidate();
            }
        }

        public override bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true;
            if (WhichType == EnumCardTypeList.None)
                return false;
            return true; // try this way instead.
        }

        private readonly SKPaint _redBrush;
        private readonly SKPaint _yellowBrush;
        private readonly SKPaint _blueBrush;
        private readonly SKPaint _greenBrush;

        public UnoGraphicsCP()
        {
            _redBrush = MiscHelpers.GetSolidPaint(SKColors.Red);
            _yellowBrush = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _blueBrush = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _greenBrush = MiscHelpers.GetSolidPaint(SKColors.Green);
        }

        public override SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return base.GetFillColor();
            if (WhichType == EnumCardTypeList.Wild && Color == EnumColorTypes.ZOther)
                return SKColors.White;
            else
                return base.GetFillColor();
        }

        private void DrawFourRectangles(SKCanvas canvas, SKRect rect_Card)
        {
            float widths;
            float heights;
            widths = rect_Card.Width / 2;
            heights = rect_Card.Height / 2;
            SKRect newRect;
            newRect = SKRect.Create(rect_Card.Left + 4, rect_Card.Top + 4, widths - 4, heights - 4);
            canvas.DrawRoundRect(newRect, 0, 0, _redBrush);
            newRect = SKRect.Create(rect_Card.Left + widths, rect_Card.Top + 4, widths - 4, heights - 4);
            canvas.DrawRoundRect(newRect, 0, 0, _yellowBrush);
            newRect = SKRect.Create(rect_Card.Left + widths, rect_Card.Top + heights, widths - 4, heights - 4);
            canvas.DrawRoundRect(newRect, 0, 0, _blueBrush);
            newRect = SKRect.Create(rect_Card.Left + 4, rect_Card.Top + heights, widths - 4, heights - 4);
            canvas.DrawRoundRect(newRect, 0, 0, _greenBrush);
            DrawBorders(canvas, rect_Card); //i think
        }

        public override void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (WhichType == EnumCardTypeList.Draw2)
                DrawValueCard(canvas, rect_Card, "+2");
            else if (WhichType == EnumCardTypeList.Skip)
                DrawValueCard(canvas, rect_Card, "S");
            else if (WhichType == EnumCardTypeList.Reverse)
                DrawValueCard(canvas, rect_Card, "R");
            else if (WhichType == EnumCardTypeList.Regular)
                DrawValueCard(canvas, rect_Card, Number.ToString());
            else if (WhichType == EnumCardTypeList.Wild)
            {
                string thisText;
                if (Draw == 0)
                    thisText = "W";
                else
                    thisText = "+4";
                if (Color == EnumColorTypes.ZOther)
                    DrawFourRectangles(canvas, rect_Card);
                DrawValueCard(canvas, rect_Card, thisText);
            }
            else
                throw new Exception("Can't Draw Uno Card");
        }
    }
}