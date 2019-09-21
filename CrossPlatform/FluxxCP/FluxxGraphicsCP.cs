using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace FluxxCP
{
    public class FluxxGraphicsCP : ObservableObject, IDeckGraphicsCP
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
            return Value != 0;
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
            canvas.DrawRoundRect(rect_Card, 6, 6, _blackPaint);
            if (MainGraphics!.IsSelected == true)
                canvas.DrawRoundRect(rect_Card, 6, 6, _aquaBorder);
            else
                canvas.DrawRoundRect(rect_Card, 6, 6, _thickRed);
            var fontSize = rect_Card.Width * 0.5f;
            canvas.Save();
            canvas.RotateDrawing(RotateExtensions.EnumRotateCategory.FlipAndRotate270, rect_Card);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Aqua, fontSize);
            var thisSize = MainGraphics.GetFontSize(-36);
            var newRect = SKRect.Create(thisSize, 0, rect_Card.Height, rect_Card.Width); // try this way
            canvas.DrawBorderText("Fluxx", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _redBorder, newRect);
            canvas.Restore();
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
        private SKPaint? _redBorder;
        private SKPaint? _blackPaint;
        private SKPaint? _aquaBorder;
        private SKPaint? _thickRed;
        private SKPaint? _aquaPaint;
        private SKPaint? _limeGreenPaint;
        private SKPaint? _orchidPaint;
        private SKPaint? _yellowPaint;
        private CustomBasicList<string>? _sideList;
        private CustomBasicList<string>? _textList;
        private CustomBasicList<string>? _keeperList;
        private SKRect _sideRect;
        private SKRect _rightRect;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(73, 113); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            _redBorder = MiscHelpers.GetStrokePaint(SKColors.Red, 2);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _aquaBorder = MiscHelpers.GetStrokePaint(SKColors.Aqua, 4);
            _thickRed = MiscHelpers.GetStrokePaint(SKColors.Red, 4);
            _aquaPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            _limeGreenPaint = MiscHelpers.GetSolidPaint(SKColors.LimeGreen);
            _orchidPaint = MiscHelpers.GetSolidPaint(SKColors.Orchid);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _sideList = new CustomBasicList<string>() { "Draw 1, Play 1", "Play 2", "Play 3", "Play 4", "Play All", "Keeper Limit 2", "Keeper Limit 3", "Keeper Limit 4", "Draw 2", "Draw 3", "Draw 4", "Draw 5", "Hand Limit 0", "Hand Limit 1", "Hand Limit 2", "No-Hand Bonus", "Poor Bonus", "Rich Bonus", "Reverse Order", "First Play Random", "X=X+1", "Double Agenda", "Milk", "The Rocket", "The Moon", "War", "Television", "The Toaster", "Money", "Love", "Dreams", "Peace", "Bread", "The Sun", "Cookies", "Time", "The Brain", "Death", "Sleep", "Chocolate", "Toast", "5 Keepers", "Time Is Money", "Bed Time", "All You Need Is Love", "Peace (No War)", "Baked Goods", "Dreamland", "Hearts And Minds", "Milk And Cookies", "Rocket To The Moon", "Hippyism", "Night and Day", "Squishy Chocolate", "Rocket Science", "Winning the Lottery", "The Appliances", "The Brain (No TV)", "Death By Chocolate", "Chocolate Cookies", "Chocolate Milk", "10 Cards in Hand", "War=Death", "Trash a New Rule", "Take Another Turn", "Trade Hands", "Trash a Keeper", "Exchange Keepers", "Steal a Keeper", "Use What You Take", "Let's Do That Again", "Scramble Keepers", "Rules Reset", "Empty the Trash", "Draw 3, Play 2", "Draw 2 & Use 'Em", "Everybody Gets 1", "Discard & Draw", "Let's Simplify", "Rotate Hands", "No Limits", "Taxation!", "Jackpot" };
            _textList = new CustomBasicList<string>()
            {
                "Play|2 cards|per turn.",
                "Play|3 cards|per turn.",
                "Play|4 cards|per turn.",
                "Play|all|cards.",
                "The most|keepers|is 2.",
                "The most|keepers|is 3.",
                "The most|keepers|is 4.",
                "Draw|2 cards|per turn.",
                "Draw|3 cards|per turn.",
                "Draw|4 cards|per turn.",
                "Draw|5 cards|per turn.",
                "Hand|Limit Is|0",
                "Hand|Limit Is|1.",
                "Hand|Limit Is|2.",
                "Draw|3 Cards|Empty|Hand.",
                "Draw|1 Card|Least|Keepers.",
                "Can|Play 1|Most|Keepers.",
                "Opposite|Order|Turn.",
                "Next|Player|Random.",
                "Add One|To Any|Number.",
                "Second|Goal|Allowed.",
                "Discard|Rule.",
                "Get|Extra|Turn.",
                "Trade|hand|anybody.",
                "Discard|Any|Keeper.",
                "Trade|Keeper.",
                "Take A|Keeper|To Use.",
                "Take|R. Card|Player.",
                "Use Action|Or Rule|From|Discard.",
                "Reshuffle|Keepers.",
                "Remove|All|Rules.",
                "Reshuffle|Cards.",
                "Draw|3 cards|play 2.",
                "Draw|2 cards|play them.",
                "Draw|1 card|pass out.",
                "Draw|All New|Cards.",
                "Discard|up to|half rules.",
                "Players|Pass|Cards.",
                "Discard|Limit|Rules.",
                "All Players|Pick One|Card.",
                "Draw|3 extra|cards!"
            };
            _keeperList = new CustomBasicList<string>()
            {
                "Milk",
                "Rocket",
                "Moon",
                "Tank",
                "TV",
                "Toaster",
                "Money",
                "Love",
                "Dreams",
                "Peace",
                "Bread",
                "Sun",
                "Cookies",
                "Time",
                "Brain",
                "Death",
                "Sleep",
                "Chocolate"
            };
        }
        private void DrawSideText(SKCanvas thisCanvas, string Text)
        {
            SKPaint ThisPaint;
            switch (Value)
            {
                case object _ when Value <= 22:
                    {
                        ThisPaint = _yellowPaint!;
                        break;
                    }

                case object _ when Value <= 40:
                    {
                        ThisPaint = _limeGreenPaint!;
                        break;
                    }

                case object _ when Value <= 63:
                    {
                        ThisPaint = _orchidPaint!;
                        break;
                    }

                default:
                    {
                        ThisPaint = _aquaPaint!;
                        break;
                    }
            }
            var fontSize = _sideRect.Height * 0.8f; // there is lots of text.  well see how it goes on tablets.
            thisCanvas.Save();
            thisCanvas.RotateDrawing(RotateExtensions.EnumRotateCategory.FlipAndRotate270, _sideRect);
            thisCanvas.DrawRect(_sideRect, ThisPaint);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            thisCanvas.DrawCustomText(Text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _sideRect, out _);
            thisCanvas.Restore(); // forgot this part.
        }
        private void DrawText(SKCanvas thisCanvas, string firstText, string secondText)
        {
            var fontSize = MainGraphics!.GetFontSize(15); // try this way (?)  may have to adjust well see.
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            var firstList = firstText.Split("|").ToCustomBasicList();
            int tops;
            tops = 10;
            foreach (var thisText in firstList)
            {
                var textRect = MainGraphics.GetActualRectangle(20, tops, 48, 20);
                textPaint.FakeBoldText = true;
                thisCanvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, textRect, out _);
                tops += 15;
            }
            var secondList = secondText.Split("|").ToCustomBasicList();
            fontSize = MainGraphics.GetFontSize(12);
            tops = 60;
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            foreach (var thisText in secondList)
            {
                var textRect = MainGraphics.GetActualRectangle(20, tops, 48, 20);
                thisCanvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, textRect, out _);
                tops += 12;
            }
        }
        private void DrawImages(SKCanvas thisCanvas, string firstText, string secondText) // this is the 2 images.
        {
            if ((firstText ?? "") == "War")
                firstText = "Tank";
            if ((secondText ?? "") == "War")
                secondText = "Tank";
            var firstRect = SKRect.Create(_rightRect.Left + (_rightRect.Width / 20), _rightRect.Top + (_rightRect.Height / 2) - (_rightRect.Width * 0.3f), _rightRect.Width - (_rightRect.Width / 10), (_rightRect.Width * 0.7f));
            SKRect secondRect = default;
            if (!string.IsNullOrEmpty(secondText))
            {
                firstRect = SKRect.Create(firstRect.Location.X, firstRect.Location.Y - (firstRect.Height * 0.55f), firstRect.Width, firstRect.Height);
                secondRect = SKRect.Create(firstRect.Location.X, firstRect.Location.Y + (firstRect.Height * 1.1f), firstRect.Width, firstRect.Height);
            }
            var thisBit = ImageExtensions.GetSkBitmap(_thisAssembly, firstText + ".png");
            thisCanvas.DrawBitmap(thisBit, firstRect, MainGraphics!.BitPaint);
            if (!string.IsNullOrEmpty(secondText))
            {
                thisBit = ImageExtensions.GetSkBitmap(_thisAssembly, secondText + ".png");
                thisCanvas.DrawBitmap(thisBit, secondRect, MainGraphics.BitPaint);
            }
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            _sideRect = MainGraphics!.GetActualRectangle(3, 3, 107, 14); // well see how this works.
            _rightRect = MainGraphics.GetActualRectangle(20, 0, 48, 113);
            if (Value == 0)
                throw new BasicBlankException("Must be greater than 0");
            var thisSide = _sideList![Value - 1];
            DrawSideText(canvas, thisSide);
            string firstText;
            string secondText;
            switch (Value)
            {
                case 1:
                    {
                        firstText = "Basic|Rules";
                        secondText = "Draw 1|Play 1";
                        break;
                    }

                case object _ when Value <= 22:
                    {
                        firstText = "New|Rule";
                        secondText = _textList![Value - 2];
                        break;
                    }

                case object _ when Value <= 40:
                    {
                        firstText = "Keeper";
                        secondText = "";
                        break;
                    }

                case object _ when Value <= 63:
                    {
                        firstText = "Goal";
                        if (Value == 42)
                            secondText = "5|Keepers|To Win";
                        else if (Value == 62)
                            secondText = "10 Card|In Hand|To Win";
                        else
                            secondText = "";
                        break;
                    }

                case object _ when Value <= 83:
                    {
                        firstText = "Action";
                        secondText = _textList![(Value - 63) + 20]; // i think
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Must be between 1 and 83; not " + Value);
                    }
            }
            DrawText(canvas, firstText, secondText);
            if ((firstText ?? "") == "Keeper")
                DrawImages(canvas, _keeperList![Value - 23], "");
            else if ((firstText ?? "") == "Goal" && string.IsNullOrEmpty(secondText))
            {
                switch (Value)
                {
                    case 41:
                        {
                            DrawImages(canvas, "Bread", "Toaster");
                            break;
                        }

                    case 43:
                        {
                            DrawImages(canvas, "Time", "Money");
                            break;
                        }

                    case 44:
                        {
                            DrawImages(canvas, "Sleep", "Time");
                            break;
                        }

                    case 45:
                        {
                            DrawImages(canvas, "Love", "");
                            break;
                        }

                    case 46:
                        {
                            DrawImages(canvas, "Peace", "NoWar");
                            break;
                        }

                    case 47:
                        {
                            DrawImages(canvas, "Bread", "Cookies");
                            break;
                        }

                    case 48:
                        {
                            DrawImages(canvas, "Dreams", "Sleep");
                            break;
                        }

                    case 49:
                        {
                            DrawImages(canvas, "Love", "Brain");
                            break;
                        }

                    case 50:
                        {
                            DrawImages(canvas, "Milk", "Cookies");
                            break;
                        }

                    case 51:
                        {
                            DrawImages(canvas, "Rocket", "Moon");
                            break;
                        }

                    case 52:
                        {
                            DrawImages(canvas, "Peace", "Love");
                            break;
                        }

                    case 53:
                        {
                            DrawImages(canvas, "Moon", "Sun");
                            break;
                        }

                    case 54:
                        {
                            DrawImages(canvas, "Sun", "Chocolate");
                            break;
                        }

                    case 55:
                        {
                            DrawImages(canvas, "Rocket", "Brain");
                            break;
                        }

                    case 56:
                        {
                            DrawImages(canvas, "Dreams", "Money");
                            break;
                        }

                    case 57:
                        {
                            DrawImages(canvas, "TV", "Toaster");
                            break;
                        }

                    case 58:
                        {
                            DrawImages(canvas, "Brain", "NoTV");
                            break;
                        }

                    case 59:
                        {
                            DrawImages(canvas, "Death", "Chocolate");
                            break;
                        }

                    case 60:
                        {
                            DrawImages(canvas, "Chocolate", "Cookies");
                            break;
                        }

                    case 61:
                        {
                            DrawImages(canvas, "Chocolate", "Milk");
                            break;
                        }

                    case 63:
                        {
                            DrawImages(canvas, "War", "Death");
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Nothing for " + Value);
                        }
                }
            }
        }
    }
}