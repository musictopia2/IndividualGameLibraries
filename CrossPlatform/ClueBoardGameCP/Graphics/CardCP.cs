using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using ClueBoardGameCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;

namespace ClueBoardGameCP.Graphics
{
    public class CardCP : ObservableObject, IDeckGraphicsCP
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
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        private EnumCardValues _cardValue;
        public EnumCardValues CardValue
        {
            get
            {
                return _cardValue;
            }
            set
            {
                if (SetProperty(ref _cardValue, value) == true)
                    // code to run
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        public bool NeedsToDrawBacks => false; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            return CardValue != EnumCardValues.None;
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
        public void DrawBacks(SKCanvas Canvas, SKRect rect_Card) { }
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
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            Assembly thisA = Assembly.GetAssembly(GetType());
            int tempValue = (int)CardValue;
            var thisImage = ImageExtensions.GetSkBitmap(thisA, "card" + tempValue.ToString() + ".png");
            var tempRect = SKRect.Create(5, 5, rect_Card.Width - 10, rect_Card.Height * 0.65f);
            canvas.DrawBitmap(thisImage, tempRect, MainGraphics!.BitPaint);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height * 0.2f);
            tempRect = SKRect.Create(0, tempRect.Bottom, rect_Card.Width, rect_Card.Height * 0.3f);
            string text = "";
            switch (CardValue)
            {
                case EnumCardValues.BallRoom:
                    {
                        text = "Ball";
                        break;
                    }

                case EnumCardValues.BilliardRoom:
                    {
                        text = "Billiard";
                        break;
                    }

                case EnumCardValues.Candlestick:
                    {
                        textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height * 0.14f);
                        text = "Candlestick";
                        break;
                    }

                case EnumCardValues.Colonel:
                    {
                        text = "Mustard";
                        break;
                    }

                case EnumCardValues.Conservatory:
                    {
                        textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height * 0.12f);
                        text = "Convervatory";
                        break;
                    }

                case EnumCardValues.DiningRoom:
                    {
                        text = "Dining";
                        break;
                    }

                case EnumCardValues.Green:
                    {
                        textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height * 0.15f);
                        text = "Mr. Green";
                        break;
                    }

                case EnumCardValues.Hall:
                    {
                        text = "Hall";
                        break;
                    }

                case EnumCardValues.Kitchen:
                    {
                        text = "Kitchen";
                        break;
                    }

                case EnumCardValues.Knife:
                    {
                        text = "Knife";
                        break;
                    }

                case EnumCardValues.LeadPipe:
                    {
                        textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height * 0.15f);
                        text = "Lead Pipe";
                        break;
                    }

                case EnumCardValues.Library:
                    {
                        text = "Library";
                        break;
                    }

                case EnumCardValues.Lounge:
                    {
                        text = "Lounge";
                        break;
                    }

                case EnumCardValues.Peacock:
                    {
                        text = "Peacock";
                        break;
                    }

                case EnumCardValues.Plum:
                    {
                        textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height * 0.15f);
                        text = "Prof. Plum";
                        break;
                    }

                case EnumCardValues.Revolver:
                    {
                        text = "Revolver";
                        break;
                    }

                case EnumCardValues.Rope:
                    {
                        text = "Rope";
                        break;
                    }

                case EnumCardValues.Scarlet:
                    {
                        text = "Scarlet";
                        break;
                    }

                case EnumCardValues.Study:
                    {
                        text = "Study";
                        break;
                    }

                case EnumCardValues.White:
                    {
                        text = "White";
                        break;
                    }

                case EnumCardValues.Wrench:
                    {
                        text = "Wrench";
                        break;
                    }
            }
            canvas.DrawCustomText(text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, tempRect, out _);
        }
    }
}
