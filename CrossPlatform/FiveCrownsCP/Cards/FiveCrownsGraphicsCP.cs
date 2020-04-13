using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FiveCrownsCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace FiveCrownsCP.Cards
{
    public class FiveCrownsGraphicsCP : ObservableObject, IDeckGraphicsCP
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
        private EnumCardValueList _cardValue;
        public EnumCardValueList CardValue
        {
            get { return _cardValue; }
            set
            {
                if (SetProperty(ref _cardValue, value))
                {
                    MainGraphics!.PaintUI?.DoInvalidate();
                }

            }
        }
        private EnumSuitList _suit;
        public EnumSuitList Suit
        {
            get { return _suit; }
            set
            {
                if (SetProperty(ref _suit, value))
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
            if (CardValue == EnumCardValueList.Joker)
                return true;
            return Suit != EnumSuitList.None;
        }
        public void DrawDefaultRectangles(SKCanvas dc, SKRect rect_Card, SKPaint thisPaint)
        {
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
        }
        public void DrawBorders(SKCanvas dc, SKRect rect_Card)
        {
            SKPaint thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 3); //i think 3 is enough.
            MainGraphics!.DrawDefaultRectangles(dc, rect_Card, thisPaint);
            if (MainGraphics.IsUnknown == true && MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _tempPaint!);
            else if (MainGraphics.IsSelected == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _selectPaint!);
            else if (Drew == true)
                MainGraphics.DrawDefaultRectangles(dc, rect_Card, _pDrewPaint!);
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card)
        {
            var ThisImage = ImageExtensions.GetSkBitmap(_thisAssembly!, "back.png");
            canvas.DrawBitmap(ThisImage, rect_Card, MainGraphics!.BitPaint);
        }
        public SKColor GetFillColor()
        {
            return MainGraphics!.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private Assembly? _thisAssembly;
        private SKPaint? _borderPaint;
        private SKPaint? _bluePaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _blackPaint;
        private SKPaint? _greenPaint;
        private SKPaint? _redPaint;
        private SKPaint? _thickBorder;
        private SKPaint? _pDrewPaint;
        private SKPaint? _selectPaint;
        private SKPaint? _tempPaint;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            SKColor thisColor = SKColors.Black;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 70); //can experiment as needed.
            _tempPaint = MiscHelpers.GetSolidPaint(otherColor);
            _thisAssembly = Assembly.GetAssembly(this.GetType());
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (CardValue == EnumCardValueList.Joker)
            {
                DrawJoker(canvas);
                return;
            }
            var SuitRect = MainGraphics!.GetActualRectangle(7, 5, 40, 40);
            DrawSuit(canvas, SuitRect);
            var TextRect = MainGraphics.GetActualRectangle(0, 42, 55, 28);
            DrawValue(canvas, TextRect);
        }
        private void DrawValue(SKCanvas Canvas, SKRect rect_Card)
        {
            SKColor thisColor;
            switch (Suit)
            {
                case EnumSuitList.Clubs:
                    {
                        thisColor = SKColors.Green;
                        break;
                    }

                case EnumSuitList.Diamonds:
                    {
                        thisColor = SKColors.Blue;
                        break;
                    }

                case EnumSuitList.Hearts:
                    {
                        thisColor = SKColors.Red;
                        break;
                    }

                case EnumSuitList.Spades:
                    {
                        thisColor = SKColors.Blue;
                        break;
                    }

                case EnumSuitList.Stars:
                    {
                        thisColor = SKColors.Yellow;
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Not Supported");
                    }
            }
            var fontSize = rect_Card.Height * 0.7f; // can vary (?)
            var thisPaint = MiscHelpers.GetTextPaint(thisColor, fontSize);
            Canvas.DrawBorderText(GetTextOfCard(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, _borderPaint!, rect_Card);
        }
        private void DrawSuit(SKCanvas canvas, SKRect rect_Card)
        {
            switch (Suit)
            {
                case EnumSuitList.Clubs:
                    {
                        canvas.DrawCardSuit(BasicGameFrameworkLibrary.RegularDeckOfCards.EnumSuitList.Clubs, rect_Card, _greenPaint!, null);
                        break;
                    }

                case EnumSuitList.Diamonds:
                    {
                        canvas.DrawCardSuit(BasicGameFrameworkLibrary.RegularDeckOfCards.EnumSuitList.Diamonds, rect_Card, _bluePaint!, null);
                        break;
                    }

                case EnumSuitList.Hearts:
                    {
                        canvas.DrawCardSuit(BasicGameFrameworkLibrary.RegularDeckOfCards.EnumSuitList.Hearts, rect_Card, _redPaint!, null);
                        break;
                    }

                case EnumSuitList.Spades:
                    {
                        canvas.DrawCardSuit(BasicGameFrameworkLibrary.RegularDeckOfCards.EnumSuitList.Spades, rect_Card, _blackPaint!, null);
                        break;
                    }

                case EnumSuitList.Stars:
                    {
                        canvas.DrawStar(rect_Card, _yellowPaint!, _thickBorder!);
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Not Supported");
                    }
            }
        }
        private void DrawJoker(SKCanvas canvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(2, 2, 55, 20);
            var fontSize = firstRect.Height * 0.8f; // can be adjusted as needed
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            canvas.DrawCustomText("Joker", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            var secondRect = MainGraphics.GetActualRectangle(8, 25, 40, 40); // i think this makes more sense
            canvas.DrawSmiley(secondRect, null!, _thickBorder!, _blackPaint!);
        }
        private string GetTextOfCard()
        {
            switch (CardValue)
            {
                case EnumCardValueList.Jack:
                    {
                        return "J";
                    }

                case EnumCardValueList.Queen:
                    {
                        return "Q";
                    }

                case EnumCardValueList.King:
                    {
                        return "K";
                    }

                default:
                    {
                        return CardValue.FromEnum().ToString();
                    }
            }
        }
    }
}
