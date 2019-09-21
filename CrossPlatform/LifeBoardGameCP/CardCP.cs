using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace LifeBoardGameCP
{
    public class CardCP : ObservableObject, IDeckGraphicsCP
    {

        private EnumCardCategory _CardCategory = EnumCardCategory.None;
        public EnumCardCategory CardCategory
        {
            get
            {
                return _CardCategory;
            }

            set
            {
                if (SetProperty(ref _CardCategory, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private EnumCareerType _Career;
        public EnumCareerType Career
        {
            get
            {
                return _Career;
            }

            set
            {
                if (SetProperty(ref _Career, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private decimal _CollectAmount;
        public decimal CollectAmount
        {
            get
            {
                return _CollectAmount;
            }

            set
            {
                if (SetProperty(ref _CollectAmount, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private decimal _TaxAmount;
        public decimal TaxAmount
        {
            get
            {
                return _TaxAmount;
            }

            set
            {
                if (SetProperty(ref _TaxAmount, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private int _StockValue;
        public int StockValue
        {
            get
            {
                return _StockValue;
            }

            set
            {
                if (SetProperty(ref _StockValue, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private EnumHouseType _House;
        public EnumHouseType House
        {
            get
            {
                return _House;
            }

            set
            {
                if (SetProperty(ref _House, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        private decimal _HouseValue;
        public decimal HouseValue
        {
            get
            {
                return _HouseValue;
            }

            set
            {
                if (SetProperty(ref _HouseValue, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private bool _Drew;
        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
                    MainGraphics?.PaintUI?.DoInvalidate();

            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown == true)
                return true; //default to true.  change to what you need to start drawing.
            return CardCategory != EnumCardCategory.None;
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
            SKRect firstRect;
            SKRect secondRect;
            SKRect thirdRect;
            SKRect fourthRect;
            firstRect = MainGraphics!.GetActualRectangle(8, 8, 15, 34);
            secondRect = MainGraphics.GetActualRectangle(23, 8, 11, 34);
            thirdRect = MainGraphics.GetActualRectangle(34, 8, 19, 34);
            fourthRect = MainGraphics.GetActualRectangle(53, 8, 19, 34);
            SKRect bottomRect;
            bottomRect = MainGraphics.GetActualRectangle(8, 50, 64, 42);
            var fontSize = firstRect.Height * 0.6f;
            canvas.DrawRect(firstRect, _purplePaint);
            canvas.DrawRect(firstRect, _blackBorder);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            canvas.DrawBorderText("L", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, firstRect);
            canvas.DrawRect(secondRect, _bluePaint);
            canvas.DrawRect(secondRect, _blackBorder);
            canvas.DrawBorderText("I", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, secondRect);
            canvas.DrawRect(thirdRect, _greenPaint);
            canvas.DrawRect(thirdRect, _blackBorder);
            canvas.DrawBorderText("F", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, thirdRect);
            canvas.DrawRect(fourthRect, _darkOrangePaint);
            canvas.DrawRect(fourthRect, _blackBorder);
            canvas.DrawBorderText("E", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, fourthRect);
            textPaint = MiscHelpers.GetTextPaint(SKColors.White, bottomRect.Height * 0.45f);
            string text;
            SKPaint currentPaint;
            switch (CardCategory)
            {
                case EnumCardCategory.Career:
                    {
                        currentPaint = _bluePaint!;
                        text = "Career";
                        break;
                    }

                case EnumCardCategory.House:
                    {
                        currentPaint = _darkOrangePaint!;
                        text = "House";
                        break;
                    }

                case EnumCardCategory.Salary:
                    {
                        currentPaint = _greenPaint!;
                        text = "Salary";
                        break;
                    }

                case EnumCardCategory.Stock:
                    {
                        currentPaint = _purplePaint!;
                        text = "Stock";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("No type selected for back of cards");
                    }
            }
            canvas.DrawRect(bottomRect, currentPaint);
            canvas.DrawRect(bottomRect, _blackBorder);
            canvas.DrawBorderText(text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, bottomRect);
        }
        public SKColor GetFillColor()
        {
            if (MainGraphics!.IsUnknown == true)
                return SKColors.Green; //this is the color of the back card if we have nothing else to it.
            return SKColors.White;
        }
        public SKRect GetDrawingRectangle()
        {
            return MainGraphics!.GetDrawingRectangle();
        }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _whiteBorder;
        private SKPaint? _steelBluePaint;
        private SKPaint? _whitePaint;
        private SKPaint? _bluePaint;
        private SKPaint? _darkOrangePaint;
        private SKPaint? _blackBorder;
        private SKPaint? _purplePaint;
        private SKPaint? _greenPaint;
        private Assembly? _thisA;
        public void Init()
        {
            _thisA = Assembly.GetAssembly(GetType());
            MainGraphics!.OriginalSize = new SKSize(80, 100); //change to what the original size is.
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _whiteBorder = MiscHelpers.GetStrokePaint(SKColors.White, 1);
            _steelBluePaint = MiscHelpers.GetSolidPaint(SKColors.SteelBlue);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _darkOrangePaint = MiscHelpers.GetSolidPaint(SKColors.DarkOrange);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _purplePaint = MiscHelpers.GetSolidPaint(SKColors.Purple);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green); //no option to show you can select unknown cards.
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            switch (CardCategory)
            {
                case EnumCardCategory.Career:
                    DrawCareerCard(canvas);
                    break;
                case EnumCardCategory.House:
                    DrawHouseCard(canvas);
                    break;
                case EnumCardCategory.Salary:
                    DrawSalaryCard(canvas);
                    break;
                case EnumCardCategory.Stock:
                    DrawStockCard(canvas);
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
        }
        private void DrawSalaryCard(SKCanvas canvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(3, 3, 70, 20);
            var secondRect = MainGraphics.GetActualRectangle(3, 24, 70, 20);
            var thirdRect = MainGraphics.GetActualRectangle(3, 55, 70, 20);
            var fourthRect = MainGraphics.GetActualRectangle(3, 76, 70, 20);
            var fontSize = firstRect.Height * 0.75f; // trial and error
            var blackText = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            SKColor otherColor;
            blackText.FakeBoldText = true;
            switch (CollectAmount)
            {
                case var @case when @case == 100000:
                    {
                        otherColor = SKColors.DarkOrange;
                        break;
                    }
                case var case1 when case1 == 90000:
                case var case2 when case2 == 70000:
                case var case3 when case3 == 60000:
                    {
                        otherColor = SKColors.Green;
                        break;
                    }
                case var case4 when case4 == 80000:
                case var case5 when case5 == 30000:
                case var case6 when case6 == 40000:
                    {
                        otherColor = SKColors.Red;
                        break;
                    }
                case var case7 when case7 == 50000:
                case var case8 when case8 == 20000:
                    {
                        otherColor = SKColors.DarkBlue;
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("The collect amount is wrong. You entered " + CollectAmount);
                    }
            }
            var otherText = MiscHelpers.GetTextPaint(otherColor, fontSize);
            otherText.FakeBoldText = true;
            canvas.DrawCustomText("Collect", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, blackText, firstRect, out _);
            canvas.DrawCustomText(CollectAmount.ToCurrency(0), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, otherText, secondRect, out _);
            canvas.DrawCustomText("Taxes Due", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, blackText, thirdRect, out _);
            canvas.DrawCustomText(TaxAmount.ToCurrency(0), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, blackText, fourthRect, out _);
        }
        private void DrawStockCard(SKCanvas canvas)
        {
            var firstRect = MainGraphics!.GetActualRectangle(3, 10, 70, 20);
            var secondRect = MainGraphics.GetActualRectangle(3, 31, 70, 20);
            var thirdRect = MainGraphics.GetActualRectangle(3, 51, 70, 20);
            var fourthRect = MainGraphics.GetActualRectangle(3, 76, 70, 20);
            var fontSize = firstRect.Height * 0.75f; // trial and error
            var otherSize = firstRect.Height * 1.2f;
            var otherText = MiscHelpers.GetTextPaint(SKColors.Purple, otherSize);
            var blackText = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            blackText.FakeBoldText = true;
            otherText.FakeBoldText = true;
            canvas.DrawCustomText(StockValue.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, otherText, firstRect, out _);
            canvas.DrawCustomText("Stock", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, blackText, secondRect, out _);
            canvas.DrawCustomText("Certificate", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, blackText, thirdRect, out _);
            canvas.DrawCustomText("$50,000", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, blackText, fourthRect, out _); // its okay to do manually
        }
        private void DrawHouseCard(SKCanvas canvas)
        {
            var pictRect = MainGraphics!.GetActualRectangle(20, 8, 40, 40);
            var firstRect = MainGraphics.GetActualRectangle(8, 65, 64, 27);
            var secondRect = MainGraphics.GetActualRectangle(8, 48, 64, 17);
            SKBitmap thisBit;
            thisBit = ImageExtensions.GetSkBitmap(_thisA, House.ToString() + ".png");
            canvas.DrawBitmap(thisBit, pictRect, MainGraphics.BitPaint);
            var thisText = House.ToString().GetWords();
            var firstText = MiscHelpers.GetTextPaint(SKColors.White, firstRect.Height * 0.4f);
            var secondText = MiscHelpers.GetTextPaint(SKColors.Black, secondRect.Height * 0.8f);
            secondText.FakeBoldText = true;
            canvas.DrawRect(firstRect, _darkOrangePaint);
            if ((int)House == (int)EnumHouseType.DutchColonial)
            {
                firstRect = MainGraphics.GetActualRectangle(8, 63, 64, 17);
                canvas.DrawCustomText("Dutch", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstText, firstRect, out _);
                firstRect = MainGraphics.GetActualRectangle(8, 75, 64, 17);
                canvas.DrawCustomText("Colonial", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstText, firstRect, out _);
            }
            else
                canvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, firstText, firstRect, out _);
            canvas.DrawCustomText(HouseValue.ToCurrency(0), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, secondText, secondRect, out _);
        }
        private void DrawCareerCard(SKCanvas canvas)
        {
            var pictRect = MainGraphics!.GetActualRectangle(15, 7, 50, 50);
            var firstRect = MainGraphics.GetActualRectangle(8, 48, 32, 10);
            var secondRect = MainGraphics.GetActualRectangle(40, 48, 32, 10);
            var centerRect = MainGraphics.GetActualRectangle(30, 43, 20, 20);
            string thisText;
            SKColor firstColor;
            SKColor secondColor;
            switch (Career)
            {
                case EnumCareerType.Accountant:
                    {
                        firstColor = SKColors.DarkBlue;
                        secondColor = SKColors.Red;
                        thisText = "Accountant";
                        break;
                    }

                case EnumCareerType.Artist:
                    {
                        firstColor = SKColors.DarkBlue;
                        secondColor = SKColors.Red;
                        thisText = "Artist";
                        break;
                    }

                case EnumCareerType.Athlete:
                    {
                        firstColor = SKColors.DarkBlue;
                        secondColor = SKColors.Red;
                        thisText = "Athlete";
                        break;
                    }

                case EnumCareerType.ComputerConsultant:
                    {
                        firstColor = SKColors.DarkBlue;
                        secondColor = SKColors.Green;
                        thisText = "Computer Consultant";
                        break;
                    }

                case EnumCareerType.Doctor:
                    {
                        firstColor = SKColors.DarkOrange;
                        secondColor = SKColors.Green;
                        thisText = "Doctor";
                        break;
                    }

                case EnumCareerType.Entertainer:
                    {
                        firstColor = SKColors.DarkBlue;
                        secondColor = SKColors.Red;
                        thisText = "Entertainer";
                        break;
                    }

                case EnumCareerType.PoliceOfficer:
                    {
                        firstColor = SKColors.Red;
                        secondColor = SKColors.Green;
                        thisText = "Police Officer";
                        break;
                    }

                case EnumCareerType.SalesPerson:
                    {
                        firstColor = SKColors.Red;
                        secondColor = SKColors.Green;
                        thisText = "Sales Person";
                        break;
                    }

                case EnumCareerType.Teacher:
                    {
                        firstColor = SKColors.Red;
                        secondColor = SKColors.Green;
                        thisText = "Teacher";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("No career selected");
                    }
            }
            SKBitmap thisBit;
            thisBit = ImageExtensions.GetSkBitmap(_thisA, Career.ToString() + ".png");
            canvas.DrawBitmap(thisBit, pictRect, MainGraphics.BitPaint);
            var firstPaint = MiscHelpers.GetSolidPaint(firstColor);
            var secondPaint = MiscHelpers.GetSolidPaint(secondColor);
            canvas.DrawRect(firstRect, firstPaint);
            canvas.DrawRect(secondRect, secondPaint);
            DrawCareerSymbol(canvas, centerRect, Career);
            var lastRect = MainGraphics.GetActualRectangle(8, 65, 64, 27);
            canvas.DrawRect(lastRect, _bluePaint);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, lastRect.Height * 0.4f);
            if ((int)Career != (int)EnumCareerType.ComputerConsultant)
                canvas.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, lastRect, out _);
            else
            {
                var Text1 = MainGraphics.GetActualRectangle(8, 65, 64, 14);
                canvas.DrawCustomText("Computer", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, Text1, out _);
                var Text2 = MainGraphics.GetActualRectangle(8, 75, 64, 14);
                canvas.DrawCustomText("Consultant", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, Text2, out _);
            }
        }

        private float _symbolHeightWidth;
        private float ActualSize(int original)
        {
            float divs;
            divs = _symbolHeightWidth / 20;
            return divs * original;
        }
        private void DrawCareerSymbol(SKCanvas canvas, SKRect rect_Career, EnumCareerType whatCareer)
        {
            _symbolHeightWidth = rect_Career.Height;
            SKPath gp_Career;
            SKPaint pn_Career;
            SKPoint[] pts_Curve = new SKPoint[3];
            SKMatrix tmp_Matrix = new SKMatrix();
            canvas.DrawOval(rect_Career, _steelBluePaint);
            switch (whatCareer)
            {
                case EnumCareerType.Artist:
                    {
                        tmp_Matrix.RotateAt(45, new SKPoint(rect_Career.Left + (rect_Career.Width / 2), rect_Career.Top + (rect_Career.Height / 2)));
                        gp_Career = new SKPath();
                        gp_Career.AddRect(SKRect.Create(rect_Career.Left + ActualSize(9), rect_Career.Top + ActualSize(7), ActualSize(2), ActualSize(11)));
                        pts_Curve = new SKPoint[6];
                        pts_Curve[0] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(7));
                        pts_Curve[1] = new SKPoint(rect_Career.Left + ActualSize(9), rect_Career.Top + ActualSize(4));
                        pts_Curve[2] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(1));
                        pts_Curve[3] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(1));
                        pts_Curve[4] = new SKPoint(rect_Career.Left + ActualSize(11), rect_Career.Top + ActualSize(4));
                        pts_Curve[5] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(7));
                        gp_Career.AddPoly(pts_Curve);
                        gp_Career.Transform(tmp_Matrix);
                        canvas.DrawPath(gp_Career, _whitePaint);
                        break;
                    }

                case EnumCareerType.Athlete:
                    {
                        gp_Career = new SKPath();
                        gp_Career.AddLine(rect_Career.Left + ActualSize(14), rect_Career.Top + ActualSize(3), rect_Career.Left + ActualSize(6), rect_Career.Top + ActualSize(3), true);
                        gp_Career.ArcTo(SKRect.Create(rect_Career.Left + ActualSize(6), rect_Career.Top + ActualSize(3), ActualSize(5), ActualSize(8)), 180, -90, false);
                        gp_Career.AddLine(rect_Career.Left + ActualSize(9), rect_Career.Top + ActualSize(15), rect_Career.Left + ActualSize(9), rect_Career.Top + ActualSize(16));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(9), rect_Career.Top + ActualSize(16), rect_Career.Left + ActualSize(7), rect_Career.Top + ActualSize(16));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(7), rect_Career.Top + ActualSize(16), rect_Career.Left + ActualSize(7), rect_Career.Top + ActualSize(18));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(7), rect_Career.Top + ActualSize(18), rect_Career.Left + ActualSize(13), rect_Career.Top + ActualSize(18));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(13), rect_Career.Top + ActualSize(18), rect_Career.Left + ActualSize(13), rect_Career.Top + ActualSize(16));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(13), rect_Career.Top + ActualSize(16), rect_Career.Left + ActualSize(11), rect_Career.Top + ActualSize(16));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(11), rect_Career.Top + ActualSize(16), rect_Career.Left + ActualSize(11), rect_Career.Top + ActualSize(15));
                        gp_Career.ArcTo(SKRect.Create(rect_Career.Left + ActualSize(9), rect_Career.Top + ActualSize(3), ActualSize(5), ActualSize(8)), 90, -90, false);
                        gp_Career.Close();
                        canvas.DrawPath(gp_Career, _whitePaint);
                        pts_Curve[0] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(3));
                        pts_Curve[1] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(7));
                        pts_Curve[2] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(12));
                        canvas.DrawLines(pts_Curve, _whiteBorder);
                        pts_Curve[0] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(3));
                        pts_Curve[1] = new SKPoint(rect_Career.Left + ActualSize(16), rect_Career.Top + ActualSize(7));
                        pts_Curve[2] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(12));
                        canvas.DrawLines(pts_Curve, _whiteBorder);
                        break;
                    }

                case EnumCareerType.Entertainer:
                    {
                        gp_Career = new SKPath();
                        gp_Career.AddLine(rect_Career.Left + ActualSize(10), rect_Career.Top, rect_Career.Left + ActualSize(8), rect_Career.Top + ActualSize(7), true);
                        gp_Career.AddLine(rect_Career.Left + ActualSize(2), rect_Career.Top + ActualSize(7), rect_Career.Left + ActualSize(7), rect_Career.Top + ActualSize(12));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(5), rect_Career.Top + ActualSize(18), rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(14));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(15), rect_Career.Top + ActualSize(18), rect_Career.Left + ActualSize(14), rect_Career.Top + ActualSize(12));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(18), rect_Career.Top + ActualSize(7), rect_Career.Left + ActualSize(12), rect_Career.Top + ActualSize(7));
                        gp_Career.Close();
                        canvas.DrawPath(gp_Career, _whitePaint);
                        break;
                    }

                case EnumCareerType.Doctor:
                    {
                        pn_Career = MiscHelpers.GetStrokePaint(SKColors.White, ActualSize(6));
                        canvas.DrawLine(System.Convert.ToInt32((rect_Career.Left + (rect_Career.Width / 2))), rect_Career.Top + ActualSize(2), System.Convert.ToInt32((rect_Career.Left + (rect_Career.Width / 2))), (rect_Career.Top + rect_Career.Height) - ActualSize(2), pn_Career);
                        canvas.DrawLine(rect_Career.Left + ActualSize(2), System.Convert.ToInt32((rect_Career.Top + (rect_Career.Height / 2))), (rect_Career.Left + rect_Career.Width) - ActualSize(2), System.Convert.ToInt32((rect_Career.Top + (rect_Career.Height / 2))), pn_Career);
                        break;
                    }

                case EnumCareerType.Teacher:
                    {
                        canvas.DrawOval(SKRect.Create(rect_Career.Left + ActualSize(3), rect_Career.Top + ActualSize(6), ActualSize(14), ActualSize(12)), _whitePaint);
                        gp_Career = new SKPath();
                        pts_Curve = new SKPoint[9];
                        pts_Curve[0] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(6));
                        pts_Curve[1] = new SKPoint(rect_Career.Left + ActualSize(8), rect_Career.Top + ActualSize(4));
                        pts_Curve[2] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(2));
                        pts_Curve[3] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(2));
                        pts_Curve[4] = new SKPoint(rect_Career.Left + ActualSize(5), rect_Career.Top + ActualSize(5));
                        pts_Curve[5] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(6));
                        pts_Curve[6] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(6));
                        pts_Curve[7] = new SKPoint(rect_Career.Left + ActualSize(12), rect_Career.Top + ActualSize(4));
                        pts_Curve[8] = new SKPoint(rect_Career.Left + ActualSize(13), rect_Career.Top + ActualSize(2));
                        gp_Career.AddPoly(pts_Curve);
                        canvas.DrawPath(gp_Career, _whitePaint);
                        break;
                    }

                case EnumCareerType.Accountant:
                    {
                        var TextPaint = MiscHelpers.GetTextPaint(SKColors.White, ActualSize(16), "Times New Roman");
                        canvas.DrawCustomText("$", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, TextPaint, rect_Career, out _);
                        break;
                    }

                case EnumCareerType.PoliceOfficer:
                    {
                        gp_Career = new SKPath();
                        pts_Curve = new SKPoint[18];
                        pts_Curve[0] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(4));
                        pts_Curve[1] = new SKPoint(rect_Career.Left + ActualSize(7), rect_Career.Top + ActualSize(6));
                        pts_Curve[2] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(4));
                        pts_Curve[3] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(4));
                        pts_Curve[4] = new SKPoint(rect_Career.Left + ActualSize(13), rect_Career.Top + ActualSize(6));
                        pts_Curve[5] = new SKPoint(rect_Career.Left + ActualSize(16), rect_Career.Top + ActualSize(4));
                        pts_Curve[6] = new SKPoint(rect_Career.Left + ActualSize(17), rect_Career.Top + ActualSize(5));
                        pts_Curve[7] = new SKPoint(rect_Career.Left + ActualSize(16), rect_Career.Top + ActualSize(6));
                        pts_Curve[8] = new SKPoint(rect_Career.Left + ActualSize(16), rect_Career.Top + ActualSize(7));
                        pts_Curve[9] = new SKPoint(rect_Career.Left + ActualSize(16), rect_Career.Top + ActualSize(7));
                        pts_Curve[10] = new SKPoint(rect_Career.Left + ActualSize(15), rect_Career.Top + ActualSize(14));
                        pts_Curve[11] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(18));
                        pts_Curve[12] = new SKPoint(rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(18));
                        pts_Curve[13] = new SKPoint(rect_Career.Left + ActualSize(5), rect_Career.Top + ActualSize(14));
                        pts_Curve[14] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(7));
                        pts_Curve[15] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(7));
                        pts_Curve[16] = new SKPoint(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(6));
                        pts_Curve[17] = new SKPoint(rect_Career.Left + ActualSize(3), rect_Career.Top + ActualSize(5));
                        gp_Career.AddPoly(pts_Curve);
                        canvas.DrawPath(gp_Career, _whitePaint);
                        break;
                    }

                case EnumCareerType.SalesPerson:
                    {
                        gp_Career = new SKPath();
                        gp_Career.AddLine(rect_Career.Left + ActualSize(2), rect_Career.Top + ActualSize(10), rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(18));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(18), rect_Career.Top + ActualSize(10), rect_Career.Left + ActualSize(10), rect_Career.Top + ActualSize(2));
                        gp_Career.AddLine(rect_Career.Left + ActualSize(6), rect_Career.Top + ActualSize(2), rect_Career.Left + ActualSize(2), rect_Career.Top + ActualSize(6));
                        gp_Career.Close();
                        gp_Career.AddOval(SKRect.Create(rect_Career.Left + ActualSize(4), rect_Career.Top + ActualSize(4), ActualSize(2), ActualSize(2)));
                        canvas.DrawPath(gp_Career, _whitePaint);
                        break;
                    }

                case EnumCareerType.ComputerConsultant:
                    {
                        canvas.DrawRect(SKRect.Create(rect_Career.Left + ActualSize(5), rect_Career.Top + ActualSize(2), ActualSize(10), ActualSize(8)), _whitePaint);
                        canvas.DrawRect(SKRect.Create(rect_Career.Left + ActualSize(3), rect_Career.Top + ActualSize(12), ActualSize(14), ActualSize(4)), _whitePaint);
                        break;
                    }
            }
            canvas.DrawRect(rect_Career, _whiteBorder);
        }
    }
}