using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Reflection;
namespace MonopolyCardGameCP
{
    public class MonopolyCardGameGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }

        private int _CardValue;
        public int CardValue
        {
            get { return _CardValue; }
            set
            {
                if (SetProperty(ref _CardValue, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
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
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown)
                return true; //default to true.  change to what you need to start drawing.
            return CardValue > 0;
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
            var fontSize = MainGraphics!.GetFontSize(13);
            canvas.Save();
            canvas.RotateDrawing(RotateExtensions.EnumRotateCategory.FlipAndRotate270, rect_Card);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Navy, fontSize);
            textPaint.FakeBoldText = true;
            var tempRect = MainGraphics.GetActualRectangle(-18, 15, 72, 55);
            canvas.DrawCustomText("Monopoly", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, tempRect, out _);
            tempRect = MainGraphics.GetActualRectangle(-18, 30, 72, 55);
            canvas.DrawCustomText("Card Game", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, tempRect, out _);
            canvas.Restore();
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Aqua; //this is the color of the back card if we have nothing else to it.
            return MainGraphics.GetFillColor();
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private Assembly? _thisAssembly;
        private SKPaint? _pDrewPaint;
        private SKPaint? _selectPaint;
        private SKPaint? _purplePaint;
        private SKPaint? _aquaPaint;
        private SKPaint? _fuchsiaPaint;
        private SKPaint? _darkOrangePaint;
        private SKPaint? _redPaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _greenPaint;
        private SKPaint? _darkBluePaint;
        private SKPaint? _whitePaint;
        private SKPaint? _blackBorder1;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72); //change to what the original size is.
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _purplePaint = MiscHelpers.GetSolidPaint(SKColors.Purple);
            _aquaPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            _fuchsiaPaint = MiscHelpers.GetSolidPaint(SKColors.Fuchsia);
            _darkOrangePaint = MiscHelpers.GetSolidPaint(SKColors.DarkOrange);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
            _darkBluePaint = MiscHelpers.GetSolidPaint(SKColors.DarkBlue);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _blackBorder1 = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        private void DrawProperties(SKCanvas thisCanvas) // this is a big one.
        {
            SKPaint fillColor;
            SKColor foreColor; // used for textpaint.
            string text1;
            string text2;
            switch (CardValue)
            {
                case 19:
                    {
                        text1 = "Medit Avenue";
                        fillColor = _purplePaint!;
                        foreColor = SKColors.White;
                        text2 = "1 of 2";
                        break;
                    }

                case 20:
                    {
                        text1 = "Baltic Avenue";
                        fillColor = _purplePaint!;
                        foreColor = SKColors.White;
                        text2 = "2 of 2";
                        break;
                    }

                case 21:
                    {
                        text1 = "Oriental Avenue";
                        fillColor = _aquaPaint!;
                        foreColor = SKColors.Black;
                        text2 = "1 of 3";
                        break;
                    }

                case 22:
                    {
                        text1 = "Vermont Avenue";
                        fillColor = _aquaPaint!;
                        foreColor = SKColors.Black;
                        text2 = "2 of 3";
                        break;
                    }

                case 23:
                    {
                        text1 = "Connect Avenue";
                        fillColor = _aquaPaint!;
                        foreColor = SKColors.Black;
                        text2 = "3 of 3";
                        break;
                    }

                case 24:
                    {
                        text1 = "Charles Avenue";
                        fillColor = _fuchsiaPaint!;
                        foreColor = SKColors.Black;
                        text2 = "1 or 3";
                        break;
                    }

                case 25:
                    {
                        text1 = "States Avenue";
                        fillColor = _fuchsiaPaint!;
                        foreColor = SKColors.Black;
                        text2 = "2 of 3";
                        break;
                    }

                case 26:
                    {
                        text1 = "Virginia Avenue";
                        fillColor = _fuchsiaPaint!;
                        foreColor = SKColors.Black;
                        text2 = "3 of 3";
                        break;
                    }

                case 27:
                    {
                        text1 = "James Place";
                        fillColor = _darkOrangePaint!;
                        foreColor = SKColors.Black;
                        text2 = "1 of 3";
                        break;
                    }

                case 28:
                    {
                        text1 = "Tenn Avenue";
                        fillColor = _darkOrangePaint!;
                        foreColor = SKColors.Black;
                        text2 = "2 of 3";
                        break;
                    }

                case 29:
                    {
                        text1 = "York Avenue";
                        fillColor = _darkOrangePaint!;
                        foreColor = SKColors.Black;
                        text2 = "3 of 3";
                        break;
                    }

                case 30:
                    {
                        text1 = "Kentucky Avenue";
                        fillColor = _redPaint!;
                        foreColor = SKColors.White;
                        text2 = "1 of 3";
                        break;
                    }

                case 31:
                    {
                        text1 = "Indiana Avenue";
                        fillColor = _redPaint!;
                        foreColor = SKColors.White;
                        text2 = "2 to 3";
                        break;
                    }

                case 32:
                    {
                        text1 = "Illinois Avenue";
                        fillColor = _redPaint!;
                        foreColor = SKColors.White;
                        text2 = "3 of 3";
                        break;
                    }

                case 33:
                    {
                        text1 = "Atlantic Avenue";
                        fillColor = _yellowPaint!;
                        foreColor = SKColors.Black;
                        text2 = "1 of 3";
                        break;
                    }

                case 34:
                    {
                        text1 = "Ventor Avenue";
                        fillColor = _yellowPaint!;
                        foreColor = SKColors.Black;
                        text2 = "2 of 3";
                        break;
                    }

                case 35:
                    {
                        text1 = "Marvin Gardens";
                        fillColor = _yellowPaint!;
                        foreColor = SKColors.Black;
                        text2 = "3 of 3";
                        break;
                    }

                case 36:
                    {
                        text1 = "Pacific Avenue";
                        fillColor = _greenPaint!;
                        foreColor = SKColors.White;
                        text2 = "1 of 3";
                        break;
                    }

                case 37:
                    {
                        text1 = "Carolina Avenue";
                        fillColor = _greenPaint!;
                        foreColor = SKColors.White;
                        text2 = "2 of 3";
                        break;
                    }

                case 38:
                    {
                        text1 = "Penn Avenue";
                        fillColor = _greenPaint!;
                        foreColor = SKColors.White;
                        text2 = "3 of 3";
                        break;
                    }

                case 39:
                    {
                        text1 = "Park Place";
                        fillColor = _darkBluePaint!;
                        foreColor = SKColors.White;
                        text2 = "1 of 2";
                        break;
                    }

                case 40:
                    {
                        text1 = "Board Walk";
                        fillColor = _darkBluePaint!;
                        foreColor = SKColors.White;
                        text2 = "2 of 2";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Nothing Found");
                    }
            }
            var firstRect = MainGraphics!.GetActualRectangle(3, 3, 50, 36);
            thisCanvas.DrawRect(firstRect, fillColor);
            var thisList = text1.Split(" ").ToCustomBasicList();
            var fontSize = MainGraphics.GetFontSize(11);
            var textPaint = MiscHelpers.GetTextPaint(foreColor, fontSize);
            textPaint.FakeBoldText = true;
            if (thisList.Count != 2)
                throw new Exception("Needs 2 items");
            int tops;
            tops = 1;
            foreach (var thisText in thisList)
            {
                var thisRect = MainGraphics.GetActualRectangle(1, tops, 53, 18);
                thisCanvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisRect, out _);
                tops += 18;
            }

            var secondRect = MainGraphics.GetActualRectangle(0, 37, 55, 35);
            fontSize = MainGraphics.GetFontSize(15);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText(text2, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
        }
        private void DrawTokenText(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(0, 0, 55, 25);
            var fontSize = MainGraphics.GetFontSize(15);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText("Token", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisRect, out _);
        }
        private void DrawChance(SKCanvas thisCanvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(2, 0, 51, 26);
            var fontSize = MainGraphics.GetFontSize(12);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText("Chance", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            var secondRect = MainGraphics.GetActualRectangle(0, 10, 60, 70);
            fontSize = MainGraphics.GetFontSize(45);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Red, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawBorderText("?", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder1, secondRect);
        }
        private void DrawGo(SKCanvas thisCanvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(0, 0, 55, 40);
            var fontSize = MainGraphics.GetFontSize(30);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawBorderText("Go", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder1, firstRect);
            var secondRect = MainGraphics.GetActualRectangle(0, 35, 55, 20);
            fontSize = MainGraphics.GetFontSize(14); // decided to make slightly smaller
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            var thirdRect = MainGraphics.GetActualRectangle(0, 53, 55, 20);
            thisCanvas.DrawCustomText("Collect", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
            thisCanvas.DrawCustomText("$200", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thirdRect, out _);
        }
        private void DrawMRMonopolyText(SKCanvas thisCanvas)
        {
            var fontSize = MainGraphics!.GetFontSize(10);
            var firstRect = MainGraphics.GetActualRectangle(0, 5, 55, 15);
            var secondRect = MainGraphics.GetActualRectangle(0, 15, 55, 15);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText("Mr.", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            thisCanvas.DrawCustomText("Monopoly", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
        }
        private void DrawHouseText(SKCanvas thisCanvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(2, 2, 51, 18);
            var secondRect = MainGraphics.GetActualRectangle(2, 15, 51, 18);
            string firstText = "";
            if (CardValue == 8)
                firstText = "1st";
            else if (CardValue == 9)
                firstText = "2nd";
            else if (CardValue == 10)
                firstText = "3rd";
            else if (CardValue == 11)
                firstText = "4th";
            var fontSize = MainGraphics.GetFontSize(13);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText(firstText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            thisCanvas.DrawCustomText("House", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
        }
        private void DrawHotel(SKCanvas thisCanvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(2, 2, 51, 28);
            var fontSize = MainGraphics.GetFontSize(14);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText("Hotel", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            var bounds = MainGraphics.GetActualRectangle(15, 30, 25, 35);
            thisCanvas.DrawRect(bounds, _redPaint);
            int int_Row;
            int int_Col;
            for (int_Row = 0; int_Row <= 2; int_Row++)
            {
                for (int_Col = 0; int_Col <= 1; int_Col++)
                    thisCanvas.DrawRect(SKRect.Create(bounds.Location.X + (bounds.Width / 20) + ((bounds.Width / 5) * int_Col), bounds.Location.Y + (bounds.Height / 10) + ((bounds.Height / 5) * int_Row), bounds.Width / 6, bounds.Height / 6), _whitePaint);
                for (int_Col = 1; int_Col <= 2; int_Col++)
                    thisCanvas.DrawRect(SKRect.Create((bounds.Location.X + ((bounds.Width * 19) / 20)) - ((bounds.Width / 5) * int_Col), bounds.Location.Y + (bounds.Height / 10) + ((bounds.Height / 5) * int_Row), bounds.Width / 6, bounds.Height / 6), _whitePaint);
            }
            thisCanvas.DrawRect(SKRect.Create((bounds.Location.X + (bounds.Width / 2)) - (bounds.Width / 10), bounds.Location.Y + ((bounds.Height * 4) / 5), bounds.Width / 5, bounds.Height / 5), _whitePaint);
        }
        private void DrawRailRoadText(SKCanvas thisCanvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(0, 2, 55, 15);
            var secondRect = MainGraphics.GetActualRectangle(0, 15, 55, 15);
            var fontSize = MainGraphics.GetFontSize(11);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            string firstText = "";
            if (CardValue == 13)
                firstText = "Reading";
            else if (CardValue == 14)
                firstText = "Penny";
            else if (CardValue == 15)
                firstText = "B & O";
            else if (CardValue == 16)
                firstText = "Shortline";
            thisCanvas.DrawCustomText(firstText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            thisCanvas.DrawCustomText("Railroad", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
        }
        private void DrawUtilityText(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(0, 0, 55, 26);
            var fontSize = MainGraphics.GetFontSize(15);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText("Utility", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisRect, out _);
        }
        private void DrawTokenImage(SKCanvas thisCanvas)
        {
            string imageName = "";
            if (CardValue == 1)
                imageName = "Dog.png";
            else if (CardValue == 2)
                imageName = "Horse.png";
            else if (CardValue == 3)
                imageName = "Iron.png";
            else if (CardValue == 4)
                imageName = "Car.png";
            else if (CardValue == 5)
                imageName = "Ship.png";
            else if (CardValue == 6)
                imageName = "Hat.png";
            var ThisRect = MainGraphics!.GetActualRectangle(6, 25, 40, 40);
            DrawActualImage(thisCanvas, imageName, ThisRect);
        }
        private void DrawMrMonopolyImage(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(13, 30, 25, 35);
            DrawActualImage(thisCanvas, "MrMonopoly.png", thisRect);
        }
        private void DrawHouseImage(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(0, 35, 55, 30);
            DrawActualImage(thisCanvas, "House.png", thisRect);
        }
        private void DrawRailroadImage(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(2, 30, 51, 30);
            DrawActualImage(thisCanvas, "RailRoad.png", thisRect);
        }
        private void DrawElectricImage(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(7, 25, 40, 40);
            DrawActualImage(thisCanvas, "Electric.png", thisRect);
        }
        private void DrawWaterworksImage(SKCanvas thisCanvas)
        {
            var thisRect = MainGraphics!.GetActualRectangle(7, 25, 40, 40);
            DrawActualImage(thisCanvas, "Waterworks.png", thisRect);
        }
        private void DrawActualImage(SKCanvas thisCanvas, string fileName, SKRect thisRect)
        {
            var thisBit = ImageExtensions.GetSkBitmap(_thisAssembly, fileName);
            thisCanvas.DrawBitmap(thisBit, thisRect, MainGraphics!.BitPaint);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            switch (CardValue)
            {
                case object _ when CardValue < 7:
                    {
                        DrawTokenText(canvas);
                        DrawTokenImage(canvas);
                        break;
                    }

                case 7:
                    {
                        DrawMRMonopolyText(canvas);
                        DrawMrMonopolyImage(canvas);
                        break;
                    }

                case object _ when CardValue < 12:
                    {
                        DrawHouseText(canvas);
                        DrawHouseImage(canvas);
                        break;
                    }

                case 12:
                    {
                        DrawHotel(canvas);
                        break;
                    }

                case object _ when CardValue < 17:
                    {
                        DrawRailRoadText(canvas);
                        DrawRailroadImage(canvas);
                        break;
                    }

                case 17:
                    {
                        DrawUtilityText(canvas);
                        DrawElectricImage(canvas);
                        break;
                    }

                case 18:
                    {
                        DrawUtilityText(canvas);
                        DrawWaterworksImage(canvas);
                        break;
                    }

                case object _ when CardValue < 41:
                    {
                        DrawProperties(canvas);
                        break;
                    }

                case 41:
                    {
                        DrawChance(canvas);
                        break;
                    }

                case 42:
                    {
                        DrawGo(canvas);
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Nothing found");
                    }
            }
        }
    }
}