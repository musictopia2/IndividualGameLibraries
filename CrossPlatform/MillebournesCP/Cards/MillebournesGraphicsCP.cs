using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MillebournesCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace MillebournesCP.Cards
{
    public class MillebournesGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }

        private EnumCompleteCategories _category;
        public EnumCompleteCategories Category
        {
            get { return _category; }
            set
            {
                if (SetProperty(ref _category, value))
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
            if (MainGraphics!.IsUnknown == true)
                return true; //default to true.  change to what you need to start drawing.
            return Category != EnumCompleteCategories.None;
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
        public void DrawBacks(SKCanvas canvas, SKRect rect_Card) { }
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _safefyPaint;
        private SKPaint? _basicPen;
        private SKPaint? _whitePaint;
        private SKPaint? _redFill;
        private SKPaint? _blackPaint;
        private SKPaint? _grayPaint;
        private SKPaint? _greenPaint;
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
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(55, 72);
            _pDrewPaint = MainGraphics.GetStandardDrewPaint();
            _selectPaint = BaseDeckGraphicsCP.GetStandardSelectPaint();
            _safefyPaint = MiscHelpers.GetSolidPaint(SKColors.Blue); // this is blue for safeties
            _basicPen = MiscHelpers.GetStrokePaint(SKColors.Black, 1); // try this way.
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _redFill = MiscHelpers.GetSolidPaint(SKColors.Red);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _grayPaint = MiscHelpers.GetSolidPaint(SKColors.Gray);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            SKPath gp_Card;
            SKPaint tempPen;
            SKPoint[] pts;
            int int_Temp;
            SKMatrix tmp_Matrix;
            SKRegion reg_Temp;
            SKPoint pt_Center;
            pt_Center = new SKPoint(rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Width / 100));
            SKPaint textPaint;
            SKPaint textBorder;
            switch (Category)
            {
                case EnumCompleteCategories.Accident:
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32(((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 8))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 10))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 8))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 10))), true);
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 3) / 4))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 9) / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))));
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 9) / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 8))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 8))));
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 4))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))));
                    gp_Card.Close(); // i think
                    canvas.DrawPath(gp_Card, _basicPen);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create((rect_Card.Left + (rect_Card.Width / 4)) - (rect_Card.Width / 12), (rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 8)) - (rect_Card.Width / 12), rect_Card.Width / 6, rect_Card.Width / 6));
                    canvas.DrawPath(gp_Card, _whitePaint);
                    canvas.DrawPath(gp_Card, _basicPen);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create((rect_Card.Left + ((rect_Card.Width * 3) / 4)) - (rect_Card.Width / 12), (rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 8)) - (rect_Card.Width / 12), rect_Card.Width / 6, rect_Card.Width / 6));
                    canvas.DrawPath(gp_Card, _whitePaint);
                    canvas.DrawPath(gp_Card, _basicPen);
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Red, rect_Card.Width / 30);
                    canvas.DrawLine(System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 3) / 4))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 19) / 20))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 10))), tempPen);
                    canvas.DrawLine(System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 3) / 4))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 10))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 19) / 20))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), tempPen);
                    break;
                case EnumCompleteCategories.OutOfGas:
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Black, rect_Card.Height / 40);
                    int_Temp = (int)rect_Card.Width;
                    gp_Card = new SKPath();
                    gp_Card.AddArc(SKRect.Create(rect_Card.Left - (int_Temp / 2) + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 3), int_Temp, int_Temp), -135, 90);
                    canvas.DrawPath(gp_Card, tempPen);
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Red, rect_Card.Height / 40);
                    tempPen.StrokeCap = SKStrokeCap.Round; // strokejoin did not help one bit
                    canvas.DrawLine(rect_Card.Left + (rect_Card.Width / 2), rect_Card.Top + (rect_Card.Height / 3) + (int_Temp / 2), rect_Card.Width / 3.5f, rect_Card.Height / 2.16f, tempPen);
                    var ThisPaint = MiscHelpers.GetTextPaint(SKColors.Red, rect_Card.Height / 4, "Ariel");
                    ThisPaint.FakeBoldText = true;
                    canvas.DrawText("E", rect_Card.Left + (rect_Card.Width / 10), rect_Card.Top + (rect_Card.Height / 3), ThisPaint);
                    ThisPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height / 4, "Ariel");
                    ThisPaint.FakeBoldText = true;
                    canvas.DrawText("F", rect_Card.Left + ((rect_Card.Width * 15) / 20), rect_Card.Top + (rect_Card.Height / 3), ThisPaint);
                    break;
                case EnumCompleteCategories.FlatTire:
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), pt_Center.Y - ((rect_Card.Width * 4) / 12), (rect_Card.Width * 4) / 6, (rect_Card.Width * 4) / 6));
                    reg_Temp = MiscHelpers.GetNewRegion(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), (pt_Center.Y - ((rect_Card.Width * 4) / 12)) + ((((rect_Card.Width * 4) / 6) * 5) / 6), (rect_Card.Width * 4) / 6, (rect_Card.Width * 4) / 6));
                    var OtherRegion = new SKRegion(reg_Temp);
                    reg_Temp.SetPath(gp_Card);
                    canvas.DrawRegion(reg_Temp, _redFill);
                    reg_Temp.SetPath(gp_Card, OtherRegion);
                    canvas.DrawRegion(reg_Temp, _whitePaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + ((rect_Card.Width * 2) / 6), pt_Center.Y - ((rect_Card.Width * 2) / 12), (rect_Card.Width * 2) / 6, (rect_Card.Width * 2) / 6));
                    canvas.DrawPath(gp_Card, _whitePaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 3) + (rect_Card.Width / 12), pt_Center.Y - (rect_Card.Width / 12), rect_Card.Width / 6, rect_Card.Width / 6));
                    canvas.DrawPath(gp_Card, _blackPaint);
                    break;
                case EnumCompleteCategories.SpeedLimit:
                    textPaint = MiscHelpers.GetTextPaint(SKColors.Black, rect_Card.Height / 3);
                    textBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1); // for speed limit, 1 is fine
                    canvas.DrawBorderText("50", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, textBorder, rect_Card);
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Red, rect_Card.Width / 10);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 8), (rect_Card.Top + (rect_Card.Height / 2)) - ((rect_Card.Width * 3) / 8), (rect_Card.Width * 3) / 4, (rect_Card.Width * 3) / 4));
                    canvas.DrawPath(gp_Card, tempPen);
                    break;
                case EnumCompleteCategories.Stop:
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 6), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Width / 6), rect_Card.Width / 3, rect_Card.Width / 3));
                    canvas.DrawPath(gp_Card, _grayPaint);
                    tmp_Matrix = SKMatrix.MakeTranslation(0, (rect_Card.Width * 5) / 13);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _grayPaint);
                    tmp_Matrix = SKMatrix.MakeTranslation(0, (-rect_Card.Width * 10) / 13);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _redFill);
                    break;
                case EnumCompleteCategories.Repairs:
                    gp_Card = new SKPath();
                    reg_Temp = MiscHelpers.GetNewRegion(SKRect.Create(rect_Card.Left + (rect_Card.Width / 3), rect_Card.Top, rect_Card.Width / 3, rect_Card.Height / 3));
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), rect_Card.Top + (rect_Card.Height / 10), (rect_Card.Width * 4) / 6, (rect_Card.Height * 3) / 7));
                    gp_Card.AddRect(SKRect.Create(rect_Card.Left + (rect_Card.Width / 3), rect_Card.Top + (rect_Card.Height / 2), rect_Card.Width / 3, (rect_Card.Height * 4) / 9));
                    var OtherRegion2 = new SKRegion(reg_Temp);
                    reg_Temp.SetPath(gp_Card);
                    canvas.DrawRegion(reg_Temp, _greenPaint);
                    reg_Temp.SetPath(gp_Card, OtherRegion2);
                    canvas.DrawRegion(reg_Temp, _whitePaint);
                    gp_Card = new SKPath();
                    gp_Card.AddRect(SKRect.Create((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 12), rect_Card.Top + (rect_Card.Height / 8), rect_Card.Width / 6, rect_Card.Height / 7));
                    canvas.DrawPath(gp_Card, _blackPaint);
                    break;
                case EnumCompleteCategories.Gasoline:
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 4))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 4))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 4))), true);
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 4))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 3) / 4))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 3))));
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 3) / 4))), System.Convert.ToInt32((rect_Card.Top + ((rect_Card.Height * 3) / 4))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 4))), System.Convert.ToInt32((rect_Card.Top + ((rect_Card.Height * 3) / 4))));
                    gp_Card.Close();
                    canvas.DrawPath(gp_Card, _greenPaint);
                    gp_Card = new SKPath();
                    gp_Card.AddRect(SKRect.Create(rect_Card.Left, rect_Card.Top, rect_Card.Width / 8, rect_Card.Height / 20));
                    tmp_Matrix = SKMatrix.MakeRotationDegrees(23, rect_Card.Left, rect_Card.Top);
                    tmp_Matrix.TransX = (rect_Card.Width * 20) / 30;
                    tmp_Matrix.TransY = rect_Card.Height / 5.4f;
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _greenPaint);
                    break;
                case EnumCompleteCategories.Spare:
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), pt_Center.Y - ((rect_Card.Width * 4) / 12), (rect_Card.Width * 4) / 6, (rect_Card.Width * 4) / 6));
                    reg_Temp = MiscHelpers.GetNewRegion(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), (pt_Center.Y - ((rect_Card.Width * 4) / 12)) + ((((rect_Card.Width * 4) / 6) * 5) / 6), (rect_Card.Width * 4) / 6, (rect_Card.Width * 4) / 6));
                    reg_Temp.SetPath(gp_Card);
                    canvas.DrawRegion(reg_Temp, _greenPaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + ((rect_Card.Width * 2) / 6), pt_Center.Y - ((rect_Card.Width * 2) / 12), (rect_Card.Width * 2) / 6, (rect_Card.Width * 2) / 6));
                    canvas.DrawPath(gp_Card, _whitePaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 3) + (rect_Card.Width / 12), pt_Center.Y - (rect_Card.Width / 12), rect_Card.Width / 6, rect_Card.Width / 6));
                    canvas.DrawPath(gp_Card, _blackPaint);
                    break;
                case EnumCompleteCategories.EndOfLimit:
                    textPaint = MiscHelpers.GetTextPaint(SKColors.Gray, rect_Card.Height / 3);
                    textBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1); // for speed limit, 1 is fine
                    textPaint.FakeBoldText = true;
                    canvas.DrawBorderText("50", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, textBorder, rect_Card);
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Green, rect_Card.Width / 10);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 8), (rect_Card.Top + (rect_Card.Height / 2)) - ((rect_Card.Width * 3) / 8), (rect_Card.Width * 3) / 4, (rect_Card.Width * 3) / 4));
                    canvas.DrawPath(gp_Card, tempPen);
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - ((rect_Card.Width * 3) / 8))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + ((rect_Card.Width * 3) / 8))), true);
                    tmp_Matrix = SKMatrix.MakeRotationDegrees(45, pt_Center.X, pt_Center.Y);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, tempPen);
                    break;
                case EnumCompleteCategories.Roll:
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 6), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Width / 6), rect_Card.Width / 3, rect_Card.Width / 3));
                    canvas.DrawPath(gp_Card, _grayPaint);
                    tmp_Matrix = SKMatrix.MakeTranslation(0, (rect_Card.Width * 5) / 13);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _greenPaint);
                    tmp_Matrix = SKMatrix.MakeTranslation(0, (-rect_Card.Width * 10) / 13);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _grayPaint);
                    break;
                case EnumCompleteCategories.DrivingAce:
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 10), (rect_Card.Top + (rect_Card.Height / 2)) - ((rect_Card.Width * 4) / 10), (rect_Card.Width * 8) / 10, (rect_Card.Width * 8) / 10));
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Blue, rect_Card.Width / 20);
                    canvas.DrawPath(gp_Card, tempPen);
                    tmp_Matrix = SKMatrix.MakeRotationDegrees(120, pt_Center.X, pt_Center.Y);
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32(((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 25))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + ((rect_Card.Width * 4) / 10))), System.Convert.ToInt32(((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 15))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), true);
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 15))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 25))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2) + ((rect_Card.Width * 4) / 10))));
                    gp_Card.Close();
                    canvas.DrawPath(gp_Card, _safefyPaint);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _safefyPaint);
                    gp_Card.Transform(tmp_Matrix);
                    canvas.DrawPath(gp_Card, _safefyPaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 8), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Width / 8), rect_Card.Width / 4, rect_Card.Width / 4));
                    canvas.DrawPath(gp_Card, _whitePaint);
                    canvas.DrawPath(gp_Card, tempPen);
                    break;
                case EnumCompleteCategories.ExtraTank:
                    gp_Card = new SKPath();
                    pts = new SKPoint[3];
                    pts[0] = new SKPoint(rect_Card.Left + (rect_Card.Width / 4), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 8));
                    pts[1] = new SKPoint(rect_Card.Left + (rect_Card.Width / 6), rect_Card.Top + (rect_Card.Height / 2));
                    pts[2] = new SKPoint(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 8));
                    var top1 = pts[0];
                    gp_Card.MoveTo(pts[0]);
                    gp_Card.QuadTo(pts[1], pts[2]);
                    pts[0] = new SKPoint(rect_Card.Left + ((rect_Card.Width * 3) / 4), rect_Card.Top + (rect_Card.Height / 2) + (rect_Card.Height / 8));
                    pts[1] = new SKPoint(rect_Card.Left + ((rect_Card.Width * 5) / 6), rect_Card.Top + (rect_Card.Height / 2));
                    pts[2] = new SKPoint(rect_Card.Left + ((rect_Card.Width * 3) / 4), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 8));
                    var bottom2 = pts[0];
                    gp_Card.MoveTo(pts[0]);
                    gp_Card.QuadTo(pts[1], pts[2]);
                    var tempRect = new SKRect(top1.X, top1.Y, bottom2.X, bottom2.Y);
                    gp_Card.Close();
                    canvas.DrawPath(gp_Card, _safefyPaint);
                    gp_Card = new SKPath();
                    gp_Card.AddRect(SKRect.Create(rect_Card.Left + ((rect_Card.Width * 2) / 5), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 8) - (rect_Card.Height / 20), rect_Card.Width / 5, rect_Card.Height / 30));
                    canvas.DrawPath(gp_Card, _safefyPaint);
                    canvas.DrawRect(tempRect, _safefyPaint);
                    break;
                case EnumCompleteCategories.PunctureProof:
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), pt_Center.Y - ((rect_Card.Width * 4) / 12), (rect_Card.Width * 4) / 6, (rect_Card.Width * 4) / 6));
                    reg_Temp = MiscHelpers.GetNewRegion(SKRect.Create(rect_Card.Left + (rect_Card.Width / 6), (pt_Center.Y - ((rect_Card.Width * 4) / 12)) + ((((rect_Card.Width * 4) / 6) * 5) / 6), (rect_Card.Width * 4) / 6, (rect_Card.Width * 4) / 6));
                    reg_Temp.SetPath(gp_Card);
                    canvas.DrawRegion(reg_Temp, _safefyPaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + ((rect_Card.Width * 2) / 6), pt_Center.Y - ((rect_Card.Width * 2) / 12), (rect_Card.Width * 2) / 6, (rect_Card.Width * 2) / 6));
                    canvas.DrawPath(gp_Card, _whitePaint);
                    gp_Card = new SKPath();
                    gp_Card.AddOval(SKRect.Create(rect_Card.Left + (rect_Card.Width / 3) + (rect_Card.Width / 12), pt_Center.Y - (rect_Card.Width / 12), rect_Card.Width / 6, rect_Card.Width / 6));
                    canvas.DrawPath(gp_Card, _blackPaint);
                    tmp_Matrix = SKMatrix.MakeTranslation(rect_Card.Width / 12, 0);
                    gp_Card = new SKPath();
                    gp_Card.AddLine(rect_Card.Left, rect_Card.Top + (rect_Card.Height * 4 / 5) + (rect_Card.Width / 24), rect_Card.Left + (rect_Card.Width / 24), rect_Card.Top + (rect_Card.Height * 4 / 5 - (rect_Card.Width / 24)), true);
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 24))), System.Convert.ToInt32(((rect_Card.Top + ((rect_Card.Height * 4) / 5)) - (rect_Card.Width / 24))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 12))), System.Convert.ToInt32((rect_Card.Top + ((rect_Card.Height * 4) / 5) + (rect_Card.Width / 24))));
                    for (var int_Count = 1; int_Count <= 10; int_Count++)
                    {
                        gp_Card.Transform(tmp_Matrix);
                        canvas.DrawPath(gp_Card, _safefyPaint);
                    }
                    break;
                case EnumCompleteCategories.RightOfWay:
                    tempPen = MiscHelpers.GetStrokePaint(SKColors.Blue, rect_Card.Width / 50);
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 5))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 9) / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), true);
                    canvas.DrawPath(gp_Card, tempPen);
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 10))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 9) / 10))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 10))), true);
                    canvas.DrawPath(gp_Card, tempPen);
                    gp_Card = new SKPath();
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 9) / 10))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 5))), true);
                    canvas.DrawPath(gp_Card, tempPen);
                    gp_Card = new SKPath();
                    canvas.DrawPath(gp_Card, _whitePaint);
                    gp_Card = new SKPath();
                    pts = new SKPoint[3];
                    pts[0] = new SKPoint((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 6), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 5));
                    pts[1] = new SKPoint(rect_Card.Left + (rect_Card.Width / 2), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 4));
                    pts[2] = new SKPoint(rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 6), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 5));
                    gp_Card.MoveTo(pts[0]);
                    gp_Card.QuadTo(pts[1], pts[2]);
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 6))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))), System.Convert.ToInt32(((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 6))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 2))));
                    gp_Card.Close();
                    canvas.DrawPath(gp_Card, _safefyPaint);
                    gp_Card = new SKPath();
                    pts = new SKPoint[3];
                    pts[0] = new SKPoint((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 12), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 8));
                    pts[1] = new SKPoint(rect_Card.Left + (rect_Card.Width / 2), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 6));
                    pts[2] = new SKPoint(rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 12), (rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 8));
                    gp_Card.MoveTo(pts[0]);
                    gp_Card.QuadTo(pts[1], pts[2]);
                    gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 2) + (rect_Card.Width / 12))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 20))), System.Convert.ToInt32(((rect_Card.Left + (rect_Card.Width / 2)) - (rect_Card.Width / 12))), System.Convert.ToInt32(((rect_Card.Top + (rect_Card.Height / 2)) - (rect_Card.Height / 20))));
                    gp_Card.Close();
                    canvas.DrawPath(gp_Card, _whitePaint);
                    canvas.DrawRect(SKRect.Create(rect_Card.Left + (rect_Card.Width / 4), rect_Card.Top + (rect_Card.Height / 2), rect_Card.Width / 2, rect_Card.Height / 10), _blackPaint);
                    break;
                case EnumCompleteCategories.Distance25:
                    DrawDistanceCard(canvas, "25", rect_Card);
                    break;
                case EnumCompleteCategories.Distance50:
                    DrawDistanceCard(canvas, "50", rect_Card);
                    break;
                case EnumCompleteCategories.Distance75:
                    DrawDistanceCard(canvas, "75", rect_Card);
                    break;
                case EnumCompleteCategories.Distance100:
                    DrawDistanceCard(canvas, "100", rect_Card);
                    break;
                case EnumCompleteCategories.Distance200:
                    DrawDistanceCard(canvas, "200", rect_Card);
                    break;
                default:
                    throw new BasicBlankException("Nothing Found");
            }
        }
        private void DrawDistanceCard(SKCanvas canvas, string str_Text, SKRect rect_Card)
        {
            SKPath gp_Card;
            SKPaint pn_Card;
            SKPaint textBorder;
            SKColor clr_Text = SKColors.Black;
            SKColor clr_Border = SKColors.Red;
            SKPaint textPaint;
            switch (str_Text)
            {
                case "25":
                    {
                        clr_Text = SKColors.Red;
                        clr_Border = SKColors.Green;
                        break;
                    }

                case "50":
                    {
                        clr_Text = SKColors.Red;
                        clr_Border = SKColors.Blue;
                        break;
                    }

                case "75":
                    {
                        clr_Text = SKColors.Green;
                        clr_Border = SKColors.Red;
                        break;
                    }

                case "100":
                    {
                        clr_Text = SKColors.Green;
                        clr_Border = SKColors.Blue;
                        break;
                    }

                case "200":
                    {
                        clr_Text = SKColors.Blue;
                        clr_Border = SKColors.Green;
                        break;
                    }
            }
            pn_Card = MiscHelpers.GetStrokePaint(clr_Border, rect_Card.Width / 20);
            gp_Card = new SKPath();
            gp_Card.AddArc(SKRect.Create(System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32((rect_Card.Top + (rect_Card.Height / 10))), System.Convert.ToInt32(((rect_Card.Width * 4) / 5)), System.Convert.ToInt32(((rect_Card.Height * 3) / 7))), 180, 180);
            gp_Card.AddLine(System.Convert.ToInt32((rect_Card.Left + ((rect_Card.Width * 9) / 10))), System.Convert.ToInt32((rect_Card.Top + ((rect_Card.Height * 9) / 10))), System.Convert.ToInt32((rect_Card.Left + (rect_Card.Width / 10))), System.Convert.ToInt32((rect_Card.Top + ((rect_Card.Height * 9) / 10))));
            gp_Card.Close();
            canvas.DrawPath(gp_Card, pn_Card);
            textPaint = MiscHelpers.GetTextPaint(clr_Text, rect_Card.Height / 3);
            textBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1); // for speed limit, 1 is fine
            textPaint.FakeBoldText = true;
            canvas.DrawBorderText(str_Text, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, textBorder, rect_Card);
        }
    }
}
