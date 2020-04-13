using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using ClueBoardGameCP.Data;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace ClueBoardGameCP.Graphics
{
    public class WeaponCP : BaseGraphicsCP
    {
        private EnumWeaponList _weapon;
        public EnumWeaponList Weapon
        {
            get
            {
                return _weapon;
            }

            set
            {
                if (SetProperty(ref _weapon, value) == true)
                {
                }
            }
        }
        private readonly SKPaint _borderPaint;
        private readonly SKPaint _mainHatchPaint;
        private readonly SKPaint _cardHatchPaint;
        private readonly Assembly _thisA;
        private SKPaint GetPaint(string path)
        {
            var thisPaint = MiscHelpers.GetBitmapPaint();
            thisPaint.Shader = ImageExtensions.GetSkShader(_thisA, path);
            return thisPaint;
        }
        public WeaponCP()
        {
            _thisA = Assembly.GetAssembly(GetType());
            _borderPaint = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
            _mainHatchPaint = GetPaint("blackaqualightupwards.png");
            _cardHatchPaint = GetPaint("blacksilverlightupwards.png");
        }
        public void DrawWeapon(SKCanvas dc, SKRect rect_Piece)
        {
            if (Weapon == EnumWeaponList.None)
                throw new BasicBlankException("You must specify a weapon to draw");
#pragma warning disable IDE0068 // Use recommended dispose pattern
            SKPath gp_Piece = new SKPath();
#pragma warning restore IDE0068 // Use recommended dispose pattern  cannot do recommended because it creates some other objects.
            SKPoint[] pts_Curve;
            int[] int_Offset;
            SKMatrix tmp_Matrix;
            tmp_Matrix = SKMatrix.MakeScale(rect_Piece.Width / 100, rect_Piece.Height / 100);
            tmp_Matrix.TransX = rect_Piece.Left;
            tmp_Matrix.TransY = rect_Piece.Top;
            switch (Weapon)
            {
                case EnumWeaponList.Candlestick:
                    {
                        int_Offset = new int[5];
                        int_Offset[0] = 35;
                        int_Offset[1] = 30;
                        int_Offset[2] = 40;
                        int_Offset[3] = 30;
                        int_Offset[4] = 50;
                        pts_Curve = new SKPoint[10]; // was 4.  has to do poly since curves don't work
                        pts_Curve[0] = new SKPoint((float)50 + int_Offset[0], 0);
                        pts_Curve[1] = new SKPoint((float)50 + int_Offset[1], 20);
                        pts_Curve[2] = new SKPoint((float)50 + int_Offset[2], 50);
                        pts_Curve[3] = new SKPoint((float)50 + int_Offset[3], 80);
                        pts_Curve[4] = new SKPoint((float)50 + int_Offset[4], 90);
                        pts_Curve[9] = new SKPoint((float)50 - int_Offset[0], 0);
                        pts_Curve[8] = new SKPoint((float)50 - int_Offset[1], 20);
                        pts_Curve[7] = new SKPoint((float)50 - int_Offset[2], 50);
                        pts_Curve[6] = new SKPoint((float)50 - int_Offset[3], 80);
                        pts_Curve[5] = new SKPoint((float)50 - int_Offset[4], 90);
                        gp_Piece.AddPoly(pts_Curve);
                        gp_Piece.AddRect(SKRect.Create(0, 90, 100, 10));
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        break;
                    }

                case EnumWeaponList.Knife:
                    {
                        int_Offset = new int[5];
                        int_Offset[0] = 0;
                        int_Offset[1] = 15;
                        int_Offset[2] = 20;
                        int_Offset[3] = 18;
                        int_Offset[4] = 15;
                        pts_Curve = new SKPoint[12];
                        pts_Curve[0] = new SKPoint((float)50 + int_Offset[0], 0);
                        pts_Curve[1] = new SKPoint((float)50 + int_Offset[1], 5);
                        pts_Curve[2] = new SKPoint((float)50 + int_Offset[2], 15);
                        pts_Curve[3] = new SKPoint((float)50 + int_Offset[3], 25);
                        pts_Curve[4] = new SKPoint((float)50 + int_Offset[4], 50);
                        pts_Curve[5] = new SKPoint(50, 100);
                        pts_Curve[11] = new SKPoint((float)50 - int_Offset[0], 0);
                        pts_Curve[10] = new SKPoint((float)50 - int_Offset[1], 5);
                        pts_Curve[9] = new SKPoint((float)50 - int_Offset[2], 15);
                        pts_Curve[8] = new SKPoint((float)50 - int_Offset[3], 25);
                        pts_Curve[7] = new SKPoint((float)50 - int_Offset[4], 50);
                        pts_Curve[6] = new SKPoint(50, 100);
                        gp_Piece.AddPoly(pts_Curve);
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        gp_Piece = new SKPath();
                        gp_Piece.AddRect(SKRect.Create(0, 25, 100, 10));
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        break;
                    }
                case EnumWeaponList.LeadPipe:
                    {
                        gp_Piece = new SKPath();
                        gp_Piece.AddRect(SKRect.Create(0, 0, 100, 100));
                        gp_Piece.Transform(tmp_Matrix);
                        if (MainColor.Equals(cs.Aqua) == true)
                            dc.DrawPath(gp_Piece, _mainHatchPaint);
                        else
                            dc.DrawPath(gp_Piece, _cardHatchPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        gp_Piece = new SKPath();
                        gp_Piece.AddRect(SKRect.Create(0, 20, 100, 60));
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        break;
                    }
                case EnumWeaponList.Revolver:
                    {
                        pts_Curve = new SKPoint[3];
                        pts_Curve[0] = new SKPoint(20, 65);
                        pts_Curve[1] = new SKPoint(55, 60);
                        pts_Curve[2] = new SKPoint(55, 30);
                        gp_Piece.AddPoly(pts_Curve); // try this way
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        gp_Piece = new SKPath();
                        pts_Curve = new SKPoint[8];
                        pts_Curve[0] = new SKPoint(100, 5);
                        pts_Curve[1] = new SKPoint(30, 5);
                        pts_Curve[2] = new SKPoint(20, 20);
                        pts_Curve[3] = new SKPoint(0, 100);
                        pts_Curve[4] = new SKPoint(30, 100);
                        pts_Curve[5] = new SKPoint(35, 80);
                        pts_Curve[6] = new SKPoint(45, 45);
                        pts_Curve[7] = new SKPoint(100, 40);
                        gp_Piece.AddPoly(pts_Curve);
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        break;
                    }
                case EnumWeaponList.Rope:
                    {
                        int int_Angle = 0;
                        int int_Count;
                        var rect_Rope = SKRect.Create(45, 45, 10, 10);
                        for (int_Count = 1; int_Count <= 8; int_Count++)
                        {
                            gp_Piece.AddArc(rect_Rope, int_Angle, 180);
                            if (int_Angle == 0)
                            {
                                rect_Rope = SKRect.Create(rect_Rope.Left, rect_Rope.Top - 5, rect_Rope.Width + 10, rect_Rope.Height + 10);
                                int_Angle = 180;
                            }
                            else
                            {
                                rect_Rope = SKRect.Create(rect_Rope.Left - 10, rect_Rope.Top - 5, rect_Rope.Width + 10, rect_Rope.Height + 10);
                                int_Angle = 0;
                            }
                        }
                        gp_Piece.AddLine(100, 65, 95, 70);
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        break;
                    }
                case EnumWeaponList.Wrench:
                    {
                        gp_Piece.AddLine(0, 10, 10, 0, true);
                        gp_Piece.AddLine(90, 0, 100, 10);
                        gp_Piece.AddLine(100, 100, 60, 100);
                        gp_Piece.AddLine(60, 40, 10, 40);
                        gp_Piece.AddLine(0, 30, 40, 30);
                        gp_Piece.AddLine(40, 10, 0, 10);
                        gp_Piece.Close();
                        gp_Piece.Transform(tmp_Matrix);
                        dc.DrawPath(gp_Piece, MainPaint);
                        dc.DrawPath(gp_Piece, _borderPaint);
                        break;
                    }
            }
        }
        public override void DrawImage(SKCanvas dc)
        {
            if (Weapon == EnumWeaponList.None)
                return;
            MainColor = cs.Aqua;
            var rect_Piece = GetMainRect();
            DrawWeapon(dc, rect_Piece);
        }
    }
}
