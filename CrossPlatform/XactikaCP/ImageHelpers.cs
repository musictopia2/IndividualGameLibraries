using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
namespace XactikaCP
{   //since this is shared, then no unit testing.  however, ui can't be unit tested anyways.
    static class ImageHelpers
    {
        public static CustomBasicList<SKPoint> GetPoints(EnumShapes shape, int howMany, SKPoint thisPoint, bool manually, float heightWidth)
        {
            float newLeft = 0;
            float margins;
            float mults;
            mults = heightWidth / 16;
            margins = 3 * mults; // for proportions
            if (manually == true)
            {
                if (howMany == 3)
                    return new CustomBasicList<SKPoint>() { new SKPoint(thisPoint.X + margins, thisPoint.Y + margins), new SKPoint(thisPoint.X + margins, thisPoint.Y + margins + heightWidth), new SKPoint(thisPoint.X + margins, thisPoint.Y + margins + heightWidth * 2) };
                else if (howMany == 1)
                    return new CustomBasicList<SKPoint>() { new SKPoint(thisPoint.X + margins, thisPoint.Y + margins + heightWidth) };
                else
                    return new CustomBasicList<SKPoint>() { new SKPoint(thisPoint.X + margins, thisPoint.Y + margins + (heightWidth / 2)), new SKPoint(thisPoint.X + margins, thisPoint.Y + margins + (heightWidth / 2) + heightWidth) };
            }
            float top1;
            float top2; // if only one, will be this one.
            float top3;
            float topFirstHalf;
            float topLastHalf;
            top1 = thisPoint.Y + margins; // try this way instead.
            top2 = top1 + heightWidth + margins;
            top3 = top2 + heightWidth + margins; // try this way (?)
            topFirstHalf = thisPoint.Y + margins + heightWidth / 2;
            topLastHalf = topFirstHalf + heightWidth + margins;
            switch (shape)
            {
                case EnumShapes.Balls:
                    {
                        newLeft = thisPoint.X + margins;
                        break;
                    }

                case EnumShapes.Cubes:
                    {
                        newLeft = thisPoint.X + heightWidth + margins * 2;
                        break;
                    }

                case EnumShapes.Cones:
                    {
                        newLeft = thisPoint.X + margins + heightWidth * 2 + margins * 2;
                        break;
                    }

                case EnumShapes.Stars:
                    {
                        newLeft = thisPoint.X + margins + heightWidth * 3 + margins * 3;
                        break;
                    }
            }

            if (howMany == 3)
                return new CustomBasicList<SKPoint>() { new SKPoint(newLeft, top1), new SKPoint(newLeft, top2), new SKPoint(newLeft, top3) };
            else if (howMany == 1)
                return new CustomBasicList<SKPoint>() { new SKPoint(newLeft, top2) };
            else
                return new CustomBasicList<SKPoint>() { new SKPoint(newLeft, topFirstHalf), new SKPoint(newLeft, topLastHalf) };
            throw new BasicBlankException("Cannot get points");
        }
        private static SKPaint? _penUsed;
        private static SKPaint? _darkOrangePaint;
        private static SKPaint? _bluePaint;
        private static SKPaint? _firstCube;
        private static SKPaint? _secondCube;
        private static SKPaint? _thirdCube;
        private static void CreatePaints()
        {
            if (_penUsed == null)
            {
                _penUsed = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
                _darkOrangePaint = MiscHelpers.GetSolidPaint(SKColors.DarkOrange);
                _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
                _firstCube = MiscHelpers.ColorFromArgb(0, 255, 255, 255);
                _secondCube = MiscHelpers.ColorFromArgb(100, 255, 255, 255);
                _thirdCube = MiscHelpers.ColorFromArgb(150, 0, 0, 0);
            }
        }
        public static void DrawCone(SKCanvas thisCanvas, SKRect thisRect) // done
        {
            CreatePaints();
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y));
            thisPath.LineTo(new SKPoint(thisRect.Location.X + ((thisRect.Width * 3) / 4), thisRect.Location.Y + ((thisRect.Height * 3) / 4)));
            thisPath.AddArc(new SKPoint(thisRect.Location.X + (thisRect.Width / 4), thisRect.Location.Y + ((thisRect.Height * 3) / 4)), new SKSize(thisRect.Width / 2, thisRect.Height), 180, false, SKPathDirection.Clockwise);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, _darkOrangePaint);
            thisCanvas.DrawPath(thisPath, _penUsed);
        }
        public static void DrawCube(SKCanvas thisCanvas, SKRect thisRect)
        {
            CreatePaints();
            SKPath thisPath = new SKPath();
            var pt_Center = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y + (thisRect.Height / 2));
            SKPoint[] pts_Side = new SKPoint[4];
            SKPoint[] pts = new SKPoint[6];
            pts[0] = new SKPoint(thisRect.Location.X, thisRect.Location.Y + (thisRect.Height / 4));
            pts[1] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y);
            pts[2] = new SKPoint(thisRect.Location.X + thisRect.Width, thisRect.Location.Y + (thisRect.Height / 4));
            pts[3] = new SKPoint(thisRect.Location.X + thisRect.Width, thisRect.Location.Y + ((thisRect.Height * 3) / 4));
            pts[4] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y + thisRect.Height);
            pts[5] = new SKPoint(thisRect.Location.X, thisRect.Location.Y + ((thisRect.Height * 3) / 4));
            thisPath.MoveTo(pts[0]);
            var loopTo = pts.Count() - 1;
            for (int i = 1; i <= loopTo; i++)
                thisPath.LineTo(pts[i]);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, _bluePaint);
            thisCanvas.DrawPath(thisPath, _penUsed);
            thisPath = new SKPath(); // shades
            pts_Side[0] = new SKPoint(thisRect.Location.X, thisRect.Location.Y + (thisRect.Height / 4));
            pts_Side[1] = pt_Center;
            pts_Side[2] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y + thisRect.Height);
            pts_Side[3] = new SKPoint(thisRect.Location.X, thisRect.Location.Y + ((thisRect.Height * 3) / 4));
            thisPath.MoveTo(pts_Side[0]);
            var loopTo1 = pts_Side.Count() - 1;
            for (int i = 1; i <= loopTo1; i++)
                thisPath.LineTo(pts_Side[i]);
            thisCanvas.DrawPath(thisPath, _firstCube);
            thisCanvas.DrawPath(thisPath, _penUsed);
            thisPath = new SKPath();
            pts_Side[0] = new SKPoint(thisRect.Location.X, thisRect.Location.Y + (thisRect.Height / 4));
            pts_Side[1] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y);
            pts_Side[2] = new SKPoint(thisRect.Location.X + thisRect.Width, thisRect.Location.Y + (thisRect.Height / 4));
            pts_Side[3] = pt_Center;
            thisPath.MoveTo(pts_Side[0]);
            var loopTo2 = pts_Side.Count() - 1;
            for (int i = 1; i <= loopTo2; i++)
                thisPath.LineTo(pts_Side[i]);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, _secondCube);
            thisCanvas.DrawPath(thisPath, _penUsed);
            thisPath = new SKPath();
            pts_Side[0] = new SKPoint(thisRect.Location.X + thisRect.Width, thisRect.Location.Y + (thisRect.Height / 4));
            pts_Side[1] = new SKPoint(thisRect.Location.X + thisRect.Width, thisRect.Location.Y + ((thisRect.Height * 3) / 4));
            pts_Side[2] = new SKPoint(thisRect.Location.X + (thisRect.Width / 2), thisRect.Location.Y + thisRect.Height);
            pts_Side[3] = pt_Center;

            thisPath.MoveTo(pts_Side[0]);
            var loopTo3 = pts_Side.Count() - 1;
            for (int i = 1; i <= loopTo3; i++)
                thisPath.LineTo(pts_Side[i]);
            thisPath.Close();
            thisCanvas.DrawPath(thisPath, _thirdCube);
            thisCanvas.DrawPath(thisPath, _penUsed);
        }
    }
}