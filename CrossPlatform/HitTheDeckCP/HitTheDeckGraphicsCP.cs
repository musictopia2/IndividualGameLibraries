using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace HitTheDeckCP
{
    public class HitTheDeckGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private int _Value;

        public int Value
        {
            get { return _Value; }
            set
            {
                if (SetProperty(ref _Value, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }

        private EnumTypeList _CardType;

        public EnumTypeList CardType
        {
            get { return _CardType; }
            set
            {
                if (SetProperty(ref _CardType, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }

        private bool _Drew;

        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
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
            if (CardType == EnumTypeList.Regular && Value == 0)
                return false;
            return CardType != EnumTypeList.None;
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
            canvas.DrawRect(rect_Card, _backPaint);
            CustomBasicList<SKRect> thisList = new CustomBasicList<SKRect>();
            float fontSize;
            fontSize = rect_Card.Height / 3.2f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Yellow, fontSize);
            var thisRect = SKRect.Create(2, 2, rect_Card.Width, rect_Card.Height / 3);
            thisList.Add(thisRect);
            thisRect = SKRect.Create(2, rect_Card.Height / 3, rect_Card.Width, rect_Card.Height / 3);
            thisList.Add(thisRect);
            thisRect = SKRect.Create(2, (rect_Card.Height / 3) * 2, rect_Card.Width, rect_Card.Height / 3);
            thisList.Add(thisRect);
            int x = 0;
            string thisText;
            foreach (var ThisRect in thisList)
            {
                x += 1;
                switch (x)
                {
                    case 1:
                        {
                            thisText = "Hit";
                            break;
                        }

                    case 2:
                        {
                            thisText = "The";
                            break;
                        }

                    case 3:
                        {
                            thisText = "Deck";
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Only 3 lines");
                        }
                }
                canvas.DrawBorderText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _textBorder, ThisRect);
            }
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
        private Assembly? _thisAssembly;
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKBitmap? _cutImage;
        private SKBitmap? _flipImage;
        private SKPaint? _redBrush;
        private SKPaint? _yellowBrush;
        private SKPaint? _blueBrush;
        private SKPaint? _greenBrush;
        private SKPaint? _textBorder;
        private SKPaint? _backPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            SKColor ThisColor = SKColors.Black;
            SKColor OtherColor = new SKColor(ThisColor.Red, ThisColor.Green, ThisColor.Blue, 70); //can experiment as needed.
            _selectPaint = MiscHelpers.GetSolidPaint(OtherColor);
            ThisColor = SKColors.White;
            OtherColor = new SKColor(ThisColor.Red, ThisColor.Green, ThisColor.Blue, 150);
            _pDrewPaint = MiscHelpers.GetSolidPaint(OtherColor);
            _cutImage = ImageExtensions.GetSkBitmap(_thisAssembly, "cut.png");
            _flipImage = ImageExtensions.GetSkBitmap(_thisAssembly, "flip.png");
            _redBrush = MiscHelpers.GetSolidPaint(SKColors.Red);
            _yellowBrush = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _blueBrush = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _greenBrush = MiscHelpers.GetSolidPaint(SKColors.Green);
            _textBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _backPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
        }
        public void DrawImage(SKCanvas Canvas, SKRect rect_Card)
        {
            if (Value > 5)
                return;
            if (CardType != EnumTypeList.Regular && CardType != EnumTypeList.Number && Value != 0)
                return;
            var TempCard = SKRect.Create(2, 2, rect_Card.Width - 4, rect_Card.Height - 4);
            if (CardType != EnumTypeList.Draw4 & CardType != EnumTypeList.Color & CardType != EnumTypeList.Number)
            {
            }
            else
            {
                DrawFourRectangles(Canvas, TempCard);// otherwise, can't show the card selected.
            }
            TempCard = SKRect.Create((float)MainGraphics!.ActualWidth / 15f, (float)MainGraphics.ActualHeight / 10f, (float)MainGraphics.ActualWidth / 4 * 3f, (float)MainGraphics.ActualHeight / 4 * 3f);
            if (CardType == EnumTypeList.Cut)
                DrawCut(Canvas, rect_Card);
            else if (CardType == EnumTypeList.Flip)
                DrawFlip(Canvas, rect_Card);
            else if (CardType == EnumTypeList.Number || CardType == EnumTypeList.Regular)
                DrawBorderText(Canvas, Value.ToString(), TempCard);
            else if (CardType == EnumTypeList.Draw4)
                DrawBorderText(Canvas, "+4?", rect_Card);
            if (CardType == EnumTypeList.Draw4 || CardType == EnumTypeList.Number || CardType == EnumTypeList.Color)
                DrawBorders(Canvas, rect_Card);
        }
        private void DrawBorderText(SKCanvas Canvas, string value, SKRect rect_Card)
        {
            float fontSize;
            if (CardType != EnumTypeList.Draw4)
                fontSize = rect_Card.Height * 1.2f;
            else
                fontSize = rect_Card.Height / 2.5f;

            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            Canvas.DrawBorderText(value, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _textBorder, rect_Card);
        }
        private void DrawFlip(SKCanvas canvas, SKRect rect_Card)
        {
            canvas.DrawBitmap(_flipImage, rect_Card, MainGraphics!.BitPaint);
        }
        private void DrawCut(SKCanvas canvas, SKRect rect_Card)
        {
            canvas.DrawBitmap(_cutImage, rect_Card, MainGraphics!.BitPaint);
        }
        private void DrawFourRectangles(SKCanvas canvas, SKRect rect_Card)
        {
            int widths;
            int heights;
            widths = (int)rect_Card.Width / 2;
            heights = (int)rect_Card.Height / 2;
            SKRect newRect;
            newRect = SKRect.Create(rect_Card.Left, rect_Card.Top, widths, heights);
            canvas.DrawRoundRect(newRect, 0, 0, _redBrush);
            newRect = SKRect.Create(rect_Card.Left + widths, rect_Card.Top, widths, heights);
            canvas.DrawRoundRect(newRect, 0, 0, _yellowBrush);
            newRect = SKRect.Create(rect_Card.Left + widths, rect_Card.Top + heights, widths, heights);
            canvas.DrawRoundRect(newRect, 0, 0, _blueBrush);
            newRect = SKRect.Create(rect_Card.Left, rect_Card.Top + heights, widths, heights);
            canvas.DrawRoundRect(newRect, 0, 0, _greenBrush);
            if ((CardType == EnumTypeList.Color) | (CardType == EnumTypeList.Draw4))
            {
                var newLeft = rect_Card.Left + (widths / 2);
                var newTop = rect_Card.Top + (heights / 2);
                newRect = SKRect.Create(newLeft, newTop, widths, heights);
                var firstPaint = MiscHelpers.GetSolidPaint(MainGraphics!.BackgroundColor.ToSKColor());
                canvas.DrawRect(newRect, firstPaint);
                var SecondPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 2); // try 2
                canvas.DrawRect(newRect, SecondPaint);
            }
        }
    }
}