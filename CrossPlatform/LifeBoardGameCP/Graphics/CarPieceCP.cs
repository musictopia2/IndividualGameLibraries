using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.CommonInterfaces;
using LifeBoardGameCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
//i think this is the most common things i like to do
namespace LifeBoardGameCP.Graphics
{
    public class CarPieceCP : BaseGraphicsCP, IEnumPiece<EnumColorChoice>
    {
        private readonly SKPaint _blackPaint;
        private readonly SKPaint _borderPaint;

        public CarPieceCP()
        {
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        public EnumTypesOfCars CarType { get; set; } = EnumTypesOfCars.Minivan;
        private string _tempValue = "";
        EnumColorChoice IEnumPiece<EnumColorChoice>.EnumValue
        {
            get
            {
                return (EnumColorChoice)Enum.Parse(typeof(EnumColorChoice), _tempValue);
            }
            set
            {
                _tempValue = value.ToString();
                MainColor = value.ToColor();
            }
        }
        private CustomBasicList<string> _arr_Pegs = new CustomBasicList<string>(); // needs to be gcolor because of autoresume
        public void AddPeg(string pegColor)
        {
            if (_arr_Pegs.Count < 6)
                _arr_Pegs.Add(pegColor);
        }
        public void ClearPegs()
        {
            _arr_Pegs = new CustomBasicList<string>();
        }
        public CustomBasicList<string> GetPegs()
        {
            return _arr_Pegs;
        }
        public void DrawPeg(SKCanvas Canvas, SKRect pegShape, SKColor color)
        {
            var thisPaint = MiscHelpers.GetSolidPaint(color);
            Canvas.DrawOval(pegShape, thisPaint);
        }
        public override void DrawImage(SKCanvas dc)
        {
            var rect_Car = GetMainRect();
            SKPoint pt_Center = new SKPoint(rect_Car.Left + (rect_Car.Width / 2), rect_Car.Top + (rect_Car.Height / 2));
            SKRect rect_Body;
            SKPath gp_Body;
            SKRect rect_Tire;
            float int_TireHeight;
            SKPoint[] pts_Body;
            SKRect rect_Hole;
            int int_Row;
            int int_Col;
            float int_HoleWidth;
            int int_Count;
            CustomBasicList<SKRect> arr_Holes;
            SKColor clr_Peg;
            SKMatrix tmp_Matrix = new SKMatrix();
            if (CarType == EnumTypesOfCars.Minivan)
            {
                // *** Draw tires
                int_TireHeight = (rect_Car.Height / 5);
                rect_Tire = SKRect.Create(rect_Car.Left, rect_Car.Top + (rect_Car.Height / 10), rect_Car.Width, int_TireHeight);
                dc.DrawRect(rect_Tire, _blackPaint);
                rect_Tire = SKRect.Create(rect_Car.Left, (rect_Car.Top + rect_Car.Height) - (rect_Car.Height / 10) - int_TireHeight, rect_Car.Width, int_TireHeight);
                dc.DrawRect(rect_Tire, _blackPaint);
                // *** Draw the body outline
                gp_Body = new SKPath();
                rect_Body = SKRect.Create(rect_Car.Left + (rect_Car.Width / 12), rect_Car.Top, rect_Car.Width - (rect_Car.Width / 6), rect_Car.Height);
                pts_Body = new SKPoint[4];
                pts_Body[0] = new SKPoint(System.Convert.ToInt32(rect_Body.Left), rect_Body.Top + (rect_Body.Height / 30));
                pts_Body[1] = new SKPoint(System.Convert.ToInt32((rect_Body.Left + rect_Body.Width)), rect_Body.Top + (rect_Body.Height / 30));
                pts_Body[2] = new SKPoint(System.Convert.ToInt32(((rect_Body.Left + rect_Body.Width) - (rect_Car.Width / 15))), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 30));
                pts_Body[3] = new SKPoint(System.Convert.ToInt32((rect_Body.Left + (rect_Car.Width / 15))), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 30));
                gp_Body.AddPoly(pts_Body);
                SKMatrix.RotateDegrees(ref tmp_Matrix, 180, pt_Center.X, pt_Center.Y);
                gp_Body.Transform(tmp_Matrix);
                dc.DrawPath(gp_Body, MainPaint);
                dc.DrawPath(gp_Body, _borderPaint);
                gp_Body = new SKPath();
                pts_Body = new SKPoint[3];
                pts_Body[0] = new SKPoint(rect_Body.Left + (rect_Body.Width / 15), rect_Body.Top + (rect_Body.Height / 15));
                pts_Body[1] = new SKPoint(rect_Body.Left + (rect_Body.Width / 2), rect_Body.Top + (rect_Body.Height / 100));
                pts_Body[2] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 15), rect_Body.Top + (rect_Body.Height / 15));
                gp_Body.AddPoly(pts_Body);
                SKMatrix.RotateDegrees(ref tmp_Matrix, 180, pt_Center.X, pt_Center.Y);
                gp_Body.Transform(tmp_Matrix);
                dc.DrawPath(gp_Body, _blackPaint);
                // *** Draw the front window
                gp_Body = new SKPath();
                pts_Body = new SKPoint[8];
                pts_Body[0] = new SKPoint(rect_Body.Left + (rect_Body.Width / 12), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3));
                pts_Body[1] = new SKPoint(rect_Body.Left + (rect_Body.Width / 2), ((rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3)) + (rect_Body.Height / 30));
                pts_Body[2] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 12), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3));
                pts_Body[3] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 12), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3));
                pts_Body[4] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 6), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 4));
                pts_Body[5] = new SKPoint(rect_Body.Left + (rect_Body.Width / 2), ((rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 4)) + (rect_Body.Height / 30));
                pts_Body[6] = new SKPoint(rect_Body.Left + (rect_Body.Width / 6), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 4));
                pts_Body[7] = new SKPoint(rect_Body.Left + (rect_Body.Width / 12), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3));
                gp_Body.AddPoly(pts_Body);
                SKMatrix.RotateDegrees(ref tmp_Matrix, 180, pt_Center.X, pt_Center.Y);
                gp_Body.Transform(tmp_Matrix);
                dc.DrawPath(gp_Body, _blackPaint);
                // *** Draw the hood
                gp_Body = new SKPath();
                pts_Body = new SKPoint[8];
                pts_Body[0] = new SKPoint(rect_Body.Left + (rect_Body.Width / 8), ((rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3)) + (rect_Body.Height / 10));
                pts_Body[1] = new SKPoint(rect_Body.Left + (rect_Body.Width / 2), ((rect_Body.Top + rect_Body.Height) - ((rect_Body.Height * 2) / 6)) + (rect_Body.Height / 30) + (rect_Body.Height / 10));
                pts_Body[2] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 8), ((rect_Body.Top + rect_Body.Height) - ((rect_Body.Height * 2) / 6)) + (rect_Body.Height / 10));
                pts_Body[3] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 8), ((rect_Body.Top + rect_Body.Height) - ((rect_Body.Height * 2) / 6)) + (rect_Body.Height / 10));
                pts_Body[4] = new SKPoint((rect_Body.Left + rect_Body.Width) - (rect_Body.Width / 5), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 30));
                pts_Body[5] = new SKPoint(rect_Body.Left + (rect_Body.Width / 2), rect_Body.Top + rect_Body.Height);
                pts_Body[6] = new SKPoint(rect_Body.Left + (rect_Body.Width / 5), (rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 30));
                pts_Body[7] = new SKPoint(rect_Body.Left + (rect_Body.Width / 8), ((rect_Body.Top + rect_Body.Height) - (rect_Body.Height / 3)) + (rect_Body.Height / 10));
                gp_Body.AddPoly(pts_Body);
                SKMatrix.RotateDegrees(ref tmp_Matrix, 180, pt_Center.X, pt_Center.Y);
                gp_Body.Transform(tmp_Matrix);
                dc.DrawPath(gp_Body, MainPaint);
            }
            else if (CarType == EnumTypesOfCars.Car)
            {
            }
            // *** Draw the holes
            arr_Holes = new CustomBasicList<SKRect>();
            int_HoleWidth = rect_Car.Width * 2 / 9;
            for (int_Row = 0; int_Row <= 2; int_Row++)
            {
                for (int_Col = 0; int_Col <= 1; int_Col++)
                {
                    if (int_Col == 0)
                        rect_Hole = SKRect.Create(rect_Car.Left + (rect_Car.Width / 5), rect_Car.Top + (rect_Car.Height * 2 / 5) + (int_Row * (int_HoleWidth + (int_HoleWidth / 4))), int_HoleWidth, int_HoleWidth);
                    else
                        rect_Hole = SKRect.Create(rect_Car.Left + rect_Car.Width - (rect_Car.Width / 5) - int_HoleWidth, rect_Car.Top + (rect_Car.Height * 2 / 5) + (int_Row * (int_HoleWidth + (int_HoleWidth / 4))), int_HoleWidth, int_HoleWidth);

                    arr_Holes.Add(rect_Hole);
                }
            }
            var loopTo = arr_Holes.Count;
            for (int_Count = 1; int_Count <= loopTo; int_Count++)
            {
                rect_Hole = arr_Holes[int_Count - 1];
                dc.DrawOval(rect_Hole, _blackPaint);

                if (_arr_Pegs.Count >= int_Count)
                {
                    clr_Peg = _arr_Pegs[int_Count - 1].ToSKColor();
                    DrawPeg(dc, rect_Hole, clr_Peg);
                }
            }
        }
    }
}
