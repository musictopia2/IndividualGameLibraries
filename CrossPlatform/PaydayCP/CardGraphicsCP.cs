using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Linq;
using System.Reflection;
namespace PaydayCP
{
    public class CardGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
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
        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                if (SetProperty(ref _Index, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }

            }
        }
        private EnumCardCategory _CardCategory;
        public EnumCardCategory CardCategory
        {
            get { return _CardCategory; }
            set
            {
                if (SetProperty(ref _CardCategory, value))
                {
                    MainGraphics?.PaintUI?.DoInvalidate();
                }
            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (CardCategory == EnumCardCategory.None)
                return false;
            if (Index == 0)
                return false;
            return true;
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
        }
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
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
        #region "Paints"
        private SKPaint? _border2Paint;
        private SKPaint? _border1Paint;
        private SKPaint? _slateGrayPaint;
        private SKPaint? _lightBluePaint;
        private SKPaint? _darkRedPaint;
        private SKPaint? _redPaint;
        private SKPaint? _bluePaint;
        private SKPaint? _goldPaint;
        private SKPaint? _aquaPaint;
        private SKPaint? _blackPaint;
        private SKPaint? _whitePaint;
        private SKPaint? _darkGoldenRodPaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _silverBorder;
        private SKPaint? _brownPaint;
        private SKPaint? _limeGreenPaint;
        private SKPaint? _darkGrayPaint;
        private SKPaint? _lightGrayBorder;
        private SKPaint? _darkBluePaint;
        private SKPaint? _sandyBrownPaint;
        private SKPaint? _peachPuffPaint;
        private SKPaint? _greenPaint;
        private SKPaint? _salmonPaint;
        private SKPaint? _silverPaint;
        private SKPaint? _orchidPaint;
        private SKPaint? _redBrownHatch;
        private SKPaint? _silverWhiteHatch;
        private SKPaint? _redYellowHatch;
        private SKPaint? _lightGrayWhiteHatch;
        private SKPaint? _redBorder;
        private SKPaint? _skyBluePaint;
        private SKPaint? _greenBorder;
        private SKPaint? _lightYellowPaint;
        private Assembly? _thisAssembly;
        private SKPaint? _selectPaint;
        private SKPaint GetPaint(string path)
        {
            var thisPaint = MiscHelpers.GetBitmapPaint();
            thisPaint.Shader = ImageExtensions.GetSkShader(_thisAssembly, path);
            return thisPaint;
        }
        public void Init()
        {
            _thisAssembly = Assembly.GetAssembly(this.GetType());
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _border2Paint = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _border1Paint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _slateGrayPaint = MiscHelpers.GetSolidPaint(SKColors.SlateGray);
            _lightBluePaint = MiscHelpers.GetSolidPaint(SKColors.LightBlue);
            _darkRedPaint = MiscHelpers.GetSolidPaint(SKColors.DarkRed);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _goldPaint = MiscHelpers.GetSolidPaint(SKColors.Gold);
            _aquaPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _darkGoldenRodPaint = MiscHelpers.GetSolidPaint(SKColors.DarkGoldenrod);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _silverBorder = MiscHelpers.GetStrokePaint(SKColors.Silver, 1);
            _brownPaint = MiscHelpers.GetSolidPaint(SKColors.Brown);
            _limeGreenPaint = MiscHelpers.GetSolidPaint(SKColors.LimeGreen);
            _darkGrayPaint = MiscHelpers.GetSolidPaint(SKColors.DarkGray);
            _lightGrayBorder = MiscHelpers.GetStrokePaint(SKColors.LightGray, 1);
            _darkBluePaint = MiscHelpers.GetSolidPaint(SKColors.DarkBlue);
            _sandyBrownPaint = MiscHelpers.GetSolidPaint(SKColors.SandyBrown);
            _peachPuffPaint = MiscHelpers.GetSolidPaint(SKColors.PeachPuff);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
            _salmonPaint = MiscHelpers.GetSolidPaint(SKColors.Salmon);
            _silverPaint = MiscHelpers.GetSolidPaint(SKColors.Silver);
            _orchidPaint = MiscHelpers.GetSolidPaint(SKColors.Orchid);
            _redBrownHatch = GetPaint("redbrownzip.png");
            _silverWhiteHatch = GetPaint("silverwhitecsmallcon.png");
            _redYellowHatch = GetPaint("redyellowlargecon.png");
            _lightGrayWhiteHatch = GetPaint("lightgraywhitesmallcon.png");
            _redBorder = MiscHelpers.GetStrokePaint(SKColors.Red, 1);
            _skyBluePaint = MiscHelpers.GetSolidPaint(SKColors.SkyBlue);
            _greenBorder = MiscHelpers.GetStrokePaint(SKColors.Green, 1);
            _lightYellowPaint = MiscHelpers.GetSolidPaint(SKColors.LightYellow);
        }
        #endregion
        private void DrawDealText(SKCanvas canvas, string texts, float fontSize, SKRect rects)
        {
            var thisPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize, "Times New Roman");
            canvas.DrawCustomText(texts, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, rects, out _);
        }
        private void DraweDealData(SKCanvas canvas, SKRect bounds)
        {
            float fontSize;
            fontSize = bounds.Height * 0.087f;
            var rect_Top = SKRect.Create(bounds.Left, bounds.Top, bounds.Width, bounds.Height * 0.6f);
            var rect_Bottom = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.6f), bounds.Width, bounds.Height * 0.4f);
            var rect_BottomLeft = SKRect.Create(rect_Bottom.Left, rect_Bottom.Top, rect_Bottom.Width / (float)2, rect_Bottom.Height);
            var rect_BottomRight = SKRect.Create(rect_Bottom.Left + (rect_Bottom.Width / (float)2), rect_Bottom.Top, rect_Bottom.Width / (float)2, rect_Bottom.Height);
            var point1 = new SKPoint(rect_BottomRight.Location.X, rect_BottomRight.Location.Y);
            var point2 = new SKPoint(rect_BottomRight.Location.X, rect_BottomRight.Top + rect_BottomRight.Height);
            SKRect rect_TopLeft;
            SKRect rect_TopRight;
            canvas.DrawLine(point1.X, point1.Y, point2.X, point2.Y, _border2Paint);
            switch (Index)
            {
                case 1:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost  $12,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value $20,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Rocket Ship International", fontSize, rect_TopRight);
                        DrawRocketShip(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 2:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost    $3,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value   $6,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Pop's Soda Pop Inc.", fontSize, rect_TopRight);
                        DrawSoda(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 3:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create((rect_Top.Left + rect_TopLeft.Width) - (float)10, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $3,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $6,000", fontSize, rect_BottomRight);
                        fontSize = bounds.Height * 0.075f;
                        DrawDealText(canvas, "Heavenly Pink Cosmetic Company", fontSize, rect_TopRight);
                        DrawLipstick(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 4:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost  $10,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value $18,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Humungous Hippos Ltd.", fontSize, rect_TopRight);
                        DrawHippo(canvas, SKRect.Create(rect_TopLeft.Left, rect_TopLeft.Top + (rect_TopLeft.Height * 0.05f), rect_TopLeft.Width, rect_TopLeft.Height * 0.9f));
                        break;
                    }

                case 5:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width - (float)10, rect_Top.Height);
                        rect_TopRight.Left += 10;
                        DrawDealText(canvas, "Cost      $3,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value      $6,500", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Chuckles Comedy Club", fontSize, rect_TopRight);
                        DrawComedy(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * (float)1, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 6:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width - (float)20, rect_Top.Height);
                        rect_TopRight.Left += 20;
                        DrawDealText(canvas, "Cost   $11,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value $16,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Hot Rocks Jewelry Co.", fontSize, rect_TopRight);
                        DrawJewelry(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * (float)1, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 7:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost   $11,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value $19,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Fly-By-Night Airlines", fontSize, rect_TopRight);
                        DrawPlane(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 1.1f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 8:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $2,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $6,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Burger 'N' Buns", fontSize, rect_TopRight);
                        DrawBurger(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.2f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.6f));
                        break;
                    }

                case 9:
                    {
                        rect_TopRight = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width, rect_Top.Height / (float)2);
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top + (rect_Top.Height / (float)2), rect_Top.Width, rect_Top.Height / (float)2);
                        DrawDealText(canvas, "Cost      $6,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $11,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Louie's Limo Inc.", fontSize, rect_TopRight);
                        DrawLimo(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top, rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 10:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.3f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $8,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value    $12,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Dipsydoodle Noodles", fontSize, rect_TopRight);
                        DrawNoodles(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * (float)1, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 11:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $9,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value    $14,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Sahara Safari", fontSize, rect_TopRight);
                        DrawSafari(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * (float)1, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 12:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost     $8,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value   $12,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Pete's Pizza Palace", fontSize, rect_TopRight);
                        DrawPizza(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.2f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 13:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $8,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value    $12,000", fontSize, rect_BottomRight);
                        fontSize = bounds.Height * 0.08f;
                        DrawDealText(canvas, "Galloping Golf  Ball Co.", fontSize, rect_TopRight);
                        DrawGolf(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.2f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 14:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $7,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value    $14,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Teen Jeans Inc.", fontSize, rect_TopRight);
                        DrawJeans(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.2f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 15:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $3,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $6,500", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Shepard's Pie Co.", fontSize, rect_TopRight);
                        DrawShephardsPie(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.2f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 16:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $2,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $5,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Tippytoe Ballet School", fontSize, rect_TopRight);
                        DrawBallet(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.2f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.8f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 17:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost  $10,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value $15,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Rock More Records", fontSize, rect_TopRight);
                        DrawMusic(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.2f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.7f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 18:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create((rect_Top.Left + rect_TopLeft.Width) - (float)10, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $2,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $5,500", fontSize, rect_BottomRight);
                        fontSize = bounds.Height * 0.074f;
                        DrawDealText(canvas, "Wheels 'N' Squeals Skateboards", fontSize, rect_TopRight);
                        DrawSkateboard(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.9f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 19:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $4,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $7,500", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Miss Muffet's Tuffets", fontSize, rect_TopRight); // maybe in red
                        DrawMissMuffet(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.3f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.5f, rect_TopLeft.Height * 0.8f));
                        break;
                    }

                case 20:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $2,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $6,500", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Gotcha Security Inc.", fontSize, rect_TopRight);
                        DrawLock(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.9f, rect_TopLeft.Height * 0.9f));
                        break;
                    }

                case 21:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost  $12,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value $20,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Everglades Condo", fontSize, rect_TopRight);
                        DrawCondo(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.9f, rect_TopLeft.Height * 0.9f));
                        break;
                    }

                case 22:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width - (float)10, rect_Top.Height);
                        rect_TopRight.Left += 10;
                        DrawDealText(canvas, "Cost     $7,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value   $13,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Laughing Gas Inc.", fontSize, rect_TopRight);
                        DrawLaughingGas(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.9f, rect_TopLeft.Height * 0.9f));
                        break;
                    }

                case 23:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width - (float)10, rect_Top.Height);
                        rect_TopRight.Left += 10;
                        DrawDealText(canvas, "Cost     $3,500", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value    $7,000", fontSize, rect_BottomRight);
                        fontSize = bounds.Height * 0.08f;
                        DrawDealText(canvas, "Fish 'N' Cheaps Pet Store", fontSize, rect_TopRight);
                        DrawPets(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.1f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.1f), rect_TopLeft.Width * 0.9f, rect_TopLeft.Height * 0.9f));
                        break;
                    }

                case 24:
                    {
                        rect_TopLeft = SKRect.Create(rect_Top.Left, rect_Top.Top, rect_Top.Width * 0.4f, rect_Top.Height);
                        rect_TopRight = SKRect.Create(rect_Top.Left + rect_TopLeft.Width, rect_Top.Top, rect_Top.Width - rect_TopLeft.Width, rect_Top.Height);
                        DrawDealText(canvas, "Cost      $5,000", fontSize, rect_BottomLeft);
                        DrawDealText(canvas, "Value     $9,000", fontSize, rect_BottomRight);
                        DrawDealText(canvas, "Yum Yum Yogurt", fontSize, rect_TopRight);
                        DrawYogurt(canvas, SKRect.Create(rect_TopLeft.Left + (rect_TopLeft.Width * 0.3f), rect_TopLeft.Top + (rect_TopLeft.Height * 0.2f), rect_TopLeft.Width * 0.5f, rect_TopLeft.Height * 0.7f));
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Only 24 for the index allowed for deals");
                    }
            }
        }
        private void DrawMail(SKCanvas canvas, SKRect bounds)
        {
            switch (Index)
            {
                case 1:
                    {
                        DrawBill(canvas, bounds, "Know-it-All", SKColors.Blue, "University", SKColors.Blue, "You will when you leave", SKColors.Black, "Tuition", SKColors.Red, "Please Pay $5,000", SKColors.Red);
                        break;
                    }

                case 2:
                    {
                        DrawBill(canvas, bounds, "Dr. Feemer", SKColors.Blue, "Super Ski Sunday", SKColors.Black, "Expense:", SKColors.Black, "Set One Broken Leg", SKColors.Black, "Please Pay $300", SKColors.Red);
                        break;
                    }

                case 3:
                    {
                        DrawPollutionCard(canvas, bounds);
                        break;
                    }

                case 4:
                    {
                        DrawMonsterCharge(canvas, bounds, "$1,000", "$100");
                        break;
                    }

                case 5:
                    {
                        DrawMadMoney(canvas, bounds, "Your band played for", SKColors.Black, "the highschool dance.", SKColors.Black, "Collect $300 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 6:
                    {
                        DrawPayANeighbor(canvas, bounds, "Made new living", SKColors.Black, "and dining room", SKColors.Black, "drapes.", SKColors.Black, "$2,000", SKColors.Red);
                        break;
                    }

                case 7:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 8:
                    {
                        DrawBill(canvas, bounds, "Dr. Patella", SKColors.Blue, "New Knee", SKColors.Black, "(You Can Dance Again)", SKColors.Black, "Please Pay $3,000", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 9:
                    {
                        DrawBill(canvas, bounds, "Zapp Electric Co.", SKColors.Blue, "Shocking!", SKColors.Black, "", SKColors.Black, "Please Pay $300", SKColors.Red, "", SKColors.Blue);
                        break;
                    }

                case 10:
                    {
                        DrawPayANeighbor(canvas, bounds, "Made a ", SKColors.Black, "bridesmaid's gown", SKColors.Black, "$2,000.", SKColors.Red, "", SKColors.Red);
                        break;
                    }

                case 11:
                    {
                        DrawMadMoney(canvas, bounds, "Wash & Vacuum", SKColors.Black, "neighbor's cars", SKColors.Black, "Collect $100 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 12:
                    {
                        DrawMonsterCharge(canvas, bounds, "$3,000", "$300");
                        break;
                    }

                case 13:
                    {
                        DrawBill(canvas, bounds, "M. Broider, M.D.", SKColors.Blue, "Ten Stitches", SKColors.Black, "($200 a Stitch)", SKColors.Black, "Please Pay $2,000", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 14:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 15:
                    {
                        DrawPayANeighbor(canvas, bounds, "Piano lessons for one", SKColors.Black, "month for all nine kids", SKColors.Black, "$300", SKColors.Red, "", SKColors.Red);
                        break;
                    }

                case 16:
                    {
                        DrawMadMoney(canvas, bounds, "Cat sitting and dog", SKColors.Black, "walking one month", SKColors.Black, "Collect $400 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 17:
                    {
                        DrawMonsterCharge(canvas, bounds, "$4,000", "$400");
                        break;
                    }

                case 18:
                    {
                        DrawScholarshipCard(canvas, bounds);
                        break;
                    }

                case 19:
                    {
                        DrawBill(canvas, bounds, "Big Noise Inc.", SKColors.Blue, "Installed car", SKColors.Black, "stereo system", SKColors.Black, "Please Pay $700", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 20:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 21:
                    {
                        DrawBill(canvas, bounds, "The Six Tubas", SKColors.Blue, "We played at your", SKColors.Black, "kid's birthday party", SKColors.Black, "Please Pay $300", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 22:
                    {
                        DrawMadMoney(canvas, bounds, "After-school job at", SKColors.Black, "Burgers 'N' Buns", SKColors.Black, "Collect $200 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 23:
                    {
                        DrawPayANeighbor(canvas, bounds, "Snow shoveling for", SKColors.Black, "six blizzards", SKColors.Black, "$200", SKColors.Red, "", SKColors.Red);
                        break;
                    }

                case 24:
                    {
                        DrawEnvironmentCard(canvas, bounds);
                        break;
                    }

                case 25:
                    {
                        DrawBill(canvas, bounds, "Boom Box Music", SKColors.Blue, "Club", SKColors.Blue, "(Went a little", SKColors.Black, "overboard didn't we?)", SKColors.Black, "Please Pay $800", SKColors.Red);
                        break;
                    }

                case 26:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 27:
                    {
                        DrawBill(canvas, bounds, "Dr. I.M. Blurd", SKColors.Blue, "One pair of", SKColors.Black, "designer eyeglasses", SKColors.Black, "Please Pay $200", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 28:
                    {
                        DrawMadMoney(canvas, bounds, "Sang at", SKColors.Black, "friend's wedding", SKColors.Black, "Collect $2,000 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 29:
                    {
                        DrawPayANeighbor(canvas, bounds, "Babysitting the twins", SKColors.Black, "for one long month", SKColors.Black, "$300", SKColors.Red, "", SKColors.Red);
                        break;
                    }

                case 30:
                    {
                        DrawWhaleCard(canvas, bounds);
                        break;
                    }

                case 31:
                    {
                        DrawBill(canvas, bounds, "Mo Larr, D.D.S.", SKColors.Blue, "Fashion braces", SKColors.Black, "Please Pay $1,500", SKColors.Red, "", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 32:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 33:
                    {
                        DrawBill(canvas, bounds, "Pearl E. White,", SKColors.Blue, "D.D.S", SKColors.Blue, "Drilling, filling & billing", SKColors.Black, "Please Pay $100", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 34:
                    {
                        DrawMadMoney(canvas, bounds, "Catered", SKColors.Black, "friend's party", SKColors.Black, "Collect $1,000 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 35:
                    {
                        DrawPayANeighbor(canvas, bounds, "Painted pet portrait", SKColors.Black, "for your Christmas", SKColors.Black, "cards", SKColors.Black, "$300", SKColors.Red);
                        break;
                    }

                case 36:
                    {
                        DrawEndangeredSpeciesCard(canvas, bounds);
                        break;
                    }

                case 37:
                    {
                        DrawMadMoney(canvas, bounds, "Neighbor drove car", SKColors.Black, "onto your front porch", SKColors.Black, "Collect $2,000 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 38:
                    {
                        DrawPayANeighbor(canvas, bounds, "Cut the grass and", SKColors.Black, "raked it, too", SKColors.Black, "$200", SKColors.Red, "", SKColors.Red);
                        break;
                    }

                case 39:
                    {
                        DrawBill(canvas, bounds, "Tick Tock Inc.", SKColors.Blue, "We cleaned your clock", SKColors.Black, "Please Pay $200", SKColors.Red, "", SKColors.Black, "", SKColors.Black);
                        break;
                    }

                case 40:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 41:
                    {
                        DrawBill(canvas, bounds, "Yakity-Yak", SKColors.Blue, "Telephone Co.", SKColors.Blue, "Talked To Cousins", SKColors.Black, " On The Phone", SKColors.Black, "Please Pay $600", SKColors.Red);
                        break;
                    }

                case 42:
                    {
                        DrawBill(canvas, bounds, "Health Club", SKColors.Blue, "Family Membership", SKColors.Blue, "(Includes pets)", SKColors.Black, "Please Pay $1,500", SKColors.Red, "", SKColors.Black);
                        break;
                    }

                case 43:
                    {
                        DrawRecyclingCard(canvas, bounds);
                        break;
                    }

                case 44:
                    {
                        DrawBill(canvas, bounds, "Away We Go", SKColors.Blue, "Trabel Agency", SKColors.Blue, "Two-week vacation", SKColors.Black, "in the sun", SKColors.Black, "Please Pay $2,500", SKColors.Red);
                        break;
                    }

                case 45:
                    {
                        DrawMadMoney(canvas, bounds, "Built doghouse", SKColors.Black, "for neighbor", SKColors.Black, "Collect $1,500 from the", SKColors.Red, "player of your choice", SKColors.Red, "NOW!", SKColors.Red);
                        break;
                    }

                case 46:
                    {
                        DrawMoveAhead(canvas, bounds);
                        break;
                    }

                case 47:
                    {
                        DrawPayANeighbor(canvas, bounds, "", SKColors.Red, "Painted your garage", SKColors.Black, "$1,000", SKColors.Red, "", SKColors.Red);
                        break;
                    }

                default:
                    {
                        throw new Exception("Nothing found for index of " + Index);
                    }
            }
        }
        #region "Deal Graphics"
        private void DrawRocketShip(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top), new SKPoint(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.3f)), true);
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.7f)), new SKPoint(bounds.Left + (bounds.Width * 0.1f), bounds.Top + (bounds.Height * 0.8f)));
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.1f), bounds.Top + bounds.Height), new SKPoint(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.85f)));
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.45f), bounds.Top + (bounds.Height * 0.85f)), new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + bounds.Height));
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.55f), bounds.Top + (bounds.Height * 0.85f)), new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (bounds.Height * 0.85f)));
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.9f), bounds.Top + bounds.Height), new SKPoint(bounds.Left + (bounds.Width * 0.9f), bounds.Top + (bounds.Height * 0.8f)));
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (bounds.Height * 0.7f)), new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (bounds.Height * 0.3f)));
            gp.Close();
            canvas.DrawPath(gp, _slateGrayPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.2f)), new SKPoint(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.4f)), true);
            gp.AddLine(new SKPoint(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.4f)), new SKPoint(bounds.Left + (bounds.Width * 0.6f), bounds.Top + (bounds.Height * 0.4f)));
            gp.Close();
            canvas.DrawPath(gp, _lightBluePaint);
        }
        private void DrawSoda(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKPoint[] pts = new SKPoint[8];
            var br_Fill = _redBrownHatch;
            pts[0] = new SKPoint(bounds.Left + (bounds.Width * 0.45f), bounds.Top);
            pts[1] = new SKPoint(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.3f));
            pts[2] = new SKPoint(bounds.Left + (bounds.Width * 0.35f), bounds.Top + (bounds.Height * 0.4f));
            pts[3] = new SKPoint(bounds.Left + (bounds.Width * 0.3f), bounds.Top + bounds.Height);
            pts[7] = new SKPoint(bounds.Left + (bounds.Width * 0.55f), bounds.Top);
            pts[6] = new SKPoint(bounds.Left + (bounds.Width * 0.6f), bounds.Top + (bounds.Height * 0.3f));
            pts[5] = new SKPoint(bounds.Left + (bounds.Width * 0.65f), bounds.Top + (bounds.Height * 0.4f));
            pts[4] = new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + bounds.Height);
            gp.AddPoly(pts, true);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _border1Paint);
        }
        private void DrawLipstick(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp2; // did not use the hatch on this one afterall.
            SKMatrix tmp_Matrix = new SKMatrix();
            SKPoint pt_Center = new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
            SKMatrix.RotateDegrees(ref tmp_Matrix, -45, pt_Center.X, pt_Center.Y);
            SKPath gp = new SKPath();
            gp.AddRect(SKRect.Create(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.5f), bounds.Width * 0.4f, bounds.Height * 0.5f));
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _bluePaint);
            gp = new SKPath();
            gp.AddRect(SKRect.Create(bounds.Left + (bounds.Width * 0.35f), bounds.Top + (bounds.Height * 0.3f), bounds.Width * 0.3f, bounds.Height * 0.2f));
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _goldPaint);
            gp = new SKPath();
            gp.AddRect(SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.05f), bounds.Width * 0.2f, bounds.Height * 0.25f));
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _redPaint);
            gp2 = new SKPath();
            gp2.AddOval(SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top, bounds.Width * 0.2f, bounds.Height * 0.1f));
            SKMatrix.RotateDegrees(ref tmp_Matrix, 30, bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.05f));
            gp2.Transform(tmp_Matrix);
            SKMatrix.RotateDegrees(ref tmp_Matrix, -45, pt_Center.X, pt_Center.Y);
            gp = new SKPath();
            gp.AddPath(gp2, SKPathAddMode.Append);
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _darkRedPaint);
        }
        private void DrawHippo(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKPoint[] pts = new SKPoint[18];
            var pn_Curve = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Height / 40);
            pts[0] = new SKPoint(bounds.Left + (0.195348837209302f * bounds.Width), bounds.Top + (0.439130434782609f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.190697674418605f * bounds.Width), bounds.Top + (0.152173913043478f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.265116279069767f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.33953488372093f * bounds.Width), bounds.Top + (0.156521739130435f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.427906976744186f * bounds.Width), bounds.Top + (0.134782608695652f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.506976744186047f * bounds.Width), bounds.Top + (0.160869565217391f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.586046511627907f * bounds.Width), bounds.Top + (0.121739130434783f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.683720930232558f * bounds.Width), bounds.Top + (0.195652173913043f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.744186046511628f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.818604651162791f * bounds.Width), bounds.Top + (0.134782608695652f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.8f * bounds.Width), bounds.Top + (0.356521739130435f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.781395348837209f * bounds.Width), bounds.Top + (0.434782608695652f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.621739130434783f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.897674418604651f * bounds.Width), bounds.Top + (0.834782608695652f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.488372093023256f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.106976744186047f * bounds.Width), bounds.Top + (0.908695652173913f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.7f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.0418604651162791f * bounds.Width), bounds.Top + (0.552173913043478f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _slateGrayPaint);
            gp = new SKPath();
            pts = new SKPoint[11];
            pts[0] = new SKPoint(bounds.Left + (0.293023255813954f * bounds.Width), bounds.Top + (0.526086956521739f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.232558139534884f * bounds.Width), bounds.Top + (0.382608695652174f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.344186046511628f * bounds.Width), bounds.Top + (0.21304347826087f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.47906976744186f * bounds.Width), bounds.Top + (0.278260869565217f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.595348837209302f * bounds.Width), bounds.Top + (0.195652173913043f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.716279069767442f * bounds.Width), bounds.Top + (0.304347826086957f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.683720930232558f * bounds.Width), bounds.Top + (0.504347826086957f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.511627906976744f * bounds.Width), bounds.Top + (0.521739130434783f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.474418604651163f * bounds.Width), bounds.Top + (0.478260869565217f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.451162790697674f * bounds.Width), bounds.Top + (0.51304347826087f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.348837209302326f * bounds.Width), bounds.Top + (0.517391304347826f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _aquaPaint);
            canvas.DrawOval(SKRect.Create(bounds.Left + (0.38f * bounds.Width), bounds.Top + (0.39f * bounds.Height), bounds.Width / 10, bounds.Width / 10), _blackPaint);
            canvas.DrawOval(SKRect.Create(bounds.Left + (0.506976744186047f * bounds.Width), bounds.Top + (0.421739130434783f * bounds.Height), bounds.Width / 10, bounds.Width / 10), _blackPaint);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.153488372093023f * bounds.Width), bounds.Top + (0.81304347826087f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.227906976744186f * bounds.Width), bounds.Top + (0.821739130434783f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.237209302325581f * bounds.Width), bounds.Top + (0.908695652173913f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.162790697674419f * bounds.Width), bounds.Top + (0.9f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.725581395348837f * bounds.Width), bounds.Top + (0.839130434782609f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.730232558139535f * bounds.Width), bounds.Top + (0.895652173913044f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.790697674418605f * bounds.Width), bounds.Top + (0.88695652173913f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.795348837209302f * bounds.Width), bounds.Top + (0.839130434782609f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, _border1Paint);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.0930232558139535f * bounds.Width), bounds.Top + (0.760869565217391f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.506976744186047f * bounds.Width), bounds.Top + (0.878260869565217f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.906976744186046f * bounds.Width), bounds.Top + (0.721739130434783f * bounds.Height));
            gp.AddLines(pts, false);
            canvas.DrawPath(gp, pn_Curve);
        }
        private void DrawComedy(SKCanvas canvas, SKRect bounds)
        {
            var rect = SKRect.Create(bounds.Left + (bounds.Width / 4), bounds.Top + (bounds.Height / 2), bounds.Width / 2, bounds.Height * 0.4f);
            SKPoint[] pts = new SKPoint[6];
            SKPath gp = new SKPath();
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 20);
            var br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LightYellow, SKColors.DarkGoldenrod, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawOval(bounds, br_Fill);
            pts[0] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.05f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.519230769230769f * rect.Width), rect.Top + (0.4f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0 * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.875f * rect.Width), rect.Top + (0.5125f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.548076923076923f * rect.Width), rect.Top + (1 * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.221153846153846f * rect.Width), rect.Top + (0.5f * rect.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, pn_Temp);
            canvas.DrawLine(new SKPoint(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.4f)), new SKPoint(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.3f)), pn_Temp);
            canvas.DrawLine(new SKPoint(bounds.Left + (bounds.Width * 0.6f), bounds.Top + (bounds.Height * 0.4f)), new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (bounds.Height * 0.3f)), pn_Temp);
        }
        private void DrawJewelry(SKCanvas canvas, SKRect bounds) // could be iffy because not sure about intersecting and excluding from regions.
        {
            var br_Fill = _silverWhiteHatch;
            SKPath gp1 = new SKPath();
            SKPath gp2 = new SKPath();
            SKPath gp3 = new SKPath();
            SKRegion reg_Temp;
            SKPoint[] pts = new SKPoint[11];
            pts[0] = new SKPoint(bounds.Left + (0.320675105485232f * bounds.Width), bounds.Top + (0.14070351758794f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.118143459915612f * bounds.Width), bounds.Top + (0.35678391959799f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.64321608040201f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.00843881856540084f * bounds.Width), bounds.Top + (0.809045226130653f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.160337552742616f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.350210970464135f * bounds.Width), bounds.Top + (0.994974874371859f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.683544303797468f * bounds.Width), bounds.Top + (0.819095477386935f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.978902953586498f * bounds.Width), bounds.Top + (0.422110552763819f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.185929648241206f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.877637130801688f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.493670886075949f * bounds.Width), bounds.Top + (0.0351758793969849f * bounds.Height));
            gp1.AddLines(pts, true);
            pts = new SKPoint[8];
            pts[0] = new SKPoint(bounds.Left + (0.206751054852321f * bounds.Width), bounds.Top + (0.407563025210084f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.405063291139241f * bounds.Width), bounds.Top + (0.457983193277311f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.206751054852321f * bounds.Width), bounds.Top + (0.735294117647059f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.223628691983122f * bounds.Width), bounds.Top + (0.920168067226891f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.472573839662447f * bounds.Width), bounds.Top + (0.865546218487395f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.890295358649789f * bounds.Width), bounds.Top + (0.558823529411765f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.881856540084388f * bounds.Width), bounds.Top + (0.340336134453782f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.578059071729958f * bounds.Width), bounds.Top + (0.373949579831933f * bounds.Height));
            gp2.AddLines(pts, true);
            pts[0] = new SKPoint(bounds.Left + (0.101503759398496f * bounds.Width), bounds.Top + (0.626865671641791f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.233082706766917f * bounds.Width), bounds.Top + (0.712686567164179f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.387218045112782f * bounds.Width), bounds.Top + (0.682835820895522f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.699248120300752f * bounds.Width), bounds.Top + (0.492537313432836f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.785714285714286f * bounds.Width), bounds.Top + (0.227611940298507f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.488805970149254f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.206766917293233f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.0827067669172932f * bounds.Width), bounds.Top + (0.861940298507463f * bounds.Height));
            gp3.AddLines(pts, true);
            reg_Temp = new SKRegion();
            reg_Temp.SetPath(gp2);
            reg_Temp.Intersect(gp3);
            canvas.DrawRegion(reg_Temp, _darkGoldenRodPaint);
            reg_Temp = new SKRegion();
            reg_Temp.SetPath(gp1);
            reg_Temp.Exclude(gp2); // could be iffy.  hopefully can figure out what is needed here.
            canvas.DrawRegion(reg_Temp, _yellowPaint);
            gp1 = new SKPath();
            pts = new SKPoint[17];
            pts[0] = new SKPoint(bounds.Left + (0.232067510548523f * bounds.Width), bounds.Top + (0.457983193277311f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.274261603375527f * bounds.Width), bounds.Top + (0.504201680672269f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.341772151898734f * bounds.Width), bounds.Top + (0.432773109243698f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.409282700421941f * bounds.Width), bounds.Top + (0.470588235294118f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.447257383966245f * bounds.Width), bounds.Top + (0.390756302521008f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.573839662447257f * bounds.Width), bounds.Top + (0.378151260504202f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.540084388185654f * bounds.Width), bounds.Top + (0.281512605042017f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.556962025316456f * bounds.Width), bounds.Top + (0.180672268907563f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.510548523206751f * bounds.Width), bounds.Top + (0.117647058823529f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.443037974683544f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.388185654008439f * bounds.Width), bounds.Top + (0.0378151260504202f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.261603375527426f * bounds.Width), bounds.Top + (0.0798319327731092f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.206751054852321f * bounds.Width), bounds.Top + (0.15546218487395f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.130801687763713f * bounds.Width), bounds.Top + (0.247899159663866f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.143459915611814f * bounds.Width), bounds.Top + (0.315126050420168f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.109704641350211f * bounds.Width), bounds.Top + (0.373949579831933f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.206751054852321f * bounds.Width), bounds.Top + (0.407563025210084f * bounds.Height));
            gp1.AddLines(pts, true);
            canvas.DrawPath(gp1, br_Fill);
            canvas.DrawPath(gp1, _silverBorder);
        }
        private void DrawPlane(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[18];
            SKPath gp = new SKPath();
            pts[0] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.5f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.921630094043887f * bounds.Width), bounds.Top + (0.427536231884058f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.673981191222571f * bounds.Width), bounds.Top + (0.402173913043478f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.554858934169279f * bounds.Width), bounds.Top + (0.0108695652173913f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.426332288401254f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.423197492163009f * bounds.Width), bounds.Top + (0.402173913043478f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.178683385579937f * bounds.Width), bounds.Top + (0.402173913043478f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.106583072100313f * bounds.Width), bounds.Top + (0.268115942028986f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.0125391849529781f * bounds.Width), bounds.Top + (0.264492753623188f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.0344827586206897f * bounds.Width), bounds.Top + (0.510869565217391f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.742753623188406f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.119122257053292f * bounds.Width), bounds.Top + (0.739130434782609f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.172413793103448f * bounds.Width), bounds.Top + (0.605072463768116f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.423197492163009f * bounds.Width), bounds.Top + (0.597826086956522f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.426332288401254f * bounds.Width), bounds.Top + (0.996376811594203f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.539184952978056f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.664576802507837f * bounds.Width), bounds.Top + (0.594202898550725f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.915360501567398f * bounds.Width), bounds.Top + (0.576086956521739f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _brownPaint);
            canvas.DrawPath(gp, _border1Paint);
        }
        private void DrawBurger(SKCanvas canvas, SKRect bounds)
        {
            var rect1 = SKRect.Create(bounds.Left, bounds.Top, bounds.Width, (bounds.Height * 5) / 12);
            var rect3 = SKRect.Create(bounds.Left, bounds.Top + ((bounds.Height * 4) / 6), bounds.Width, (bounds.Height / 4));
            var rect2 = SKRect.Create(bounds.Left, bounds.Top + rect1.Height, bounds.Width, bounds.Height / 3);
            SKPaint br_Fill;
            SKPath gp = new SKPath();
            gp.AddArc(SKRect.Create(rect1.Left, rect1.Top, rect1.Width, rect1.Height * 0.8f), 180, 180);
            gp.AddLine(rect1.Left + rect1.Width, rect2.Top, rect1.Left, rect2.Top);
            gp.Close();
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.BurlyWood, SKColors.Brown, rect1, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            gp.AddArc(rect3, 180, -180);
            gp.AddLine(rect1.Left + rect1.Width, rect3.Top, rect1.Left, rect3.Top);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Brown, SKColors.BurlyWood, rect3, MiscHelpers.EnumLinearGradientPercent.Angle135);
            canvas.DrawPath(gp, br_Fill);
            rect1 = SKRect.Create(rect2.Left, rect2.Top, rect2.Width, rect2.Height / 6);
            canvas.DrawRect(rect1, _limeGreenPaint);
            rect1 = SKRect.Create(rect2.Left + (rect2.Width * 0.1f), rect2.Top + (rect2.Height / 6), rect2.Width * 0.8f, rect2.Height / 6);
            canvas.DrawRect(rect1, _redPaint);
            rect1 = SKRect.Create(rect2.Left, rect2.Top + (rect2.Height / 3), rect2.Width, rect2.Height / 6);
            canvas.DrawRect(rect1, _yellowPaint);
            rect1 = SKRect.Create(rect2.Left, rect2.Top + (rect2.Height / 2), rect2.Width, rect2.Height / 2);
            canvas.DrawRect(rect1, _darkRedPaint);
        }
        private void DrawLimo(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKPoint[] pts = new SKPoint[11];
            SKRect rect;
            pts[0] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.912087912087912f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.991735537190083f * bounds.Width), bounds.Top + (0.527472527472527f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.931818181818182f * bounds.Width), bounds.Top + (0.406593406593407f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.824380165289256f * bounds.Width), bounds.Top + (0.395604395604396f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.743801652892562f * bounds.Width), bounds.Top + (0.021978021978022f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.18801652892562f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.121900826446281f * bounds.Width), bounds.Top + (0.428571428571429f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.0495867768595041f * bounds.Width), bounds.Top + (0.43956043956044f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.703296703296703f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.997933884297521f * bounds.Width), bounds.Top + (0.967032967032967f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, _silverBorder);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.225206611570248f * bounds.Width), bounds.Top + (0.175824175824176f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.332644628099174f * bounds.Width), bounds.Top + (0.175824175824176f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.328512396694215f * bounds.Width), bounds.Top + (0.362637362637363f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.194214876033058f * bounds.Width), bounds.Top + (0.373626373626374f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.357438016528926f * bounds.Width), bounds.Top + (0.164835164835165f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.355371900826446f * bounds.Width), bounds.Top + (0.362637362637363f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.597107438016529f * bounds.Width), bounds.Top + (0.373626373626374f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.597107438016529f * bounds.Width), bounds.Top + (0.175824175824176f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.638429752066116f * bounds.Width), bounds.Top + (0.175824175824176f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.638429752066116f * bounds.Width), bounds.Top + (0.373626373626374f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.727272727272727f * bounds.Width), bounds.Top + (0.362637362637363f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.68801652892562f * bounds.Width), bounds.Top + (0.186813186813187f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
            rect = SKRect.Create(bounds.Left + (0.111570247933884f * bounds.Width), bounds.Top + (0.714285714285714f * bounds.Height), bounds.Height * 0.5f, bounds.Height * 0.6f);
            canvas.DrawOval(rect, _blackPaint);
            rect = SKRect.Create(rect.Left + (rect.Width / 4), rect.Top + (rect.Height / 4), rect.Width / 2, rect.Height / 2);
            canvas.DrawOval(rect, _whitePaint);
            rect = SKRect.Create(bounds.Left + (0.824380165289256f * bounds.Width), bounds.Top + (0.703296703296703f * bounds.Height), bounds.Height * 0.5f, bounds.Height * 0.6f);
            canvas.DrawOval(rect, _blackPaint);
            rect = SKRect.Create(rect.Left + (rect.Width / 4), rect.Top + (rect.Height / 4), rect.Width / 2, rect.Height / 2);
            canvas.DrawOval(rect, _whitePaint);
        }
        private void DrawNoodle(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            var br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Yellow, SKColors.DarkGoldenrod, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            int int_Count;
            SKRect rect;
            gp.AddLine(new SKPoint(bounds.Left, bounds.Top + (bounds.Height / 2)), new SKPoint(bounds.Left + (bounds.Width), bounds.Top), true);
            gp.AddLine(new SKPoint(bounds.Left + bounds.Width, bounds.Top + (bounds.Height / 2)), new SKPoint(bounds.Left, bounds.Top + bounds.Height));
            gp.Close();
            canvas.DrawPath(gp, br_Fill);
            for (int_Count = 0; int_Count <= 5; int_Count++)
            {
                rect = SKRect.Create(bounds.Left + ((bounds.Width / 6) * int_Count), bounds.Top - ((int_Count - 5) * (bounds.Width / 12)) - (bounds.Height / 10), bounds.Width / 10, (bounds.Height / 2) + (bounds.Height / 5));
                canvas.DrawOval(rect, _goldPaint);
            }
        }
        private void DrawNoodles(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            rect = SKRect.Create(bounds.Left, bounds.Top, bounds.Width * 0.7f, bounds.Height * 0.7f);
            DrawNoodle(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.3f), bounds.Width * 0.7f, bounds.Height * 0.7f);
            DrawNoodle(canvas, rect);
        }
        private void DrawSafari(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            SKPath gp2;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 25);
            SKRect rect;
            gp = new SKPath();
            pts = new SKPoint[12];
            pts[0] = new SKPoint(bounds.Left + (0.170347003154574f * bounds.Width), bounds.Top + (0.565040650406504f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.252365930599369f * bounds.Width), bounds.Top + (0.845528455284553f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.347003154574133f * bounds.Width), bounds.Top + (0.691056910569106f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.353312302839117f * bounds.Width), bounds.Top + (0.796747967479675f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.435331230283912f * bounds.Width), bounds.Top + (0.821138211382114f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.561514195583596f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.71608832807571f * bounds.Width), bounds.Top + (0.995934959349594f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.779179810725552f * bounds.Width), bounds.Top + (0.878048780487805f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.842271293375394f * bounds.Width), bounds.Top + (0.971544715447154f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.892744479495268f * bounds.Width), bounds.Top + (0.861788617886179f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.807570977917981f * bounds.Width), bounds.Top + (0.613821138211382f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.813880126182965f * bounds.Width), bounds.Top + (0.382113821138211f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            gp2 = new SKPath();
            pts = new SKPoint[6];
            pts[0] = new SKPoint(bounds.Left + (0.362776025236593f * bounds.Width), bounds.Top + (0.536585365853659f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.347003154574133f * bounds.Width), bounds.Top + (0.768292682926829f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.429022082018927f * bounds.Width), bounds.Top + (0.808943089430894f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.457413249211356f * bounds.Width), bounds.Top + (0.857723577235772f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.618296529968454f * bounds.Width), bounds.Top + (0.670731707317073f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.608832807570978f * bounds.Width), bounds.Top + (0.451219512195122f * bounds.Height));
            gp2.AddLines(pts, true);
            canvas.DrawPath(gp2, _brownPaint);
            canvas.DrawPath(gp, pn_Temp);
            canvas.DrawOval(SKRect.Create(bounds.Left + (0.466876971608833f * bounds.Width), bounds.Top + (0.51219512195122f * bounds.Height), bounds.Width / 5, bounds.Height / 5), _blackPaint);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.675078864353312f * bounds.Width), bounds.Top + (0.528455284552846f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.750788643533123f * bounds.Width), bounds.Top + (0.605691056910569f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.772870662460568f * bounds.Width), bounds.Top + (0.898373983739837f * bounds.Height));
            var NewPath = new SKPath();
            NewPath.AddLines(pts, false);
            canvas.DrawPath(NewPath, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[6];
            pts[0] = new SKPoint(bounds.Left + (0.744479495268139f * bounds.Width), bounds.Top + (0.642276422764228f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.798107255520505f * bounds.Width), bounds.Top + (0.658536585365854f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.861198738170347f * bounds.Width), bounds.Top + (0.609756097560976f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.867507886435331f * bounds.Width), bounds.Top + (0.699186991869919f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.801261829652997f * bounds.Width), bounds.Top + (0.821138211382114f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.690851735015773f * bounds.Width), bounds.Top + (0.74390243902439f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _darkGrayPaint);
            canvas.DrawPath(gp, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[7];
            pts[0] = new SKPoint(bounds.Left + (0.457413249211356f * bounds.Width), bounds.Top + (0.17479674796748f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.151419558359621f * bounds.Width), bounds.Top + (0.33739837398374f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.536585365853659f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.157728706624606f * bounds.Width), bounds.Top + (0.613821138211382f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.589905362776025f * bounds.Width), bounds.Top + (0.548780487804878f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.296747967479675f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.899053627760252f * bounds.Width), bounds.Top + (0.121951219512195f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.214511041009464f * bounds.Width), bounds.Top + (0.394308943089431f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.444794952681388f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.82018927444795f * bounds.Width), bounds.Top + (0.284552845528455f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.498422712933754f * bounds.Width), bounds.Top + (0.443089430894309f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, pn_Temp);
            canvas.DrawOval(SKRect.Create(bounds.Left + (0.621451104100946f * bounds.Width), bounds.Top + (0.849593495934959f * bounds.Height), bounds.Width / 15, bounds.Width / 15), _blackPaint);
            canvas.DrawOval(SKRect.Create(bounds.Left + (0.646687697160883f * bounds.Width), bounds.Top + (0.886178861788618f * bounds.Height), bounds.Width / 15, bounds.Width / 15), _blackPaint);
            canvas.DrawOval(SKRect.Create(bounds.Left + (0.681388012618297f * bounds.Width), bounds.Top + (0.845528455284553f * bounds.Height), bounds.Width / 15, bounds.Width / 15), _blackPaint);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.463722397476341f * bounds.Width), bounds.Top + (0.0121951219512195f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.375394321766562f * bounds.Width), bounds.Top + (0.252032520325203f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.324921135646688f * bounds.Width), bounds.Top + (0.439024390243902f * bounds.Height));
            NewPath = new SKPath();
            NewPath.AddLines(pts, false);
            canvas.DrawPath(NewPath, pn_Temp);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.473186119873817f * bounds.Width), bounds.Top + (0.024390243902439f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.498422712933754f * bounds.Width), bounds.Top + (0.239837398373984f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.498422712933754f * bounds.Width), bounds.Top + (0.439024390243902f * bounds.Height));
            NewPath = new SKPath();
            NewPath.AddLines(pts, false);
            canvas.DrawPath(NewPath, pn_Temp);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.470031545741325f * bounds.Width), bounds.Top + (0.016260162601626f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.627760252365931f * bounds.Width), bounds.Top + (0.158536585365854f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.703470031545741f * bounds.Width), bounds.Top + (0.365853658536585f * bounds.Height));
            NewPath = new SKPath();
            NewPath.AddLines(pts, false);
            canvas.DrawPath(NewPath, pn_Temp);
            rect = SKRect.Create(bounds.Left + (0.4f * bounds.Width), bounds.Top - (bounds.Height / 20), bounds.Width / 10, bounds.Height / 10);
            canvas.DrawOval(rect, _whitePaint);
            canvas.DrawOval(rect, pn_Temp);
        }
        private void DrawPizza(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts = new SKPoint[21];
            SKPath gp = new SKPath();
            SKRect rect;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.SaddleBrown, bounds.Width / 12);
            var br_Fill = _redYellowHatch;
            rect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height / 3), bounds.Width, (bounds.Height * 2) / 3);
            canvas.DrawOval(rect, br_Fill);
            canvas.DrawOval(rect, pn_Temp);
            rect = SKRect.Create(bounds.Left + (bounds.Width / 8), bounds.Top, (bounds.Width * 4) / 5, bounds.Height / 3);
            pts[0] = new SKPoint(rect.Left + (0.164893617021277f * rect.Width), rect.Top + (0.975609756097561f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.159574468085106f * rect.Width), rect.Top + (0.780487804878049f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.414634146341463f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.212765957446809f * rect.Width), rect.Top + (0.695121951219512f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.265957446808511f * rect.Width), rect.Top + (0.621951219512195f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.287234042553192f * rect.Width), rect.Top + (0.231707317073171f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.361702127659574f * rect.Width), rect.Top + (0.597560975609756f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.446808510638298f * rect.Width), rect.Top + (0.597560975609756f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.473404255319149f * rect.Width), rect.Top + (0.341463414634146f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.430851063829787f * rect.Width), rect.Top + (0.280487804878049f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.515957446808511f * rect.Width), rect.Top + (0 * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.622340425531915f * rect.Width), rect.Top + (0.182926829268293f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.563829787234043f * rect.Width), rect.Top + (0.292682926829268f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.569148936170213f * rect.Width), rect.Top + (0.621951219512195f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.691489361702128f * rect.Width), rect.Top + (0.621951219512195f * rect.Height));
            pts[15] = new SKPoint(rect.Left + (0.728723404255319f * rect.Width), rect.Top + (0.219512195121951f * rect.Height));
            pts[16] = new SKPoint(rect.Left + (0.765957446808511f * rect.Width), rect.Top + (0.585365853658537f * rect.Height));
            pts[17] = new SKPoint(rect.Left + (0.829787234042553f * rect.Width), rect.Top + (0.682926829268293f * rect.Height));
            pts[18] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.402439024390244f * rect.Height));
            pts[19] = new SKPoint(rect.Left + (0.893617021276596f * rect.Width), rect.Top + (0.841463414634146f * rect.Height));
            pts[20] = new SKPoint(rect.Left + (0.888297872340426f * rect.Width), rect.Top + (1 * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _goldPaint);
            canvas.DrawPath(gp, _border1Paint);
        }
        private void DrawGolf(SKCanvas canvas, SKRect bounds)
        {
            var br_Fill = _lightGrayWhiteHatch;
            canvas.DrawOval(bounds, br_Fill);
            canvas.DrawOval(bounds, _lightGrayBorder);
        }
        private void DrawJeans(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            SKPaint br_Fill;
            gp = new SKPath();
            pts = new SKPoint[6];
            pts[0] = new SKPoint(bounds.Left + (0.00917431192660551f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.336244541484716f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.0871559633027523f * bounds.Width), bounds.Top + (0.646288209606987f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.637614678899083f * bounds.Width), bounds.Top + (0.716157205240175f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.743119266055046f * bounds.Width), bounds.Top + (0.323144104803493f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.660550458715596f * bounds.Width), bounds.Top + (0.00873362445414847f * bounds.Height));
            gp.AddLines(pts, true);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LightBlue, SKColors.DarkBlue, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.0779816513761468f * bounds.Width), bounds.Top + (0.602620087336245f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.334862385321101f * bounds.Width), bounds.Top + (0.449781659388646f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.463302752293578f * bounds.Width), bounds.Top + (0.724890829694323f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.0779816513761468f * bounds.Width), bounds.Top + (0.624454148471616f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _darkBluePaint);
            gp = new SKPath();
            pts = new SKPoint[5];
            pts[0] = new SKPoint(bounds.Left + (0.31651376146789f * bounds.Width), bounds.Top + (0.480349344978166f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.330275229357798f * bounds.Width), bounds.Top + (0.934497816593886f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.857798165137615f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.589519650655022f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.784403669724771f * bounds.Width), bounds.Top + (0.449781659388646f * bounds.Height));
            gp.AddPoly(pts);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LightBlue, SKColors.DarkBlue, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.00869565217391304f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.648401826484018f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.671232876712329f * bounds.Width), bounds.Top + (0.0478260869565217f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.0045662100456621f * bounds.Width), bounds.Top + (0.0652173913043478f * bounds.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
        }
        private void DrawShephardsPie(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.SaddleBrown, bounds.Width / 20);
            var rect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height / 2), bounds.Width, bounds.Height / 2);
            var rect2 = SKRect.Create(bounds.Left, bounds.Top, bounds.Width, bounds.Height / 2);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(rect.Left + (0.0515873015873016f * rect.Width), rect.Top + (0.547169811320755f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.138888888888889f * rect.Width), rect.Top + (0.990566037735849f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.825396825396825f * rect.Width), rect.Top + (1 * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.956349206349206f * rect.Width), rect.Top + (0.481132075471698f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _slateGrayPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[8];
            pts[0] = new SKPoint(rect.Left + (0.48015873015873f * rect.Width), rect.Top + (0.150943396226415f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.0912698412698413f * rect.Width), rect.Top + (0.19811320754717f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.443396226415094f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.0992063492063492f * rect.Width), rect.Top + (0.688679245283019f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.472222222222222f * rect.Width), rect.Top + (0.745283018867924f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.936507936507937f * rect.Width), rect.Top + (0.575471698113208f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.349056603773585f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.888888888888889f * rect.Width), rect.Top + (0.179245283018868f * rect.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _brownPaint);
            canvas.DrawPath(gp, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[8];
            pts[0] = new SKPoint(rect.Left + (0.0714285714285714f * rect.Width), rect.Top + (0.433962264150943f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.158730158730159f * rect.Width), rect.Top + (0.160377358490566f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.452380952380952f * rect.Width), rect.Top + (0 * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.825396825396825f * rect.Width), rect.Top + (0.169811320754717f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.916666666666667f * rect.Width), rect.Top + (0.367924528301887f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.817460317460317f * rect.Width), rect.Top + (0.518867924528302f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.46031746031746f * rect.Width), rect.Top + (0.575471698113208f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.162698412698413f * rect.Width), rect.Top + (0.556603773584906f * rect.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _sandyBrownPaint);
            canvas.DrawPath(gp, pn_Temp);
            pts = new SKPoint[7];
            pts[0] = new SKPoint(rect2.Left + (0.262357414448669f * rect2.Width), rect2.Top + (0.991452991452991f * rect2.Height));
            pts[1] = new SKPoint(rect2.Left + (0.250950570342205f * rect2.Width), rect2.Top + (0.786324786324786f * rect2.Height));
            pts[2] = new SKPoint(rect2.Left + (0.368821292775665f * rect2.Width), rect2.Top + (0.521367521367521f * rect2.Height));
            pts[3] = new SKPoint(rect2.Left + (0.304182509505703f * rect2.Width), rect2.Top + (0.376068376068376f * rect2.Height));
            pts[4] = new SKPoint(rect2.Left + (0.311787072243346f * rect2.Width), rect2.Top + (0.205128205128205f * rect2.Height));
            pts[5] = new SKPoint(rect2.Left + (0.414448669201521f * rect2.Width), rect2.Top + (0.0854700854700855f * rect2.Height));
            pts[6] = new SKPoint(rect2.Left + (0.372623574144487f * rect2.Width), rect2.Top + (0.00854700854700855f * rect2.Height));
            canvas.DrawLines(pts, _border1Paint);
            pts = new SKPoint[5];
            pts[0] = new SKPoint(rect2.Left + (0.47148288973384f * rect2.Width), rect2.Top + (0.811965811965812f * rect2.Height));
            pts[1] = new SKPoint(rect2.Left + (0.482889733840304f * rect2.Width), rect2.Top + (0.572649572649573f * rect2.Height));
            pts[2] = new SKPoint(rect2.Left + (0.532319391634981f * rect2.Width), rect2.Top + (0.538461538461538f * rect2.Height));
            pts[3] = new SKPoint(rect2.Left + (0.543726235741445f * rect2.Width), rect2.Top + (0.196581196581197f * rect2.Height));
            pts[4] = new SKPoint(rect2.Left + (0.615969581749049f * rect2.Width), rect2.Top + (0.0854700854700855f * rect2.Height));
            canvas.DrawLines(pts, _border1Paint);
            pts = new SKPoint[6];
            pts[0] = new SKPoint(rect2.Left + (0.631178707224335f * rect2.Width), rect2.Top + (0.897435897435897f * rect2.Height));
            pts[1] = new SKPoint(rect2.Left + (0.673003802281369f * rect2.Width), rect2.Top + (0.811965811965812f * rect2.Height));
            pts[2] = new SKPoint(rect2.Left + (0.638783269961977f * rect2.Width), rect2.Top + (0.606837606837607f * rect2.Height));
            pts[3] = new SKPoint(rect2.Left + (0.733840304182509f * rect2.Width), rect2.Top + (0.435897435897436f * rect2.Height));
            pts[4] = new SKPoint(rect2.Left + (0.699619771863118f * rect2.Width), rect2.Top + (0.247863247863248f * rect2.Height));
            pts[5] = new SKPoint(rect2.Left + (0.749049429657795f * rect2.Width), rect2.Top + (0.102564102564103f * rect2.Height));
            canvas.DrawLines(pts, _border1Paint);
        }
        private void DrawBallet(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Silver, bounds.Width / 20);
            SKRect rect;
            canvas.DrawRoundRect(bounds, bounds.Width / 10, bounds.Width / 10, _lightBluePaint);
            canvas.DrawRoundRect(bounds, bounds.Width / 10, bounds.Width / 10, pn_Temp);
            rect = SKRect.Create(bounds.Left + (bounds.Width / 3), bounds.Top + (bounds.Height * 0.1f), bounds.Width / 3, bounds.Height * 0.8f);
            gp = new SKPath();
            pts = new SKPoint[7];
            pts[0] = new SKPoint(rect.Left + (0.05f * rect.Width), rect.Top + (0.0160427807486631f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.1f * rect.Width), rect.Top + (0.13903743315508f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.15f * rect.Width), rect.Top + (0.251336898395722f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.158415841584158f * rect.Width), rect.Top + (0.427807486631016f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.277227722772277f * rect.Width), rect.Top + (0.732620320855615f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.663366336633663f * rect.Width), rect.Top + (0.245989304812834f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.574257425742574f * rect.Width), rect.Top + (0 * rect.Height));
            gp.AddPoly(pts, true);
            canvas.DrawPath(gp, _peachPuffPaint);
            canvas.DrawPath(gp, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[10];
            pts[0] = new SKPoint(rect.Left + (0.653465346534653f * rect.Width), rect.Top + (0.0802139037433155f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.262032085561497f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.99009900990099f * rect.Width), rect.Top + (0.374331550802139f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.564356435643564f * rect.Width), rect.Top + (0.631016042780749f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.356435643564356f * rect.Width), rect.Top + (0.983957219251337f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.188118811881188f * rect.Width), rect.Top + (1 * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.0297029702970297f * rect.Width), rect.Top + (0.978609625668449f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.695187165775401f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.217821782178218f * rect.Width), rect.Top + (0.577540106951872f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.603960396039604f * rect.Width), rect.Top + (0.0855614973262032f * rect.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, pn_Temp);
        }
        private void DrawMusic(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            gp = new SKPath();
            pts = new SKPoint[17];
            pts[0] = new SKPoint(bounds.Left + (0.267857142857143f * bounds.Width), bounds.Top + (0.761658031088083f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.107142857142857f * bounds.Width), bounds.Top + (0.761658031088083f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.865284974093264f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.130952380952381f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.351190476190476f * bounds.Width), bounds.Top + (0.875647668393782f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.321428571428571f * bounds.Width), bounds.Top + (0.523316062176166f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.547619047619048f * bounds.Width), bounds.Top + (0.492227979274611f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.81547619047619f * bounds.Width), bounds.Top + (0.362694300518135f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.85f * bounds.Width), bounds.Top + (0.632124352331606f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.773809523809524f * bounds.Width), bounds.Top + (0.55f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.630952380952381f * bounds.Width), bounds.Top + (0.725388601036269f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.791666666666667f * bounds.Width), bounds.Top + (0.865284974093264f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.647668393782383f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.952380952380952f * bounds.Width), bounds.Top + (0.455958549222798f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.81547619047619f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.511904761904762f * bounds.Width), bounds.Top + (0.170984455958549f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.172619047619048f * bounds.Width), bounds.Top + (0.207253886010363f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _blackPaint);
            gp = new SKPath();
            pts = new SKPoint[6];
            pts[0] = new SKPoint(bounds.Left + (0.267857142857143f * bounds.Width), bounds.Top + (0.238341968911917f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.31547619047619f * bounds.Width), bounds.Top + (0.409326424870466f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.541666666666667f * bounds.Width), bounds.Top + (0.409326424870466f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.773809523809524f * bounds.Width), bounds.Top + (0.274611398963731f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.755952380952381f * bounds.Width), bounds.Top + (0.10880829015544f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.523809523809524f * bounds.Width), bounds.Top + (0.238341968911917f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _goldPaint);
        }
        private void DrawSkateboard(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            gp = new SKPath();
            pts = new SKPoint[11];
            pts[0] = new SKPoint(bounds.Left + (0.945701357466063f * bounds.Width), bounds.Top + (0.0850202429149798f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.805429864253394f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.678733031674208f * bounds.Width), bounds.Top + (0.0647773279352227f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.497737556561086f * bounds.Width), bounds.Top + (0.417004048582996f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.239819004524887f * bounds.Width), bounds.Top + (0.663967611336032f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.781376518218624f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.00452488687782805f * bounds.Width), bounds.Top + (0.82995951417004f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.104072398190045f * bounds.Width), bounds.Top + (0.955465587044534f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.294117647058824f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.719457013574661f * bounds.Width), bounds.Top + (0.59919028340081f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.914027149321267f * bounds.Width), bounds.Top + (0.291497975708502f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _limeGreenPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[13];
            pts[0] = new SKPoint(bounds.Left + (0.647058823529412f * bounds.Width), bounds.Top + (0.0566801619433198f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.606334841628959f * bounds.Width), bounds.Top + (0.153846153846154f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.67420814479638f * bounds.Width), bounds.Top + (0.263157894736842f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.755656108597285f * bounds.Width), bounds.Top + (0.230769230769231f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.877828054298643f * bounds.Width), bounds.Top + (0.311740890688259f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.832579185520362f * bounds.Width), bounds.Top + (0.384615384615385f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.95475113122172f * bounds.Width), bounds.Top + (0.392712550607287f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.287449392712551f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.936651583710407f * bounds.Width), bounds.Top + (0.218623481781377f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.891402714932127f * bounds.Width), bounds.Top + (0.267206477732794f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.778280542986425f * bounds.Width), bounds.Top + (0.178137651821862f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.773755656108597f * bounds.Width), bounds.Top + (0.117408906882591f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.69683257918552f * bounds.Width), bounds.Top + (0.048582995951417f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _bluePaint);
            gp = new SKPath();
            pts = new SKPoint[12];
            pts[0] = new SKPoint(bounds.Left + (0.257918552036199f * bounds.Width), bounds.Top + (0.607287449392713f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.221719457013575f * bounds.Width), bounds.Top + (0.728744939271255f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.3710407239819f * bounds.Width), bounds.Top + (0.744939271255061f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.438914027149321f * bounds.Width), bounds.Top + (0.801619433198381f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.411764705882353f * bounds.Width), bounds.Top + (0.874493927125506f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.497737556561086f * bounds.Width), bounds.Top + (0.939271255060729f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.606334841628959f * bounds.Width), bounds.Top + (0.8582995951417f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.547511312217195f * bounds.Width), bounds.Top + (0.765182186234818f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.470588235294118f * bounds.Width), bounds.Top + (0.773279352226721f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.411764705882353f * bounds.Width), bounds.Top + (0.700404858299595f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.393665158371041f * bounds.Width), bounds.Top + (0.595141700404858f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.285067873303167f * bounds.Width), bounds.Top + (0.595141700404858f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _bluePaint);
        }
        private void DrawMissMuffet(SKCanvas canvas, SKRect bounds)
        {
            SKPoint[] pts;
            SKPath gp;
            gp = new SKPath();
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.744939271255061f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.51063829787234f * bounds.Width), bounds.Top + (0.781376518218624f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.148936170212766f * bounds.Width), bounds.Top + bounds.Height);
            gp.AddLines(pts);
            gp.AddLine(new SKPoint(bounds.Left + (0.148936170212766f * bounds.Width), bounds.Top + (0.975708502024291f * bounds.Height)), new SKPoint(bounds.Left + bounds.Width, bounds.Top + bounds.Height));
            gp.Close();
            canvas.DrawPath(gp, _greenPaint);
            gp = new SKPath();
            pts = new SKPoint[38];
            pts[0] = new SKPoint(bounds.Left + (0.297872340425532f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.148936170212766f * bounds.Width), bounds.Top + (0.0688259109311741f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.099290780141844f * bounds.Width), bounds.Top + (0.133603238866397f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.177304964539007f * bounds.Width), bounds.Top + (0.194331983805668f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.170212765957447f * bounds.Width), bounds.Top + (0.311740890688259f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.24113475177305f * bounds.Width), bounds.Top + (0.295546558704453f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.276595744680851f * bounds.Width), bounds.Top + (0.327935222672065f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.276595744680851f * bounds.Width), bounds.Top + (0.352226720647773f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.375886524822695f * bounds.Width), bounds.Top + (0.348178137651822f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.425531914893617f * bounds.Width), bounds.Top + (0.437246963562753f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.411347517730496f * bounds.Width), bounds.Top + (0.582995951417004f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.340425531914894f * bounds.Width), bounds.Top + (0.582995951417004f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.319148936170213f * bounds.Width), bounds.Top + (0.506072874493927f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.276595744680851f * bounds.Width), bounds.Top + (0.489878542510121f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.219858156028369f * bounds.Width), bounds.Top + (0.445344129554656f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.156028368794326f * bounds.Width), bounds.Top + (0.481781376518219f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.0638297872340425f * bounds.Width), bounds.Top + (0.493927125506073f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.0212765957446809f * bounds.Width), bounds.Top + (0.566801619433198f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.177304964539007f * bounds.Width), bounds.Top + (0.659919028340081f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.163120567375887f * bounds.Width), bounds.Top + (0.736842105263158f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.156028368794326f * bounds.Width), bounds.Top + (0.931174089068826f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.939271255060729f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.0212765957446809f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.319148936170213f * bounds.Width), bounds.Top + (0.979757085020243f * bounds.Height));
            pts[24] = new SKPoint(bounds.Left + (0.304964539007092f * bounds.Width), bounds.Top + (0.910931174089069f * bounds.Height));
            pts[25] = new SKPoint(bounds.Left + (0.319148936170213f * bounds.Width), bounds.Top + (0.765182186234818f * bounds.Height));
            pts[26] = new SKPoint(bounds.Left + (0.432624113475177f * bounds.Width), bounds.Top + (0.809716599190283f * bounds.Height));
            pts[27] = new SKPoint(bounds.Left + (0.624113475177305f * bounds.Width), bounds.Top + (0.838056680161943f * bounds.Height));
            pts[28] = new SKPoint(bounds.Left + (0.765957446808511f * bounds.Width), bounds.Top + (0.740890688259109f * bounds.Height));
            pts[29] = new SKPoint(bounds.Left + (0.730496453900709f * bounds.Width), bounds.Top + (0.506072874493927f * bounds.Height));
            pts[30] = new SKPoint(bounds.Left + (0.574468085106383f * bounds.Width), bounds.Top + (0.421052631578947f * bounds.Height));
            pts[31] = new SKPoint(bounds.Left + (0.517730496453901f * bounds.Width), bounds.Top + (0.315789473684211f * bounds.Height));
            pts[32] = new SKPoint(bounds.Left + (0.581560283687943f * bounds.Width), bounds.Top + (0.246963562753036f * bounds.Height));
            pts[33] = new SKPoint(bounds.Left + (0.553191489361702f * bounds.Width), bounds.Top + (0.17004048582996f * bounds.Height));
            pts[34] = new SKPoint(bounds.Left + (0.50354609929078f * bounds.Width), bounds.Top + (0.137651821862348f * bounds.Height));
            pts[35] = new SKPoint(bounds.Left + (0.517730496453901f * bounds.Width), bounds.Top + (0.0890688259109312f * bounds.Height));
            pts[36] = new SKPoint(bounds.Left + (0.411347517730496f * bounds.Width), bounds.Top + (0.048582995951417f * bounds.Height));
            pts[37] = new SKPoint(bounds.Left + (0.404255319148936f * bounds.Width), bounds.Top + (0.0242914979757085f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _salmonPaint);
        }
        private void DrawKey(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKPoint[] pts = new SKPoint[23];
            SKRect rect;
            pts[0] = new SKPoint(bounds.Left + (0.694915254237288f * bounds.Width), bounds.Top + (0.47887323943662f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.983050847457627f * bounds.Width), bounds.Top + (0.316901408450704f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.105633802816901f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.542372881355932f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.11864406779661f * bounds.Width), bounds.Top + (0.112676056338028f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.302816901408451f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.305084745762712f * bounds.Width), bounds.Top + (0.464788732394366f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.322033898305085f * bounds.Width), bounds.Top + (0.492957746478873f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.322033898305085f * bounds.Width), bounds.Top + (0.542253521126761f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.203389830508475f * bounds.Width), bounds.Top + (0.570422535211268f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.203389830508475f * bounds.Width), bounds.Top + (0.626760563380282f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.305084745762712f * bounds.Width), bounds.Top + (0.640845070422535f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.322033898305085f * bounds.Width), bounds.Top + (0.704225352112676f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.203389830508475f * bounds.Width), bounds.Top + (0.73943661971831f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.186440677966102f * bounds.Width), bounds.Top + (0.788732394366197f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.322033898305085f * bounds.Width), bounds.Top + (0.802816901408451f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.322033898305085f * bounds.Width), bounds.Top + (0.859154929577465f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.203389830508475f * bounds.Width), bounds.Top + (0.880281690140845f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.203389830508475f * bounds.Width), bounds.Top + (0.936619718309859f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.288135593220339f * bounds.Width), bounds.Top + (0.922535211267606f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.372881355932203f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.627118644067797f * bounds.Width), bounds.Top + (0.964788732394366f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.694915254237288f * bounds.Width), bounds.Top + (0.901408450704225f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _silverPaint);
            canvas.DrawPath(gp, _border1Paint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.1f), bounds.Width * 0.2f, bounds.Width * 0.2f);
            canvas.DrawOval(rect, _blackPaint);
        }
        private void DrawLock(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            SKPath gp = new SKPath();
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Silver, bounds.Width / 15);
            SKPaint br_Fill;
            canvas.DrawRect(bounds, _bluePaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.45f, bounds.Height * 0.5f);
            canvas.DrawRect(rect, _darkGoldenRodPaint);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.4f), bounds.Top + (bounds.Height * 0.45f), bounds.Width * 0.4f, bounds.Height * 0.45f);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LightYellow, SKColors.Gold, rect, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawRect(rect, br_Fill);
            gp.AddLine(new SKPoint(rect.Left + (rect.Width / 4), rect.Top), new SKPoint(rect.Left + (rect.Width / 4), rect.Top - (bounds.Height * 0.2f)), true);
            gp.AddArc(SKRect.Create(rect.Left + (rect.Width / 4), rect.Top - (bounds.Height * 0.3f), rect.Width / 2, bounds.Height * 0.2f), 180, 180);
            gp.AddLine(new SKPoint(rect.Left + ((rect.Width * 3) / 4), rect.Top - (bounds.Height * 0.2f)), new SKPoint(rect.Left + ((rect.Width * 3) / 4), rect.Top));
            canvas.DrawPath(gp, pn_Temp);
            rect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.3f, bounds.Height * 0.55f);
            DrawKey(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.15f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.3f, bounds.Height * 0.55f);
            DrawKey(canvas, rect);
        }
        private void DrawCondo(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect1;
            SKRect rect2;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.White, bounds.Width / 15);
            SKPaint br_Fill;
            SKPoint[] pts;
            rect1 = SKRect.Create(bounds.Left, bounds.Top, bounds.Width, (bounds.Height * 2) / 3);
            rect2 = SKRect.Create(bounds.Left, bounds.Top + ((bounds.Height * 2) / 3), bounds.Width, bounds.Height / 3);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.DarkBlue, SKColors.Magenta, rect1, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawRect(rect1, br_Fill);
            rect1 = SKRect.Create(bounds.Left + (bounds.Width / 3), bounds.Top + (bounds.Height * 0.3f), bounds.Width / 3, bounds.Width / 3);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Yellow, SKColors.Orange, rect1, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawOval(rect1, br_Fill);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Blue, SKColors.Aqua, rect2, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawRect(rect2, br_Fill);
            SKPath gp = new SKPath();
            pts = new SKPoint[37];
            pts[0] = new SKPoint(bounds.Left + (0.0280373831775701f * bounds.Width), bounds.Top + (0.325471698113208f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.0981308411214953f * bounds.Width), bounds.Top + (0.273584905660377f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.228971962616822f * bounds.Width), bounds.Top + (0.363207547169811f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.257009345794393f * bounds.Width), bounds.Top + (0.424528301886792f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.214953271028037f * bounds.Width), bounds.Top + (0.424528301886792f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.219626168224299f * bounds.Width), bounds.Top + (0.466981132075472f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.257009345794393f * bounds.Width), bounds.Top + (0.518867924528302f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.219626168224299f * bounds.Width), bounds.Top + (0.537735849056604f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.177570093457944f * bounds.Width), bounds.Top + (0.481132075471698f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.144859813084112f * bounds.Width), bounds.Top + (0.471698113207547f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.116822429906542f * bounds.Width), bounds.Top + (0.44811320754717f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.0981308411214953f * bounds.Width), bounds.Top + (0.452830188679245f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.0700934579439252f * bounds.Width), bounds.Top + (0.608490566037736f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.158878504672897f * bounds.Width), bounds.Top + (0.693396226415094f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.5f * bounds.Width), bounds.Top + (0.820754716981132f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.841121495327103f * bounds.Width), bounds.Top + (0.716981132075472f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.920560747663551f * bounds.Width), bounds.Top + (0.627358490566038f * bounds.Height));
            pts[17] = new SKPoint(bounds.Left + (0.906542056074766f * bounds.Width), bounds.Top + (0.490566037735849f * bounds.Height));
            pts[18] = new SKPoint(bounds.Left + (0.878504672897196f * bounds.Width), bounds.Top + (0.570754716981132f * bounds.Height));
            pts[19] = new SKPoint(bounds.Left + (0.836448598130841f * bounds.Width), bounds.Top + (0.580188679245283f * bounds.Height));
            pts[20] = new SKPoint(bounds.Left + (0.841121495327103f * bounds.Width), bounds.Top + (0.514150943396226f * bounds.Height));
            pts[21] = new SKPoint(bounds.Left + (0.841121495327103f * bounds.Width), bounds.Top + (0.471698113207547f * bounds.Height));
            pts[22] = new SKPoint(bounds.Left + (0.771028037383178f * bounds.Width), bounds.Top + (0.523584905660377f * bounds.Height));
            pts[23] = new SKPoint(bounds.Left + (0.728971962616822f * bounds.Width), bounds.Top + (0.5f * bounds.Height));
            pts[24] = new SKPoint(bounds.Left + (0.775700934579439f * bounds.Width), bounds.Top + (0.476415094339623f * bounds.Height));
            pts[25] = new SKPoint(bounds.Left + (0.836448598130841f * bounds.Width), bounds.Top + (0.433962264150943f * bounds.Height));
            pts[26] = new SKPoint(bounds.Left + (0.775700934579439f * bounds.Width), bounds.Top + (0.39622641509434f * bounds.Height));
            pts[27] = new SKPoint(bounds.Left + (0.794392523364486f * bounds.Width), bounds.Top + (0.372641509433962f * bounds.Height));
            pts[28] = new SKPoint(bounds.Left + (0.892523364485981f * bounds.Width), bounds.Top + (0.382075471698113f * bounds.Height));
            pts[29] = new SKPoint(bounds.Left + (0.906542056074766f * bounds.Width), bounds.Top + (0.410377358490566f * bounds.Height));
            pts[30] = new SKPoint(bounds.Left + (0.967289719626168f * bounds.Width), bounds.Top + (0.443396226415094f * bounds.Height));
            pts[31] = new SKPoint(bounds.Left + (0.981308411214953f * bounds.Width), bounds.Top + (0.674528301886792f * bounds.Height));
            pts[32] = new SKPoint(bounds.Left + (0.976635514018692f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[33] = new SKPoint(bounds.Left + (0.892523364485981f * bounds.Width), bounds.Top + (0.995283018867924f * bounds.Height));
            pts[34] = new SKPoint(bounds.Left + (0.0420560747663551f * bounds.Width), bounds.Top + (0.990566037735849f * bounds.Height));
            pts[35] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.933962264150943f * bounds.Height));
            pts[36] = new SKPoint(bounds.Left + (0.00467289719626168f * bounds.Width), bounds.Top + (0.287735849056604f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _blackPaint);
            canvas.DrawRect(bounds, pn_Temp);
        }
        private void DrawLaughingFace(SKCanvas canvas, SKRect bounds)
        {
            var br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Yellow, SKColors.Gold, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            SKPath gp = new SKPath();
            canvas.DrawOval(bounds, br_Fill);
            gp.AddArc(SKRect.Create(bounds.Left + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.6f, bounds.Height * 0.5f), 0, 180);
            gp.Close();
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.Red, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            gp.AddArc(SKRect.Create(bounds.Left + (bounds.Width * 0.2f), bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.2f, bounds.Height * 0.4f), 180, 180);
            canvas.DrawPath(gp, _border1Paint);
            canvas.DrawArc(SKRect.Create(bounds.Left + (bounds.Width * 0.6f), bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.2f, bounds.Height * 0.4f), 180, 180, _border1Paint);
        }
        private void DrawLaughingGas(SKCanvas canvas, SKRect bounds)
        {
            SKRect rect;
            rect = SKRect.Create(bounds.Left, bounds.Top, bounds.Width * 0.55f, bounds.Height * 0.55f);
            DrawLaughingFace(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.6f), bounds.Top, bounds.Width * 0.55f, bounds.Height * 0.55f);
            DrawLaughingFace(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (bounds.Height * 0.4f), bounds.Width * 0.55f, bounds.Height * 0.55f);
            DrawLaughingFace(canvas, rect);
        }
        private void DrawFish(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp = new SKPath();
            SKPoint[] pts = new SKPoint[7];
            var br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LightBlue, SKColors.Blue, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            pts[0] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.0344827586206897f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.0111111111111111f * bounds.Width), bounds.Top + (0.862068965517241f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.244444444444444f * bounds.Width), bounds.Top + (0.672413793103448f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.566666666666667f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.603448275862069f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.688888888888889f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.288888888888889f * bounds.Width), bounds.Top + (0.258620689655172f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, br_Fill);
        }
        private void DrawParrot(SKCanvas canvas, SKRect bounds)
        {
            SKPaint br_Fill;
            SKPath gp;
            SKPoint[] pts;
            SKPath gp2;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 15);
            pts = new SKPoint[3];
            pts[0] = new SKPoint(bounds.Left + (0.329896907216495f * bounds.Width), bounds.Top + (0.709677419354839f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.288659793814433f * bounds.Width), bounds.Top + (0.794354838709677f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.190721649484536f * bounds.Width), bounds.Top + (0.806451612903226f * bounds.Height));
            canvas.DrawLines(pts, pn_Temp);
            gp2 = new SKPath();
            pts = new SKPoint[17];
            pts[0] = new SKPoint(bounds.Left + (0.0851063829787234f * bounds.Width), bounds.Top + (0.628571428571429f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.25531914893617f * bounds.Width), bounds.Top + (0.755102040816326f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.627659574468085f * bounds.Width), bounds.Top + (0.881632653061225f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.765957446808511f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.991836734693878f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.898936170212766f * bounds.Width), bounds.Top + (0.922448979591837f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.925531914893617f * bounds.Width), bounds.Top + (0.848979591836735f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.813829787234043f * bounds.Width), bounds.Top + (0.775510204081633f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (0.75f * bounds.Width), bounds.Top + (0.722448979591837f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.728723404255319f * bounds.Width), bounds.Top + (0.6f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.638297872340426f * bounds.Width), bounds.Top + (0.506122448979592f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.627659574468085f * bounds.Width), bounds.Top + (0.416326530612245f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.563829787234043f * bounds.Width), bounds.Top + (0.379591836734694f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.542553191489362f * bounds.Width), bounds.Top + (0.253061224489796f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.351063829787234f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.143617021276596f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.0851063829787234f * bounds.Width), bounds.Top + (0.118367346938776f * bounds.Height));
            gp2.AddLines(pts, true);
            canvas.DrawPath(gp2, _redPaint);
            gp = new SKPath();
            pts = new SKPoint[17];
            pts[0] = new SKPoint(bounds.Left + (0.531914893617021f * bounds.Width), bounds.Top + (0.253061224489796f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.218085106382979f * bounds.Width), bounds.Top + (0.302040816326531f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.148936170212766f * bounds.Width), bounds.Top + (0.444897959183673f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.234042553191489f * bounds.Width), bounds.Top + (0.653061224489796f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.478723404255319f * bounds.Width), bounds.Top + (0.726530612244898f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.526595744680851f * bounds.Width), bounds.Top + (0.83265306122449f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.664893617021277f * bounds.Width), bounds.Top + (0.893877551020408f * bounds.Height));
            pts[7] = new SKPoint(bounds.Left + (0.776595744680851f * bounds.Width), bounds.Top + (0.995918367346939f * bounds.Height));
            pts[8] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.987755102040816f * bounds.Height));
            pts[9] = new SKPoint(bounds.Left + (0.898936170212766f * bounds.Width), bounds.Top + (0.914285714285714f * bounds.Height));
            pts[10] = new SKPoint(bounds.Left + (0.930851063829787f * bounds.Width), bounds.Top + (0.844897959183674f * bounds.Height));
            pts[11] = new SKPoint(bounds.Left + (0.75f * bounds.Width), bounds.Top + (0.730612244897959f * bounds.Height));
            pts[12] = new SKPoint(bounds.Left + (0.707446808510638f * bounds.Width), bounds.Top + (0.591836734693878f * bounds.Height));
            pts[13] = new SKPoint(bounds.Left + (0.632978723404255f * bounds.Width), bounds.Top + (0.510204081632653f * bounds.Height));
            pts[14] = new SKPoint(bounds.Left + (0.617021276595745f * bounds.Width), bounds.Top + (0.412244897959184f * bounds.Height));
            pts[15] = new SKPoint(bounds.Left + (0.558510638297872f * bounds.Width), bounds.Top + (0.375510204081633f * bounds.Height));
            pts[16] = new SKPoint(bounds.Left + (0.537234042553192f * bounds.Width), bounds.Top + (0.261224489795918f * bounds.Height));
            gp.AddLines(pts, true);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LimeGreen, SKColors.Blue, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle135);
            canvas.DrawPath(gp, br_Fill);
            gp = new SKPath();
            pts = new SKPoint[6];
            pts[0] = new SKPoint(bounds.Left + (0.23936170212766f * bounds.Width), bounds.Top + (0.0530612244897959f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.164893617021277f * bounds.Width), bounds.Top + (0.0979591836734694f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.212765957446809f * bounds.Width), bounds.Top + (0.163265306122449f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.265957446808511f * bounds.Width), bounds.Top + (0.216326530612245f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.303191489361702f * bounds.Width), bounds.Top + (0.175510204081633f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.313829787234043f * bounds.Width), bounds.Top + (0.0816326530612245f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawOval(SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top + (gp.Bounds.Height * 0.2f), bounds.Width / 12, bounds.Width / 12), _blackPaint);
            canvas.DrawPath(gp2, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[7];
            pts[0] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.314285714285714f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.0106382978723404f * bounds.Width), bounds.Top + (0.175510204081633f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.0797872340425532f * bounds.Width), bounds.Top + (0.130612244897959f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.159574468085106f * bounds.Width), bounds.Top + (0.155102040816327f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.138297872340426f * bounds.Width), bounds.Top + (0.179591836734694f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.191489361702128f * bounds.Width), bounds.Top + (0.220408163265306f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.0957446808510638f * bounds.Width), bounds.Top + (0.257142857142857f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _yellowPaint);
            canvas.DrawPath(gp, pn_Temp);
        }
        private void DrawPets(SKCanvas canvas, SKRect bounds)
        {
            var rect = SKRect.Create(bounds.Left, bounds.Top, bounds.Width * 0.7f, bounds.Height);
            DrawParrot(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.5f), bounds.Top + (bounds.Height * 0), bounds.Width * 0.3f, bounds.Height * 0.25f);
            DrawFish(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (bounds.Height * 0.5f), bounds.Width * 0.3f, bounds.Height * 0.25f);
            DrawFish(canvas, rect);
            rect = SKRect.Create(bounds.Left + (bounds.Width * 0.8f), bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.3f, bounds.Height * 0.25f);
            DrawFish(canvas, rect);
        }
        private void DrawYogurt(SKCanvas canvas, SKRect bounds)
        {
            SKPath gp;
            SKPath gp2;
            SKPoint[] pts;
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Purple, bounds.Width / 15);
            gp2 = new SKPath();
            pts = new SKPoint[7];
            pts[0] = new SKPoint(bounds.Left + (0.00625f * bounds.Width), bounds.Top + (0.361990950226244f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.025f * bounds.Width), bounds.Top + (0.389140271493213f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.06875f * bounds.Width), bounds.Top + (0.923076923076923f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.4375f * bounds.Width), bounds.Top + (1 * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.8875f * bounds.Width), bounds.Top + (0.918552036199095f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.925f * bounds.Width), bounds.Top + (0.389140271493213f * bounds.Height));
            pts[6] = new SKPoint(bounds.Left + (0.96875f * bounds.Width), bounds.Top + (0.352941176470588f * bounds.Height));
            gp2.AddLines(pts, true);
            canvas.DrawPath(gp2, _orchidPaint);
            gp = new SKPath();
            pts = new SKPoint[6];
            pts[0] = new SKPoint(bounds.Left + (0.475f * bounds.Width), bounds.Top + (0.375565610859729f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.5625f * bounds.Width), bounds.Top + (0.850678733031674f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.875f * bounds.Width), bounds.Top + (0.918552036199095f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.925f * bounds.Width), bounds.Top + (0.778280542986425f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.91875f * bounds.Width), bounds.Top + (0.393665158371041f * bounds.Height));
            pts[5] = new SKPoint(bounds.Left + (0.9625f * bounds.Width), bounds.Top + (0.343891402714932f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            gp = new SKPath();
            pts = new SKPoint[5];
            pts[0] = new SKPoint(bounds.Left + (0.925f * bounds.Width), bounds.Top + (0.597285067873303f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.80625f * bounds.Width), bounds.Top + (0.592760180995475f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.71875f * bounds.Width), bounds.Top + (0.642533936651584f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.73125f * bounds.Width), bounds.Top + (0.733031674208145f * bounds.Height));
            pts[4] = new SKPoint(bounds.Left + (0.86875f * bounds.Width), bounds.Top + (0.800904977375566f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _redPaint);
            canvas.DrawPath(gp2, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0 * bounds.Width), bounds.Top + (0.357466063348416f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.475f * bounds.Width), bounds.Top + (0.425339366515837f * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (0.975f * bounds.Width), bounds.Top + (0.343891402714932f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.5f * bounds.Width), bounds.Top + (0.266968325791855f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _yellowPaint);
            canvas.DrawPath(gp, pn_Temp);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(bounds.Left + (0.61875f * bounds.Width), bounds.Top + (0.321266968325792f * bounds.Height));
            pts[1] = new SKPoint(bounds.Left + (0.9125f * bounds.Width), bounds.Top + (0 * bounds.Height));
            pts[2] = new SKPoint(bounds.Left + (1 * bounds.Width), bounds.Top + (0.0316742081447964f * bounds.Height));
            pts[3] = new SKPoint(bounds.Left + (0.7125f * bounds.Width), bounds.Top + (0.334841628959276f * bounds.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp, _border1Paint);
        }
        #endregion
        #region "Mail Graphics"
        private void DrawMiscCard(SKCanvas canvas, SKRect bounds, string firstText, string secondText, bool exception = false)
        {
            SKRect firstRect;
            if (exception == false)
                firstRect = SKRect.Create(bounds.Left, bounds.Top, bounds.Width / 2, bounds.Height / 2);
            else
                firstRect = SKRect.Create(bounds.Left + 5, bounds.Top + 5, bounds.Width, bounds.Height / 2);
            SKPaint textPaint;
            textPaint = MiscHelpers.GetTextPaint(SKColors.Red, firstRect.Height * 0.22f);
            if (exception == false)
                canvas.DrawCustomText(firstText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            else
            {
                textPaint = MiscHelpers.GetTextPaint(SKColors.Red, firstRect.Height * 0.18f);
                canvas.DrawCustomText(firstText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Start, textPaint, firstRect, out _);
            }
            if (exception == false)
                firstRect = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height / 2), bounds.Width / 2, bounds.Height / 2);
            else
                firstRect = SKRect.Create(bounds.Left + 5, bounds.Top + (bounds.Height / 2), bounds.Width / 2, bounds.Height / 2);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Red, firstRect.Height * 0.22f);
            canvas.DrawCustomText(secondText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
        }
        private void DrawRecyclingCard(SKCanvas canvas, SKRect bounds)
        {
            DrawMiscCard(canvas, bounds, "Support Local Recycling", "Give $200", true);
            SKMatrix tmp_Matrix = new SKMatrix();
            SKRect rect_Right = SKRect.Create(bounds.Left + (bounds.Width * 0.6f), bounds.Top + (bounds.Height * 0.3f), bounds.Width * 0.35f, bounds.Height * 0.6f);
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Green, bounds.Width / 30);
            SKPoint[] pts;
            SKPath gp;
            SKRect rect;
            SKPoint pt_Center = new SKPoint(rect_Right.Left + (rect_Right.Width / 2), rect_Right.Top + (rect_Right.Height / 2));
            canvas.DrawRect(rect_Right, _blackPaint);
            rect = SKRect.Create(rect_Right.Left, rect_Right.Top, rect_Right.Width, rect_Right.Height);
            gp = new SKPath();
            pts = new SKPoint[4];
            pts[0] = new SKPoint(rect.Left + (0.199203187250996f * rect.Width), rect.Top + (0.283154121863799f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.306772908366534f * rect.Width), rect.Top + (0.10752688172043f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.545816733067729f * rect.Width), rect.Top + (0.10752688172043f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.382470119521912f * rect.Width), rect.Top + (0.390681003584229f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _limeGreenPaint);
            tmp_Matrix.RotateAt(120, pt_Center);
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _limeGreenPaint);
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _limeGreenPaint);
            gp = new SKPath();
            pts = new SKPoint[3];
            pts[0] = new SKPoint(rect.Left + (0.35f * rect.Width), rect.Top + (0.0896057347670251f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.44f * rect.Width), rect.Top + (0.17921146953405f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.5f * rect.Width), rect.Top + (0.28f * rect.Height));
            gp.AddLines(pts);
            gp.AddLine(new SKPoint(rect.Left + (0.4f * rect.Width), rect.Top + (0.4f * rect.Height)), new SKPoint(rect.Left + (0.697211155378486f * rect.Width), rect.Top + (0.4f * rect.Height)));
            gp.AddLine(new SKPoint(rect.Left + (0.697211155378486f * rect.Width), rect.Top + (0.4f * rect.Height)), new SKPoint(rect.Left + (0.8f * rect.Width), rect.Top + (0.1f * rect.Height)));
            pts[0] = new SKPoint(rect.Left + (0.68f * rect.Width), rect.Top + (0.189964157706093f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.63f * rect.Width), rect.Top + (0.118279569892473f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.58f * rect.Width), rect.Top + (0.0716845878136201f * rect.Height));
            gp.AddLines(pts, true);
            canvas.DrawPath(gp, _limeGreenPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _limeGreenPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp.Transform(tmp_Matrix);
            canvas.DrawPath(gp, _limeGreenPaint);
            canvas.DrawPath(gp, _border1Paint);
            canvas.DrawRect(rect_Right, pn_Temp);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawEndangeredSpeciesCard(SKCanvas canvas, SKRect bounds)
        {
            DrawMiscCard(canvas, bounds, "Save Threatened Species", "Give $400", true);
            var rect_Right = SKRect.Create(bounds.Left + (bounds.Width * 0.5f), bounds.Top + (bounds.Height * 0.25f), bounds.Width * 0.45f, bounds.Height * 0.7f);
            SKPoint[] pts;
            SKPath gp;
            SKPath gp2;
            SKPaint br_Fill;
            SKRect rect;
            rect = SKRect.Create(rect_Right.Left, rect_Right.Top, rect_Right.Width * 0.6f, rect_Right.Height * 0.6f);
            gp = new SKPath();
            pts = new SKPoint[5];
            pts[0] = new SKPoint(rect.Left + (0.833333333333333f * rect.Width), rect.Top + (0.0292682926829268f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.902298850574713f * rect.Width), rect.Top + (0.0195121951219512f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.102439024390244f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.948275862068966f * rect.Width), rect.Top + (0.0829268292682927f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.816091954022989f * rect.Width), rect.Top + (0.102439024390244f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _peachPuffPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[23];
            pts[0] = new SKPoint(rect.Left + (0.551724137931034f * rect.Width), rect.Top + (0.770731707317073f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.580459770114943f * rect.Width), rect.Top + (0.863414634146341f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.505747126436782f * rect.Width), rect.Top + (0.84390243902439f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.408045977011494f * rect.Width), rect.Top + (0.878048780487805f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.252873563218391f * rect.Width), rect.Top + (0.980487804878049f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.310344827586207f * rect.Width), rect.Top + (0.975609756097561f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.477011494252874f * rect.Width), rect.Top + (0.921951219512195f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.448275862068966f * rect.Width), rect.Top + (0.985365853658537f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.482758620689655f * rect.Width), rect.Top + (0.926829268292683f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.603448275862069f * rect.Width), rect.Top + (0.931707317073171f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.609195402298851f * rect.Width), rect.Top + (0.985365853658537f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.735632183908046f * rect.Width), rect.Top + (1 * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.770114942528736f * rect.Width), rect.Top + (0.882926829268293f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.844827586206897f * rect.Width), rect.Top + (0.887804878048781f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.919540229885057f * rect.Width), rect.Top + (0.770731707317073f * rect.Height));
            pts[15] = new SKPoint(rect.Left + (0.82183908045977f * rect.Width), rect.Top + (0.790243902439024f * rect.Height));
            pts[16] = new SKPoint(rect.Left + (0.793103448275862f * rect.Width), rect.Top + (0.746341463414634f * rect.Height));
            pts[17] = new SKPoint(rect.Left + (0.724137931034483f * rect.Width), rect.Top + (0.726829268292683f * rect.Height));
            pts[18] = new SKPoint(rect.Left + (0.655172413793103f * rect.Width), rect.Top + (0.619512195121951f * rect.Height));
            pts[19] = new SKPoint(rect.Left + (0.620689655172414f * rect.Width), rect.Top + (0.697560975609756f * rect.Height));
            pts[20] = new SKPoint(rect.Left + (0.666666666666667f * rect.Width), rect.Top + (0.804878048780488f * rect.Height));
            pts[21] = new SKPoint(rect.Left + (0.591954022988506f * rect.Width), rect.Top + (0.741463414634146f * rect.Height));
            pts[22] = new SKPoint(rect.Left + (0.67816091954023f * rect.Width), rect.Top + (0.141463414634146f * rect.Height));
            gp.AddPoly(pts); // i think that adding poly was better in this case
            canvas.DrawPath(gp, _brownPaint);
            gp2 = new SKPath();
            pts = new SKPoint[14];
            pts[0] = new SKPoint(rect.Left + (0.85632183908046f * rect.Width), rect.Top + (0.0292682926829268f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.775862068965517f * rect.Width), rect.Top + (-0.1f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.672413793103448f * rect.Width), rect.Top + (0 * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.666666666666667f * rect.Width), rect.Top + (0.170731707317073f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.459770114942529f * rect.Width), rect.Top + (0.239024390243902f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.770731707317073f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.21264367816092f * rect.Width), rect.Top + (0.75609756097561f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.402298850574713f * rect.Width), rect.Top + (0.697560975609756f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.540229885057471f * rect.Width), rect.Top + (0.819512195121951f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.729885057471264f * rect.Width), rect.Top + (0.678048780487805f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.71264367816092f * rect.Width), rect.Top + (0.609756097560976f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.873563218390805f * rect.Width), rect.Top + (0.453658536585366f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.931034482758621f * rect.Width), rect.Top + (0.302439024390244f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.844827586206897f * rect.Width), rect.Top + (0.185365853658537f * rect.Height));
            gp2.AddPoly(pts);
            canvas.DrawPath(gp2, _blackPaint);
            gp = new SKPath();
            pts = new SKPoint[5];
            pts[0] = new SKPoint(rect.Left + (0.649425287356322f * rect.Width), rect.Top + (0.0536585365853659f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.752873563218391f * rect.Width), rect.Top + (0.0048780487804878f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.839080459770115f * rect.Width), rect.Top + (0.024390243902439f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.833333333333333f * rect.Width), rect.Top + (0.165853658536585f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.752873563218391f * rect.Width), rect.Top + (0.165853658536585f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _whitePaint);
            canvas.DrawPath(gp2, _border1Paint);
            rect = SKRect.Create(rect_Right.Left + (rect_Right.Width / 2), rect_Right.Top + (rect_Right.Height / 2), rect_Right.Width / 2, rect_Right.Height / 2);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.LightBlue, SKColors.Blue, rect, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawOval(rect, br_Fill);
            gp = new SKPath();
            pts = new SKPoint[26];
            pts[0] = new SKPoint(rect.Left + (0.157142857142857f * rect.Width), rect.Top + (0.245033112582781f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.242857142857143f * rect.Width), rect.Top + (0.337748344370861f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.321428571428571f * rect.Width), rect.Top + (0.403973509933775f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.321428571428571f * rect.Width), rect.Top + (0.496688741721854f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.407142857142857f * rect.Width), rect.Top + (0.463576158940397f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.471428571428571f * rect.Width), rect.Top + (0.516556291390728f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.471428571428571f * rect.Width), rect.Top + (0.602649006622517f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.585714285714286f * rect.Width), rect.Top + (0.675496688741722f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.628571428571429f * rect.Width), rect.Top + (0.768211920529801f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.714285714285714f * rect.Width), rect.Top + (0.834437086092715f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.685714285714286f * rect.Width), rect.Top + (0.741721854304636f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.757142857142857f * rect.Width), rect.Top + (0.655629139072848f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.721428571428571f * rect.Width), rect.Top + (0.56953642384106f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.585714285714286f * rect.Width), rect.Top + (0.516556291390728f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.55f * rect.Width), rect.Top + (0.536423841059603f * rect.Height));
            pts[15] = new SKPoint(rect.Left + (0.5f * rect.Width), rect.Top + (0.47682119205298f * rect.Height));
            pts[16] = new SKPoint(rect.Left + (0.428571428571429f * rect.Width), rect.Top + (0.437086092715232f * rect.Height));
            pts[17] = new SKPoint(rect.Left + (0.364285714285714f * rect.Width), rect.Top + (0.450331125827815f * rect.Height));
            pts[18] = new SKPoint(rect.Left + (0.364285714285714f * rect.Width), rect.Top + (0.384105960264901f * rect.Height));
            pts[19] = new SKPoint(rect.Left + (0.414285714285714f * rect.Width), rect.Top + (0.364238410596026f * rect.Height));
            pts[20] = new SKPoint(rect.Left + (0.485714285714286f * rect.Width), rect.Top + (0.410596026490066f * rect.Height));
            pts[21] = new SKPoint(rect.Left + (0.485714285714286f * rect.Width), rect.Top + (0.33112582781457f * rect.Height));
            pts[22] = new SKPoint(rect.Left + (0.571428571428571f * rect.Width), rect.Top + (0.264900662251656f * rect.Height));
            pts[23] = new SKPoint(rect.Left + (0.492857142857143f * rect.Width), rect.Top + (0.198675496688742f * rect.Height));
            pts[24] = new SKPoint(rect.Left + (0.378571428571429f * rect.Width), rect.Top + (0.0993377483443709f * rect.Height));
            pts[25] = new SKPoint(rect.Left + (0.221428571428571f * rect.Width), rect.Top + (0.145695364238411f * rect.Height));
            gp.AddPoly(pts);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.Brown, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _border1Paint);
            canvas.DrawOval(rect, _border1Paint);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawPollutionCard(SKCanvas canvas, SKRect bounds)
        {
            DrawMiscCard(canvas, bounds, "Stop Polution", "Give $400");
            var rect_Right = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.05f), bounds.Width * 0.45f, bounds.Height * 0.7f);
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 40);
            SKPoint[] pts;
            SKPath gp = new SKPath();
            SKPaint br_Fill;
            SKRect rect;
            SKRect rect1;
            SKRect rect2;
            SKRect rect3;
            canvas.DrawRect(rect_Right, _skyBluePaint);
            pts = new SKPoint[4];
            pts[0] = new SKPoint(rect_Right.Left, rect_Right.Top + (rect_Right.Height / 2));
            pts[1] = new SKPoint(rect_Right.Left + (rect_Right.Width / 4), rect_Right.Top + (rect_Right.Height * 0.4f));
            pts[2] = new SKPoint(rect_Right.Left + (rect_Right.Width / 2), rect_Right.Top + (rect_Right.Height / 2));
            pts[3] = new SKPoint(rect_Right.Left + (rect_Right.Width), rect_Right.Top + (rect_Right.Height * 0.3f));
            gp.AddLines(pts);
            gp.AddLine((int)rect_Right.Left + (int)rect_Right.Width, (int)rect_Right.Top + (int)rect_Right.Height, (int)rect_Right.Left, (int)rect_Right.Top + (int)rect_Right.Height);
            gp.Close();
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Green, SKColors.LimeGreen, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _border1Paint);
            rect = SKRect.Create(rect_Right.Left, rect_Right.Top, rect_Right.Width, rect_Right.Height * 0.25f);
            gp = new SKPath();
            pts = new SKPoint[16];
            pts[0] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.650793650793651f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.0561797752808989f * rect.Width), rect.Top + (0.555555555555556f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.138576779026217f * rect.Width), rect.Top + (1 * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.262172284644195f * rect.Width), rect.Top + (0.555555555555556f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.352059925093633f * rect.Width), rect.Top + (0.873015873015873f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.423220973782772f * rect.Width), rect.Top + (0.682539682539683f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.48689138576779f * rect.Width), rect.Top + (0.857142857142857f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.546816479400749f * rect.Width), rect.Top + (0.555555555555556f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.625468164794007f * rect.Width), rect.Top + (0.825396825396825f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.711610486891386f * rect.Width), rect.Top + (0.825396825396825f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.722846441947566f * rect.Width), rect.Top + (0.603174603174603f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.801498127340824f * rect.Width), rect.Top + (0.888888888888889f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.876404494382023f * rect.Width), rect.Top + (0.825396825396825f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.887640449438202f * rect.Width), rect.Top + (0.46031746031746f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.970037453183521f * rect.Width), rect.Top + (0.301587301587302f * rect.Height));
            pts[15] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0 * rect.Height));
            gp.AddLines(pts);
            gp.AddLine((int)rect_Right.Left + (int)rect_Right.Width, (int)rect_Right.Top, (int)rect_Right.Left, (int)(rect_Right.Top));
            gp.Close();
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.DarkSlateGray, SKColors.White, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            rect = SKRect.Create(rect_Right.Left + (rect_Right.Width * 0.15f), rect_Right.Top + (rect_Right.Height * 0.65f), rect_Right.Width * 0.7f, rect_Right.Height * 0.2f);
            rect1 = SKRect.Create(rect.Left + (rect.Width * 0.4f), rect_Right.Top + (rect_Right.Height * 0.25f), rect.Width * 0.1f, rect_Right.Height * 0.45f);
            rect2 = SKRect.Create(rect.Left + (rect.Width * 0.625f), rect_Right.Top + (rect_Right.Height * 0.25f), rect.Width * 0.1f, rect_Right.Height * 0.45f);
            rect3 = SKRect.Create(rect.Left + (rect.Width * 0.85f), rect_Right.Top + (rect_Right.Height * 0.25f), rect.Width * 0.1f, rect_Right.Height * 0.45f);
            gp.AddRect(rect);
            gp.AddRect(rect1);
            gp.AddRect(rect2);
            gp.AddRect(rect3);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.Silver, rect1, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawRect(rect1, br_Fill);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.Silver, rect2, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawRect(rect2, br_Fill);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.Silver, rect3, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawRect(rect3, br_Fill);
            canvas.DrawPath(gp, _border1Paint);
            rect = SKRect.Create(rect.Left, rect.Top, rect.Width / 2, rect.Height);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.Silver, rect, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawRect(rect, br_Fill);
            rect = SKRect.Create(rect.Left + rect.Width, rect.Top, rect.Width, rect.Height);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Silver, SKColors.Black, rect, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawRect(rect, br_Fill);
            rect = SKRect.Create(rect_Right.Left + (rect_Right.Width * 0.15f), rect_Right.Top + (rect_Right.Height * 0.65f), rect_Right.Width * 0.7f, rect_Right.Height * 0.2f);
            canvas.DrawRect(rect, _border1Paint);
            canvas.DrawRect(rect_Right, pn_Temp);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawEnvironmentCard(SKCanvas canvas, SKRect bounds)
        {
            DrawMiscCard(canvas, bounds, "Citizens for a clean environment", "Give $300", true);
            var rect_Right = SKRect.Create(bounds.Left + (bounds.Width * 0.6f), bounds.Top + (bounds.Height * 0.5f), bounds.Width * 0.35f, bounds.Height * 0.45f);
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.SlateGray, bounds.Width / 40);
            SKPoint[] pts;
            SKPath gp;
            SKPaint br_Fill;
            SKRect rect;
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.SkyBlue, SKColors.Orchid, rect_Right, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawRect(rect_Right, br_Fill);
            rect = SKRect.Create(rect_Right.Left + (rect_Right.Width / 3), rect_Right.Top + (rect_Right.Height * 0.4f), rect_Right.Width / 3, rect_Right.Width / 3);
            canvas.DrawRect(rect, _yellowPaint);
            rect = SKRect.Create(rect_Right.Left, rect_Right.Top + (rect_Right.Height * 0.6f), rect_Right.Width, rect_Right.Height / 3);
            gp = new SKPath();
            pts = new SKPoint[8];
            pts[0] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0 * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.148471615720524f * rect.Width), rect.Top + (0.032967032967033f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.240174672489083f * rect.Width), rect.Top + (0.208791208791209f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.366812227074236f * rect.Width), rect.Top + (0.153846153846154f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.480349344978166f * rect.Width), rect.Top + (0.428571428571429f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.707423580786026f * rect.Width), rect.Top + (0.043956043956044f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.860262008733624f * rect.Width), rect.Top + (0.373626373626374f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.373626373626374f * rect.Height));
            gp.AddLines(pts);
            gp.AddLine(rect.Left + rect.Width, rect.Top + rect.Height, rect.Left, rect.Top + rect.Height);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.Green, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _greenBorder);
            canvas.DrawRect(rect_Right, pn_Temp);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawWhaleCard(SKCanvas canvas, SKRect bounds)
        {
            DrawMiscCard(canvas, bounds, "Whale Rescue Patrol", "Give $400", true);
            var rect_Right = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.25f), bounds.Width * 0.45f, bounds.Height * 0.7f);
            var pn_Temp = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 40);
            SKPoint[] pts;
            SKPath gp;
            SKPaint br_Fill;
            SKRect rect;
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Blue, SKColors.DarkBlue, rect_Right, MiscHelpers.EnumLinearGradientPercent.Angle180);
            canvas.DrawRect(rect_Right, br_Fill);
            rect = SKRect.Create(rect_Right.Left, rect_Right.Top + (rect_Right.Height * 0.1f), rect_Right.Width, rect_Right.Height * 0.7f);
            gp = new SKPath();
            pts = new SKPoint[27];
            pts[0] = new SKPoint(rect.Left + (0.714285714285714f * rect.Width), rect.Top + (0.44559585492228f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.860902255639098f * rect.Width), rect.Top + (0.55440414507772f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.932330827067669f * rect.Width), rect.Top + (0.450777202072539f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.996240601503759f * rect.Width), rect.Top + (0.414507772020725f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.476683937823834f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.943609022556391f * rect.Width), rect.Top + (0.626943005181347f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.93984962406015f * rect.Width), rect.Top + (0.756476683937824f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.870466321243523f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.932330827067669f * rect.Width), rect.Top + (0.854922279792746f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.853383458646617f * rect.Width), rect.Top + (0.735751295336788f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.74812030075188f * rect.Width), rect.Top + (0.740932642487047f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.537593984962406f * rect.Width), rect.Top + (0.77720207253886f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.398496240601504f * rect.Width), rect.Top + (0.797927461139896f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0.417293233082707f * rect.Width), rect.Top + (0.880829015544041f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.402255639097744f * rect.Width), rect.Top + (1 * rect.Height));
            pts[15] = new SKPoint(rect.Left + (0.308270676691729f * rect.Width), rect.Top + (0.901554404145078f * rect.Height));
            pts[16] = new SKPoint(rect.Left + (0.293233082706767f * rect.Width), rect.Top + (0.797927461139896f * rect.Height));
            pts[17] = new SKPoint(rect.Left + (0.105263157894737f * rect.Width), rect.Top + (0.735751295336788f * rect.Height));
            pts[18] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.585492227979275f * rect.Height));
            pts[19] = new SKPoint(rect.Left + (0.101503759398496f * rect.Width), rect.Top + (0.414507772020725f * rect.Height));
            pts[20] = new SKPoint(rect.Left + (0.236842105263158f * rect.Width), rect.Top + (0.409326424870466f * rect.Height));
            pts[21] = new SKPoint(rect.Left + (0.353383458646617f * rect.Width), rect.Top + (0.404145077720207f * rect.Height));
            pts[22] = new SKPoint(rect.Left + (0.466165413533835f * rect.Width), rect.Top + (0.0673575129533679f * rect.Height));
            pts[23] = new SKPoint(rect.Left + (0.537593984962406f * rect.Width), rect.Top + (0 * rect.Height));
            pts[24] = new SKPoint(rect.Left + (0.522556390977444f * rect.Width), rect.Top + (0.196891191709845f * rect.Height));
            pts[25] = new SKPoint(rect.Left + (0.552631578947368f * rect.Width), rect.Top + (0.393782383419689f * rect.Height));
            pts[26] = new SKPoint(rect.Left + (0.616541353383459f * rect.Width), rect.Top + (0.44559585492228f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _blackPaint);
            pts = new SKPoint[5];
            pts[0] = new SKPoint(rect.Left + (0.176691729323308f * rect.Width), rect.Top + (0.549222797927461f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.233082706766917f * rect.Width), rect.Top + (0.476683937823834f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.315789473684211f * rect.Width), rect.Top + (0.487046632124352f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.274436090225564f * rect.Width), rect.Top + (0.50259067357513f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.236842105263158f * rect.Width), rect.Top + (0.569948186528497f * rect.Height));
            canvas.DrawPoly(pts, _whitePaint);
            pts = new SKPoint[5];
            pts[0] = new SKPoint(rect.Left + (0.0601503759398496f * rect.Width), rect.Top + (0.663212435233161f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.116541353383459f * rect.Width), rect.Top + (0.606217616580311f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.236842105263158f * rect.Width), rect.Top + (0.673575129533679f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.296992481203008f * rect.Width), rect.Top + (0.77720207253886f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.135338345864662f * rect.Width), rect.Top + (0.730569948186529f * rect.Height));
            canvas.DrawPoly(pts, _whitePaint);
            pts = new SKPoint[7];
            pts[0] = new SKPoint(rect.Left + (0.394736842105263f * rect.Width), rect.Top + (0.77720207253886f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.421052631578947f * rect.Width), rect.Top + (0.709844559585492f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.503759398496241f * rect.Width), rect.Top + (0.689119170984456f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.593984962406015f * rect.Width), rect.Top + (0.575129533678757f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.763157894736842f * rect.Width), rect.Top + (0.61139896373057f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.68796992481203f * rect.Width), rect.Top + (0.652849740932642f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.492481203007519f * rect.Width), rect.Top + (0.761658031088083f * rect.Height));
            canvas.DrawPoly(pts, _whitePaint);
            rect = SKRect.Create(rect_Right.Left + (rect_Right.Width * 0.55f), rect_Right.Top + (rect_Right.Width * 0.75f), rect_Right.Width * 0.2f, rect_Right.Height * 0.15f);
            DrawFish(canvas, rect);
            rect = SKRect.Create(rect_Right.Left + (rect_Right.Width * 0.75f), rect_Right.Top + (rect_Right.Width * 0.85f), rect_Right.Width * 0.2f, rect_Right.Height * 0.15f);
            DrawFish(canvas, rect);
            canvas.DrawRect(rect_Right, pn_Temp);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawScholarshipCard(SKCanvas canvas, SKRect bounds)
        {
            DrawMiscCard(canvas, bounds, "Scholarship Drive", "$400 to charity", true);
            var rect_Right = SKRect.Create(bounds.Left + (bounds.Width * 0.5f), bounds.Top + (bounds.Height * 0.2f), bounds.Width * 0.45f, bounds.Height * 0.8f);
            SKPoint[] pts;
            SKPath gp = new SKPath();
            SKPaint br_Fill;
            SKRect rect;
            rect = rect_Right;
            pts = new SKPoint[15];
            pts[0] = new SKPoint(rect.Left + (0.309446254071661f * rect.Width), rect.Top + (0.337313432835821f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.201954397394137f * rect.Width), rect.Top + (0.0925373134328358f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.436482084690554f * rect.Width), rect.Top + (0.253731343283582f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.521172638436482f * rect.Width), rect.Top + (0 * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.609120521172638f * rect.Width), rect.Top + (0.244776119402985f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.840390879478827f * rect.Width), rect.Top + (0.125373134328358f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.716612377850163f * rect.Width), rect.Top + (0.346268656716418f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (1 * rect.Width), rect.Top + (0.376119402985075f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.745928338762215f * rect.Width), rect.Top + (0.465671641791045f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.827361563517915f * rect.Width), rect.Top + (0.719402985074627f * rect.Height));
            pts[10] = new SKPoint(rect.Left + (0.579804560260586f * rect.Width), rect.Top + (0.57910447761194f * rect.Height));
            pts[11] = new SKPoint(rect.Left + (0.0944625407166124f * rect.Width), rect.Top + (0.653731343283582f * rect.Height));
            pts[12] = new SKPoint(rect.Left + (0.306188925081433f * rect.Width), rect.Top + (0.486567164179104f * rect.Height));
            pts[13] = new SKPoint(rect.Left + (0 * rect.Width), rect.Top + (0.373134328358209f * rect.Height));
            pts[14] = new SKPoint(rect.Left + (0.315960912052117f * rect.Width), rect.Top + (0.346268656716418f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _lightYellowPaint);
            gp = new SKPath();
            pts = new SKPoint[9];
            pts[0] = new SKPoint(rect.Left + (0.299674267100977f * rect.Width), rect.Top + (0.617910447761194f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.439739413680782f * rect.Width), rect.Top + (0.588059701492537f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.563517915309446f * rect.Width), rect.Top + (0.674626865671642f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.602605863192182f * rect.Width), rect.Top + (0.665671641791045f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.657980456026059f * rect.Width), rect.Top + (0.602985074626866f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.775244299674267f * rect.Width), rect.Top + (0.638805970149254f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.742671009771987f * rect.Width), rect.Top + (0.558208955223881f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.729641693811075f * rect.Width), rect.Top + (0.450746268656716f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.37785016286645f * rect.Width), rect.Top + (0.438805970149254f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _silverPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[10];
            pts[0] = new SKPoint(rect.Left + (0.143322475570033f * rect.Width), rect.Top + (0.408955223880597f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.358306188925081f * rect.Width), rect.Top + (0.382089552238806f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.560260586319218f * rect.Width), rect.Top + (0.304477611940298f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.583061889250814f * rect.Width), rect.Top + (0.340298507462687f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.706840390879479f * rect.Width), rect.Top + (0.408955223880597f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.892508143322476f * rect.Width), rect.Top + (0.429850746268657f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.830618892508143f * rect.Width), rect.Top + (0.447761194029851f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.560260586319218f * rect.Width), rect.Top + (0.53134328358209f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.462540716612378f * rect.Width), rect.Top + (0.525373134328358f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.185667752442997f * rect.Width), rect.Top + (0.429850746268657f * rect.Height));
            gp.AddPoly(pts);
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.Silver, gp.Bounds, MiscHelpers.EnumLinearGradientPercent.Angle90);
            canvas.DrawPath(gp, br_Fill);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[3];
            pts[0] = new SKPoint(rect.Left + (0.732899022801303f * rect.Width), rect.Top + (0.576119402985075f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.817589576547231f * rect.Width), rect.Top + (0.573134328358209f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.820846905537459f * rect.Width), rect.Top + (0.632835820895522f * rect.Height));
            gp.AddLines(pts);
            gp.AddLine(new SKPoint(rect.Left + (0.498371335504886f * rect.Width), rect.Top + (0.916417910447761f * rect.Height)), new SKPoint(rect.Left + (0.384364820846906f * rect.Width), rect.Top + (0.814925373134328f * rect.Height)));
            gp.Close();
            canvas.DrawPath(gp, _silverPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[5];
            pts[0] = new SKPoint(rect.Left + (0.423452768729642f * rect.Width), rect.Top + (0.925373134328358f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.358306188925081f * rect.Width), rect.Top + (0.86865671641791f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.39413680781759f * rect.Width), rect.Top + (0.794029850746269f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.495114006514658f * rect.Width), rect.Top + (0.817910447761194f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.50814332247557f * rect.Width), rect.Top + (0.898507462686567f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _silverPaint);
            canvas.DrawPath(gp, _border1Paint);
            gp = new SKPath();
            pts = new SKPoint[10];
            pts[0] = new SKPoint(rect.Left + (0.553745928338762f * rect.Width), rect.Top + (0.683582089552239f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.651465798045603f * rect.Width), rect.Top + (0.71044776119403f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.654723127035831f * rect.Width), rect.Top + (0.797014925373134f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.785016286644951f * rect.Width), rect.Top + (0.808955223880597f * rect.Height));
            pts[4] = new SKPoint(rect.Left + (0.798045602605863f * rect.Width), rect.Top + (0.767164179104478f * rect.Height));
            pts[5] = new SKPoint(rect.Left + (0.732899022801303f * rect.Width), rect.Top + (0.767164179104478f * rect.Height));
            pts[6] = new SKPoint(rect.Left + (0.801302931596091f * rect.Width), rect.Top + (0.728358208955224f * rect.Height));
            pts[7] = new SKPoint(rect.Left + (0.732899022801303f * rect.Width), rect.Top + (0.722388059701493f * rect.Height));
            pts[8] = new SKPoint(rect.Left + (0.677524429967427f * rect.Width), rect.Top + (0.653731343283582f * rect.Height));
            pts[9] = new SKPoint(rect.Left + (0.596091205211726f * rect.Width), rect.Top + (0.653731343283582f * rect.Height));
            gp.AddPoly(pts);
            canvas.DrawPath(gp, _redPaint);
            pts = new SKPoint[4];
            pts[0] = new SKPoint(rect.Left + (0.472312703583062f * rect.Width), rect.Top + (0.922388059701493f * rect.Height));
            pts[1] = new SKPoint(rect.Left + (0.433224755700326f * rect.Width), rect.Top + (0.88955223880597f * rect.Height));
            pts[2] = new SKPoint(rect.Left + (0.47557003257329f * rect.Width), rect.Top + (0.853731343283582f * rect.Height));
            pts[3] = new SKPoint(rect.Left + (0.478827361563518f * rect.Width), rect.Top + (0.886567164179105f * rect.Height));
            canvas.DrawLines(pts, _border1Paint);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawMonsterCharge(SKCanvas canvas, SKRect bounds, string str_Total, string str_Due)
        {
            var rect_Top = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height / 50), bounds.Width, bounds.Height * 0.3f);
            var rect_Left = SKRect.Create(bounds.Left, bounds.Top + (bounds.Height * 0.3f), bounds.Width / 2, bounds.Height * 0.7f);
            var rect_Right = SKRect.Create(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height * 0.3f), bounds.Width / 2, bounds.Height * 0.7f);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.17f);
            textPaint.FakeBoldText = true;
            canvas.DrawCustomText("Monster Charge", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Top, out _);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height * 0.11f);
            textPaint.FakeBoldText = true;
            canvas.DrawCustomText("Total Charges", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Left, out _);
            canvas.DrawCustomText("Interest Due", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Right, out _);
            rect_Left = SKRect.Create(rect_Left.Left, bounds.Top + (bounds.Height * 0.65f), bounds.Width / 2, bounds.Height / 4);
            rect_Right = SKRect.Create(rect_Right.Left, bounds.Top + (bounds.Height * 0.65f), bounds.Width / 2, bounds.Height / 4);
            rect_Left = SKRect.Create(rect_Left.Left, bounds.Top + (bounds.Height * 0.7f), bounds.Width / 2, bounds.Height / 4);
            rect_Right = SKRect.Create(rect_Right.Left, bounds.Top + (bounds.Height * 0.7f), bounds.Width / 2, bounds.Height / 4);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.17f);
            canvas.DrawCustomText(str_Total, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Left, out _);
            canvas.DrawCustomText(str_Due, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect_Right, out _);
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawCardFromTemplate(string str_Type, SKCanvas canvas, SKRect bounds, string str_Line1, SKColor clr_1, string str_Line2, SKColor clr_2, string str_Line3, SKColor clr_3, string str_Line4, SKColor clr_4, string str_Line5, SKColor clr_5)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height * 0.13f);
            textPaint.FakeBoldText = true;
            var rect_Top = SKRect.Create(bounds.Left, bounds.Top + 1, bounds.Width, bounds.Height * 0.2f);
            SKRect rect;
            int int_Count;
            string str_Current;
            SKColor clr_Current;
            float Left;
            float Top;
            Left = rect_Top.Left + (rect_Top.Width / 2);
            Top = rect_Top.Top + (bounds.Height / 40);
            if (str_Type == "Bill")
            {
                for (int_Count = 0; int_Count <= 2; int_Count++)
                {
                    canvas.DrawLine(new SKPoint(bounds.Left + (bounds.Width * 0.05f), bounds.Top + (rect_Top.Height / 6) + (rect_Top.Height / 6) * int_Count), new SKPoint(bounds.Left + (bounds.Width * 0.3f), bounds.Top + (rect_Top.Height / 6) + (rect_Top.Height / 6) * int_Count), _redBorder);
                    canvas.DrawLine(new SKPoint(bounds.Left + (bounds.Width * 0.95f), bounds.Top + (rect_Top.Height / 6) + (rect_Top.Height / 6) * int_Count), new SKPoint(bounds.Left + (bounds.Width * 0.7f), bounds.Top + (rect_Top.Height / 6) + (rect_Top.Height / 6) * int_Count), _redBorder);
                }
                textPaint.Color = SKColors.Red;
                canvas.DrawText("Bill", Left, Top, true, textPaint);
            }
            else if (str_Type == "Neighbor")
            {
                textPaint.Color = SKColors.Blue;
                canvas.DrawText("Pay A Neighbor", Left, Top, true, textPaint);
            }
            else if (str_Type == "MadMoney")
            {
                textPaint.Color = SKColors.Green;
                canvas.DrawText("Mad Money", Left, Top, true, textPaint);
            }
            else if (str_Type == "MoveAhead")
            {
                textPaint.Color = SKColors.Green;
                canvas.DrawText("Move Ahead", Left, Top, true, textPaint);
            }
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height * 0.12f);
            textPaint.FakeBoldText = true;
            if ((str_Type == "MoveAhead") | (str_Type == "Neighbor"))
            {
                textPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height * 0.14f);
                textPaint.FakeBoldText = true;
            }
            for (int_Count = 0; int_Count <= 4; int_Count++)
            {
                if (!string.IsNullOrEmpty(str_Line5))
                    rect = SKRect.Create(bounds.Left, ((bounds.Top + (rect_Top.Height)) - (bounds.Height / 30)) + (((bounds.Height - rect_Top.Height) / 5) * int_Count), bounds.Width, ((bounds.Height - rect_Top.Height) / 5));
                else if (!string.IsNullOrEmpty(str_Line4))
                    rect = SKRect.Create(bounds.Left, ((bounds.Top + (rect_Top.Height)) - (bounds.Height / 30)) + (((bounds.Height - rect_Top.Height) / 4) * int_Count), bounds.Width, ((bounds.Height - rect_Top.Height) / 4));
                else
                    rect = SKRect.Create(bounds.Left, ((bounds.Top + (rect_Top.Height)) - (bounds.Height / 30)) + (((bounds.Height - rect_Top.Height) / 3) * int_Count), bounds.Width, ((bounds.Height - rect_Top.Height) / 3));
                switch (int_Count)
                {
                    case 0:
                        {
                            str_Current = str_Line1;
                            clr_Current = clr_1;
                            break;
                        }

                    case 1:
                        {
                            str_Current = str_Line2;
                            clr_Current = clr_2;
                            break;
                        }

                    case 2:
                        {
                            str_Current = str_Line3;
                            clr_Current = clr_3;
                            break;
                        }

                    case 3:
                        {
                            str_Current = str_Line4;
                            clr_Current = clr_4;
                            break;
                        }
                    default:
                        {
                            str_Current = str_Line5;
                            clr_Current = clr_5;
                            break;
                        }
                }
                if (str_Current == "deal or buyer")
                {
                    var topRect = SKRect.Create(bounds.Left + (bounds.Width * 0.3f), bounds.Top, bounds.Width, bounds.Height);
                    var bottomRect = SKRect.Create(bounds.Left + (bounds.Width * 0.72f), bounds.Top, bounds.Width, bounds.Height); // 
                    textPaint.Color = SKColors.Red;
                    canvas.DrawCustomText("deal", TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, topRect, out _);
                    textPaint.Color = SKColors.Blue;
                    canvas.DrawCustomText("buyer", TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, bottomRect, out _);
                    textPaint.Color = SKColors.Black;
                    canvas.DrawCustomText("or", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect, out _);
                }
                else
                {
                    textPaint.Color = clr_Current;
                    canvas.DrawCustomText(str_Current, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, rect, out _);
                }
            }
            canvas.DrawRect(bounds, _redBorder);
        }
        private void DrawPayANeighbor(SKCanvas canvas, SKRect bounds, string str_Line1, SKColor clr_1, string str_Line2, SKColor clr_2, string str_Line3, SKColor clr_3, string str_Line4, SKColor clr_4)
        {
            DrawCardFromTemplate("Neighbor", canvas, bounds, str_Line1, clr_1, str_Line2, clr_2, str_Line3, clr_3, str_Line4, clr_4, "", SKColors.Black);
        }
        private void DrawBill(SKCanvas canvas, SKRect bounds, string str_Line1, SKColor clr_1, string str_Line2, SKColor clr_2, string str_Line3, SKColor clr_3, string str_Line4, SKColor clr_4, string str_Line5, SKColor clr_5)
        {
            DrawCardFromTemplate("Bill", canvas, bounds, str_Line1, clr_1, str_Line2, clr_2, str_Line3, clr_3, str_Line4, clr_4, str_Line5, clr_5);
        }
        private void DrawMadMoney(SKCanvas canvas, SKRect bounds, string str_Line1, SKColor clr_1, string str_Line2, SKColor clr_2, string str_Line3, SKColor clr_3, string str_Line4, SKColor clr_4, string str_Line5, SKColor clr_5)
        {
            DrawCardFromTemplate("MadMoney", canvas, bounds, str_Line1, clr_1, str_Line2, clr_2, str_Line3, clr_3, str_Line4, clr_4, str_Line5, clr_5);
        }
        private void DrawMoveAhead(SKCanvas canvas, SKRect bounds)
        {
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Green, bounds.Height * 0.7f * 0.2f);
            textPaint.FakeBoldText = true;
            var thisRect = SKRect.Create(bounds.Left, bounds.Top, bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("Move Ahead", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisRect, out _);
            var nextRect = SKRect.Create(bounds.Left, thisRect.Bottom, bounds.Width, bounds.Height * 0.2f);
            textPaint.Color = SKColors.Black;
            canvas.DrawCustomText("to the next", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, nextRect, out _);
            textPaint.Color = SKColors.Red;
            var thisBottom = nextRect.Bottom;
            string dealText;
            dealText = "deal ";
            SKPaint dealPaint;
            dealPaint = MiscHelpers.GetTextPaint(SKColors.Red, bounds.Height * 0.7f * 0.2f);
            var dealWidth = dealText.CalculateTextWidth(dealPaint);
            dealWidth += 3;
            var dealRect = SKRect.Create(bounds.Left + 3, thisBottom, dealWidth, bounds.Height * 0.2f);
            var orPaint = MiscHelpers.GetTextPaint(SKColors.Black, bounds.Height * 0.7f * 0.2f);
            string orText;
            orText = "or ";
            var orWidth = orText.CalculateTextWidth(orPaint);
            orWidth += 3;
            var orRect = SKRect.Create(0, thisBottom + 2, orWidth, bounds.Height * 0.2f);
            string buyText;
            buyText = "buyer";
            var buyPaint = MiscHelpers.GetTextPaint(SKColors.Blue, bounds.Height * 0.7f * 0.2f);
            var buyWidth = buyText.CalculateTextWidth(textPaint);
            var buyRect = SKRect.Create(0, thisBottom, buyWidth, bounds.Height * 0.2f);
            var totals = dealRect.Width + orRect.Width + buyRect.Width;
            var margins = (bounds.Width - totals) / 2;
            dealRect = SKRect.Create(margins, thisBottom, dealWidth, bounds.Height * 0.2f);
            orRect = SKRect.Create(dealRect.Right, thisBottom + 3, orWidth, bounds.Height * 0.2f);
            buyRect = SKRect.Create(orRect.Right, thisBottom, buyWidth, bounds.Height * 0.2f);
            canvas.DrawCustomText(dealText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, dealPaint, dealRect, out _);
            canvas.DrawCustomText(orText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, orPaint, orRect, out _);
            canvas.DrawCustomText(buyText, TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, buyPaint, buyRect, out _);
            nextRect = SKRect.Create(bounds.Left, buyRect.Bottom, bounds.Width, bounds.Height * 0.2f);
            textPaint.Color = SKColors.Black;
            canvas.DrawCustomText("space", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, nextRect, out _);
            thisRect = SKRect.Create(bounds.Left, nextRect.Bottom, bounds.Width, bounds.Height * 0.2f);
            canvas.DrawCustomText("NOW!", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisRect, out _);
        }
        #endregion
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            if (CardCategory == EnumCardCategory.Deal)
                DraweDealData(canvas, rect_Card);
            else
                DrawMail(canvas, rect_Card);
        }
    }
}