using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using LifeCardGameCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Reflection;
namespace LifeCardGameCP.Cards
{
    public class LifeCardGameGraphicsCP : ObservableObject, IDeckGraphicsCP
    {
        public BaseDeckGraphicsCP? MainGraphics { get; set; }
        private int _points;
        public int Points
        {
            get
            {
                return _points;
            }

            set
            {
                if (SetProperty(ref _points, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private EnumSpecialCardCategory _requirement = EnumSpecialCardCategory.Unknown;
        public EnumSpecialCardCategory Requirement
        {
            get
            {
                return _requirement;
            }

            set
            {
                if (SetProperty(ref _requirement, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private EnumAction _action = EnumAction.Unknown;
        public EnumAction Action
        {
            get
            {
                return _action;
            }

            set
            {
                if (SetProperty(ref _action, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private EnumSpecialCardCategory _specialCategory = EnumSpecialCardCategory.Unknown;
        public EnumSpecialCardCategory SpecialCategory
        {
            get
            {
                return _specialCategory;
            }

            set
            {
                if (SetProperty(ref _specialCategory, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (SetProperty(ref _description, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private EnumFirstCardCategory _firstCategory = EnumFirstCardCategory.None;
        public EnumFirstCardCategory FirstCategory
        {
            get
            {
                return _firstCategory;
            }

            set
            {
                if (SetProperty(ref _firstCategory, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }


        private bool _opponentKeepsCard = false;
        public bool OpponentKeepsCard
        {
            get
            {
                return _opponentKeepsCard;
            }

            set
            {
                if (SetProperty(ref _opponentKeepsCard, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }

        private EnumSwitchCategory _switchCategory = EnumSwitchCategory.Unknown;
        public EnumSwitchCategory SwitchCategory
        {
            get
            {
                return _switchCategory;
            }

            set
            {
                if (SetProperty(ref _switchCategory, value) == true)
                    MainGraphics?.PaintUI?.DoInvalidate();
            }
        }


        private bool _drew;

        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                    MainGraphics?.PaintUI?.DoInvalidate();

            }
        }
        public bool NeedsToDrawBacks => true; //if you don't need to draw backs, then set false.
        public bool CanStartDrawing()
        {
            if (MainGraphics!.IsUnknown)
                return true;
            return FirstCategory != EnumFirstCardCategory.None;
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
            firstRect = MainGraphics!.GetActualRectangle(8, 8, 21, 50);
            secondRect = MainGraphics.GetActualRectangle(29, 8, 17, 50);
            thirdRect = MainGraphics.GetActualRectangle(46, 8, 23, 50);
            fourthRect = MainGraphics.GetActualRectangle(69, 8, 23, 50);
            SKRect bottomRect;
            bottomRect = MainGraphics.GetActualRectangle(8, 65, 84, 42);
            canvas.DrawRect(rect_Card, _whitePaint);
            var fontSize = firstRect.Height * 0.6f;
            canvas.DrawRect(firstRect, _purplePaint);
            canvas.DrawRect(firstRect, _blackBorder1);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            canvas.DrawBorderText("L", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder1!, firstRect);
            canvas.DrawRect(secondRect, _bluePaint);
            canvas.DrawRect(secondRect, _blackBorder1);
            canvas.DrawBorderText("I", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder1!, secondRect);
            canvas.DrawRect(thirdRect, _greenPaint);
            canvas.DrawRect(thirdRect, _blackBorder1);
            canvas.DrawBorderText("F", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder1!, thirdRect);
            canvas.DrawRect(fourthRect, _darkOrangePaint);
            canvas.DrawRect(fourthRect, _blackBorder1);
            canvas.DrawBorderText("E", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder1!, fourthRect);
            canvas.DrawRect(bottomRect, _limeGreenPaint);
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
        private SKPaint? _selectPaint;
        private SKPaint? _pDrewPaint;
        private SKPaint? _redPaint;
        private SKPaint? _whitePaint;
        private SKPaint? _darkBluePaint;
        private SKPaint? _darkOrangePaint;
        private SKPaint? _limeGreenPaint;
        private SKPaint? _yellowPaint;
        private SKPaint? _lightBluePaint;
        private SKPaint? _deepPinkPaint;
        private SKPaint? _purplePaint;
        private SKPaint? _bluePaint;
        private SKPaint? _greenPaint;
        private SKPaint? _blackPaint;
        private SKPaint? _whiteBorder2;
        private SKPaint? _blackBorder2;
        private SKPaint? _blackBorder1;
        public void Init()
        {
            MainGraphics!.OriginalSize = new SKSize(100, 115); //change to what the original size is.
            _thisAssembly = Assembly.GetAssembly(GetType());
            SKColor thisColor = SKColors.Black;
            SKColor otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 50); //can experiment as needed.
            _selectPaint = MiscHelpers.GetSolidPaint(otherColor);
            thisColor = SKColors.White;
            otherColor = new SKColor(thisColor.Red, thisColor.Green, thisColor.Blue, 150);
            _pDrewPaint = MiscHelpers.GetSolidPaint(otherColor);
            _redPaint = MiscHelpers.GetSolidPaint(SKColors.Red);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _darkBluePaint = MiscHelpers.GetSolidPaint(SKColors.DarkBlue);
            _darkOrangePaint = MiscHelpers.GetSolidPaint(SKColors.DarkOrange);
            _limeGreenPaint = MiscHelpers.GetSolidPaint(SKColors.LimeGreen);
            _lightBluePaint = MiscHelpers.GetSolidPaint(SKColors.LightBlue);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
            _deepPinkPaint = MiscHelpers.GetSolidPaint(SKColors.DeepPink);
            _purplePaint = MiscHelpers.GetSolidPaint(SKColors.Purple);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _greenPaint = MiscHelpers.GetSolidPaint(SKColors.Green);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _whiteBorder2 = MiscHelpers.GetStrokePaint(SKColors.White, 2);
            _blackBorder2 = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _blackBorder1 = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        #region "Draw Symbols"
        private void DrawBoxAndArrow(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            bounds = SKRect.Create(bounds.Location.X + 3, bounds.Location.Y + 3, bounds.Width - 6, bounds.Height - 6);
            // *** Draw the box
            dc.DrawRect(bounds, _whiteBorder2);
            var thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 40);
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Top + (bounds.Width / 10)));
            thisPath.LineTo(bounds.Location.X + ((bounds.Width * 4) / 5), bounds.Top + (bounds.Height / 2));
            thisPath.LineTo(bounds.Location.X + ((bounds.Width * 3) / 5), bounds.Location.Y + (bounds.Height / 2));
            thisPath.LineTo(bounds.Location.X + ((bounds.Width * 3) / 5), bounds.Location.Y + ((bounds.Height * 9) / 10));
            thisPath.LineTo(bounds.Location.X + ((bounds.Width * 2) / 5), bounds.Location.Y + ((bounds.Height * 9) / 10));
            thisPath.LineTo(bounds.Location.X + ((bounds.Width * 2) / 5), bounds.Location.Y + (bounds.Height / 2));
            thisPath.LineTo(bounds.Location.X + (bounds.Width / 5), bounds.Location.Y + (bounds.Height / 2));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
        }
        private void DrawWeddingRing(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            bounds = SKRect.Create(bounds.Location.X - 1, bounds.Location.Y - 1, bounds.Width, bounds.Height);
            var thickBorder = MiscHelpers.GetStrokePaint(SKColors.White, bounds.Width / 100);
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 21) / 40), bounds.Location.Y + (bounds.Height / 12)));
            thisPath.AddLine(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 3), bounds.Location.X + (bounds.Width * 19 / 40), bounds.Location.Y + (bounds.Height / 12));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 21) / 40), bounds.Location.Y + (bounds.Height / 12)));
            thisPath.AddLine(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 3), bounds.Location.X + (bounds.Width * 23 / 40), bounds.Location.Y + (bounds.Height / 11));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 19) / 40), bounds.Location.Y + (bounds.Height / 12)));
            thisPath.AddLine(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 3), bounds.Location.X + (bounds.Width * 17 / 40), bounds.Location.Y + (bounds.Height / 11));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 23) / 40), bounds.Location.Y + (bounds.Height / 11)));
            thisPath.AddLine(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 3), bounds.Location.X + (bounds.Width * 25 / 40), bounds.Location.Y + (bounds.Height / 10));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 17) / 40), bounds.Location.Y + (bounds.Height / 11)));
            thisPath.AddLine(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 3), bounds.Location.X + (bounds.Width * 15 / 40), bounds.Location.Y + (bounds.Height / 10));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            // *** Draw the ring
            SKPoint centerPoint;
            float radiusX;
            float radiusY;
            centerPoint = new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 2) / 3));
            radiusX = bounds.Width / 3;
            radiusY = bounds.Height / 3;
            dc.DrawOval(centerPoint.X, centerPoint.Y, radiusX, radiusY, _whiteBorder2);
        }
        private void DrawDiploma(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            var thisPaint = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.LightGray, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            var thisStroke = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 100);
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 6), bounds.Location.Y + (bounds.Height / 10)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + (bounds.Width / 6), bounds.Location.Y + ((bounds.Height * 4) / 10)), new SKSize(bounds.Width / 6, bounds.Height / 6), 180, false, SKPathDirection.CounterClockwise);
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 6), bounds.Location.Y + ((bounds.Height * 9) / 10)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 6), bounds.Location.Y + ((bounds.Height * 6) / 10)), new SKSize(bounds.Width / 6, bounds.Height / 6), 180, false, SKPathDirection.CounterClockwise);
            thisPath.Close();
            dc.DrawPath(thisPath, thisPaint);
            dc.DrawPath(thisPath, thisStroke);
            dc.DrawLine(new SKPoint(bounds.Location.X + ((bounds.Width * 4) / 50), bounds.Location.Y + ((bounds.Height * 6) / 20)), new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 6), bounds.Top + ((bounds.Height * 17) / 20)), thisStroke);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 6), bounds.Location.Y + ((bounds.Height * 9) / 10)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 6), bounds.Location.Y + ((bounds.Height * 6) / 10)), new SKSize(bounds.Width / 6, bounds.Height / 6), 180, false, SKPathDirection.CounterClockwise);
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 6), bounds.Location.Y + ((bounds.Height * 9) / 10)), new SKSize(bounds.Width / 6, bounds.Height / 6), 180, false, SKPathDirection.CounterClockwise);
            thisPaint = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.LightGray, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            dc.DrawPath(thisPath, thisPaint);
            var penRibbon = MiscHelpers.GetLinearGradientPaint(SKColors.Red, SKColors.DarkRed, bounds, MiscHelpers.EnumLinearGradientPercent.Angle135); // this is the closest
            penRibbon.Style = SKPaintStyle.Stroke; // i think
            penRibbon.StrokeWidth = bounds.Width / 20;
            penRibbon.StrokeCap = SKStrokeCap.Round;
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 7) / 20)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 4) / 10), bounds.Location.Y + ((bounds.Height * 23) / 40)), new SKSize(bounds.Width / 4, bounds.Height / 4), 80, false, SKPathDirection.CounterClockwise);
            dc.DrawPath(thisPath, penRibbon);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 7) / 20)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 7) / 10), bounds.Location.Y + ((bounds.Height * 10) / 40)), new SKSize(bounds.Width / 4, bounds.Height / 4), 80, false, SKPathDirection.CounterClockwise);
            dc.DrawPath(thisPath, penRibbon);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 7) / 20)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 10), bounds.Location.Y + ((bounds.Height * 8) / 40)), new SKSize(bounds.Width / 4, bounds.Height / 4), 80, false, SKPathDirection.CounterClockwise);
            dc.DrawPath(thisPath, penRibbon);
        }
        private void DrawPassport(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            var rect_Box = SKRect.Create(bounds.Location.X + (bounds.Width / 6), bounds.Location.Y + (bounds.Height / 8), (bounds.Width * 4) / 6, (bounds.Height * 6) / 8);
            var rect_Box2 = SKRect.Create(bounds.Location.X + (bounds.Width / 6), bounds.Location.Y + (bounds.Height / 8), ((bounds.Width * 4) / 6) + (rect_Box.Width / 30), (bounds.Height * 6) / 8);
            var rect_Box3 = SKRect.Create(bounds.Location.X + (bounds.Width / 6), bounds.Location.Y + (bounds.Height / 8), ((bounds.Width * 4) / 6) + (rect_Box.Width / 15), (bounds.Height * 6) / 8);
            dc.Save();
            dc.Skew(rect_Box.Location.X, rect_Box.Location.Y);
            dc.Skew(rect_Box.Location.X, rect_Box.Location.Y);
            var thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 50);
            dc.DrawRoundRect(rect_Box3, bounds.Width / 10, bounds.Width / 10, _whitePaint);
            dc.DrawRoundRect(rect_Box3, bounds.Width / 10, bounds.Width / 10, thickBorder);
            dc.Restore();
            dc.DrawRoundRect(rect_Box2, bounds.Width / 10, bounds.Width / 10, _whitePaint);
            dc.DrawRoundRect(rect_Box2, bounds.Width / 10, bounds.Width / 10, thickBorder);
            dc.Restore();
            dc.DrawRoundRect(rect_Box, bounds.Width / 10, bounds.Width / 10, _whitePaint);
            dc.DrawRoundRect(rect_Box, bounds.Width / 10, bounds.Width / 10, thickBorder);
            // *** Draw the center
            var rect_Center = SKRect.Create(rect_Box.Location.X + (rect_Box.Width / 4), rect_Box.Location.Y + (rect_Box.Height / 4), rect_Box.Width / 2, rect_Box.Height / 4);
            dc.DrawRoundRect(rect_Center, bounds.Width / 10, bounds.Width / 10, _whitePaint);
            dc.DrawRoundRect(rect_Center, bounds.Width / 10, bounds.Width / 10, thickBorder);
            var finalRect = SKRect.Create(rect_Center.Location.X + (rect_Center.Width / 3), rect_Center.Location.Y + (rect_Center.Height / 6), rect_Center.Height / 1.5f, rect_Center.Height / 1.5f);
            dc.DrawOval(finalRect, _darkBluePaint);
        }
        private void DrawHouse(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            SKPath thisPath = new SKPath();
            var thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 50);
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 10), bounds.Location.Y + (bounds.Height / 2)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 8) / 10), bounds.Location.Y + (bounds.Height / 2)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 2) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 2) / 10), bounds.Location.Y + (bounds.Height / 2)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + (bounds.Width / 10), bounds.Location.Y + (bounds.Height / 2)));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 5) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 15) / 20), bounds.Location.Y + (bounds.Height / 2)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 15) / 20), bounds.Location.Y + ((bounds.Height * 9) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 12) / 20), bounds.Location.Y + ((bounds.Height * 9) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 12) / 20), bounds.Location.Y + ((bounds.Height * 6) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 8) / 20), bounds.Location.Y + ((bounds.Height * 6) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 8) / 20), bounds.Location.Y + ((bounds.Height * 9) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 20), bounds.Location.Y + ((bounds.Height * 9) / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 20), bounds.Location.Y + (bounds.Height / 2)));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
        }
        private void DrawAirplane(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 19) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 21) / 40), bounds.Location.Y + ((bounds.Height * 18) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 13) / 20), bounds.Location.Y + ((bounds.Height * 19) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 13) / 20), bounds.Location.Y + ((bounds.Height * 35) / 40)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 11) / 20), bounds.Location.Y + ((bounds.Height * 16) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 23) / 40), bounds.Location.Y + ((bounds.Height * 13) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 18) / 20), bounds.Location.Y + ((bounds.Height * 14) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 18) / 20), bounds.Location.Y + ((bounds.Height * 12) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 23) / 40), bounds.Location.Y + ((bounds.Height * 9) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 23) / 40), bounds.Location.Y + ((bounds.Height * 5) / 20)));
            thisPath.AddArc(new SKPoint(bounds.Location.X + ((bounds.Width * 17) / 40), bounds.Location.Y + ((bounds.Height * 5) / 20)), new SKSize((bounds.Width * 6) / 40, bounds.Height), 180, false, SKPathDirection.CounterClockwise);
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 17) / 40), bounds.Location.Y + ((bounds.Height * 5) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 17) / 40), bounds.Location.Y + ((bounds.Height * 9) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 2) / 20), bounds.Location.Y + ((bounds.Height * 12) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 2) / 20), bounds.Location.Y + ((bounds.Height * 14) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 17) / 40), bounds.Location.Y + ((bounds.Height * 13) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 20), bounds.Location.Y + ((bounds.Height * 16) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 7) / 20), bounds.Location.Y + ((bounds.Height * 35) / 40)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 7) / 20), bounds.Location.Y + ((bounds.Height * 19) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 19) / 40), bounds.Location.Y + ((bounds.Height * 18) / 20)));
            thisPath.Close();
            var ThickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 50);
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, ThickBorder);
        }
        private void DrawCar(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 10), bounds.Location.Y + ((bounds.Height * 14) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + (bounds.Width / 10), bounds.Location.Y + ((bounds.Height * 9) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 2) / 10), bounds.Location.Y + ((bounds.Height * 8) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 3) / 10), bounds.Location.Y + ((bounds.Height * 2) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 7) / 10), bounds.Location.Y + ((bounds.Height * 2) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 8) / 10), bounds.Location.Y + ((bounds.Height * 8) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 10), bounds.Location.Y + ((bounds.Height * 9) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 10), bounds.Location.Y + ((bounds.Height * 14) / 20)));
            thisPath.Close();
            var thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 50);
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 6) / 20), bounds.Location.Y + ((bounds.Height * 7) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 7) / 20), bounds.Location.Y + ((bounds.Height * 3) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 13) / 20), bounds.Location.Y + ((bounds.Height * 3) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 14) / 20), bounds.Location.Y + ((bounds.Height * 7) / 20)));
            thisPath.Close();
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            SKRect rect1;
            SKRect rect2;
            SKRect rect3;
            SKRect rect4;
            rect1 = SKRect.Create(bounds.Location.X + ((bounds.Width * 3) / 20), bounds.Location.Y + ((bounds.Height * 29) / 40), bounds.Width / 10, (bounds.Height * 2) / 10);
            rect2 = SKRect.Create(bounds.Location.X + ((bounds.Width * 15) / 20), bounds.Location.Y + ((bounds.Height * 29) / 40), bounds.Width / 10, (bounds.Height * 2) / 10);
            rect3 = SKRect.Create(bounds.Location.X + (bounds.Width / 4), bounds.Location.Y + ((bounds.Height * 11) / 20), bounds.Width / 15, bounds.Width / 15);
            rect4 = SKRect.Create(bounds.Location.X + ((bounds.Width * 3) / 4), bounds.Location.Y + ((bounds.Height * 11) / 20), bounds.Width / 15, bounds.Width / 15);
            dc.DrawRect(rect1, _whitePaint);
            dc.DrawRect(rect1, thickBorder);
            dc.DrawRect(rect2, _whitePaint);
            dc.DrawRect(rect2, thickBorder);
            dc.DrawOval(rect3, _whitePaint);
            dc.DrawOval(rect4, _whitePaint);
        }
        private void DrawBoat(SKCanvas dc, SKRect bounds)
        {
            PrivateDrawRectangle(dc, bounds);
            SKPath thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + (bounds.Height / 10)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 10), bounds.Location.Y + ((bounds.Height * 15) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + (bounds.Width / 2), bounds.Location.Y + ((bounds.Height * 15) / 20)));
            thisPath.Close();
            var thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, bounds.Width / 50);
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 20), bounds.Location.Y + ((bounds.Height * 3) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + (bounds.Width / 10), bounds.Location.Y + ((bounds.Height * 15) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 9) / 20), bounds.Location.Y + ((bounds.Height * 15) / 20)));
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
            thisPath = new SKPath();
            thisPath.MoveTo(new SKPoint(bounds.Location.X + ((bounds.Width * 4) / 20), bounds.Location.Y + ((bounds.Height * 16) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 5) / 20), bounds.Location.Y + ((bounds.Height * 18) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 14) / 20), bounds.Location.Y + ((bounds.Height * 18) / 20)));
            thisPath.LineTo(new SKPoint(bounds.Location.X + ((bounds.Width * 16) / 20), bounds.Location.Y + ((bounds.Height * 16) / 20)));
            dc.DrawPath(thisPath, _whitePaint);
            dc.DrawPath(thisPath, thickBorder);
        }
        private void PrivateDrawRectangle(SKCanvas thisCanvas, SKRect thisRect)
        {
            thisCanvas.DrawRect(thisRect, _redPaint);
        }
        #endregion
        private void DrawTopPortion(SKCanvas dc) // done now.
        {
            var bounds = MainGraphics!.GetActualRectangle(5, 5, 90, 20);
            dc.DrawRect(bounds, _darkOrangePaint);
            dc.DrawRect(bounds, _blackBorder1); // i think
            SKRect firstRect;
            firstRect = MainGraphics.GetActualRectangle(5, 5, 20, 20);
            var lastRect = MainGraphics.GetActualRectangle(71, 5, 20, 20);
            var fontSize = MainGraphics.GetFontSize(15); // i think.  can be adjusted
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            dc.DrawRect(firstRect, _limeGreenPaint);
            dc.DrawRect(firstRect, _blackBorder2);
            dc.DrawCustomText(Points.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            if (Requirement != EnumSpecialCardCategory.None && Action != EnumAction.MovingHouse)
            {
                switch (Requirement)
                {
                    case EnumSpecialCardCategory.Airplane:
                        {
                            DrawAirplane(dc, lastRect);
                            break;
                        }

                    case EnumSpecialCardCategory.Boat:
                        {
                            DrawBoat(dc, lastRect);
                            break;
                        }

                    case EnumSpecialCardCategory.Car:
                        {
                            DrawCar(dc, lastRect);
                            break;
                        }

                    case EnumSpecialCardCategory.Degree:
                        {
                            DrawDiploma(dc, lastRect);
                            break;
                        }

                    case EnumSpecialCardCategory.House:
                        {
                            DrawHouse(dc, lastRect);
                            break;
                        }

                    case EnumSpecialCardCategory.Marriage:
                        {
                            DrawWeddingRing(dc, lastRect);
                            break;
                        }

                    case EnumSpecialCardCategory.Passport:
                        {
                            DrawPassport(dc, lastRect);
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Don't know what it is for the symbol");
                        }
                }
            }
            else if ((int)SpecialCategory == (int)EnumSpecialCardCategory.Switch)
            {
                DrawBoxAndArrow(dc, lastRect);
            }
        }
        private void DrawPayday(SKCanvas dc) // done
        {
            var firstRect = MainGraphics!.GetActualRectangle(3, 15, 94, 60);
            var fontSize = MainGraphics.GetFontSize(40);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
            dc.DrawBorderText("$", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder2!, firstRect);
            fontSize = MainGraphics.GetFontSize(12);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            var firstList = Description.Split("|").ToCustomBasicList();
            int Tops;
            Tops = 65;
            foreach (var thisText in firstList)
            {
                var lastRect = MainGraphics.GetActualRectangle(3, Tops, 94, 15);
                dc.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, lastRect, out _);
                Tops += 15; // i think
            }
        }
        private void DrawAction(SKCanvas dc) // done
        {
            var topRect = MainGraphics!.GetActualRectangle(3, 25, 94, 13);
            bool isComplex = false;
            string lastBold;
            if (OpponentKeepsCard == true)
            {
                if ((int)Action == (int)EnumAction.MidlifeCrisis)
                {
                    isComplex = true;
                    lastBold = "They put this card in|their life story and|take a new card.";
                }
                else
                {
                    lastBold = "They keep this card.";
                }
            }
            else if ((int)Action == (int)EnumAction.LifeSwap || (int)Action == (int)EnumAction.SecondChance)
            {
                lastBold = "You keep this card.";
            }
            else
            {
                lastBold = "";
            }
            string firstBold;
            switch (Action)
            {
                case EnumAction.Lawsuit:
                    {
                        firstBold = "Lawsuit";
                        break;
                    }

                case EnumAction.IMTheBoss:
                    {
                        firstBold = "I'm The Boss";
                        break;
                    }

                case EnumAction.YoureFired:
                    {
                        firstBold = "You're Fired";
                        break;
                    }

                case EnumAction.TurnBackTime:
                    {
                        firstBold = "Turn Back Time";
                        break;
                    }

                case EnumAction.CareerSwap:
                    {
                        firstBold = "Career Swap";
                        break;
                    }

                case EnumAction.LostPassport:
                    {
                        firstBold = "Lost Passport";
                        break;
                    }

                case EnumAction.YourStory:
                    {
                        firstBold = "Your Story";
                        break;
                    }

                case EnumAction.LifeSwap:
                    {
                        firstBold = "Life Swap";
                        break;
                    }

                case EnumAction.SecondChance:
                    {
                        firstBold = "Second Chance";
                        break;
                    }

                case EnumAction.AdoptBaby:
                    {
                        firstBold = "Adopt a Baby";
                        break;
                    }

                case EnumAction.LongLostRelative:
                    {
                        firstBold = "Long-Lost Relative";
                        break;
                    }

                case EnumAction.MidlifeCrisis:
                    {
                        firstBold = "Mid-Life Crisis";
                        break;
                    }

                case EnumAction.MixUpAtVets:
                    {
                        firstBold = "Mix-up At Vet's";
                        break;
                    }

                case EnumAction.DonateToCharity:
                    {
                        firstBold = "Donate to Charity";
                        break;
                    }

                case EnumAction.MovingHouse:
                    {
                        firstBold = "Moving House";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Don't know what to do about " + Action.ToString());
                    }
            }
            var fontSize = MainGraphics.GetFontSize(9);
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            var simpleBottomRect = MainGraphics.GetActualRectangle(3, 100, 94, 10);
            dc.DrawCustomText(firstBold, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, topRect, out _);
            int tops;
            var middleText = Description.Split("|").ToCustomBasicList();
            tops = 45; // was 37.  well see what happens from here.
            fontSize = MainGraphics.GetFontSize(10);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            foreach (var thisMiddle in middleText)
            {
                var middleRect = MainGraphics.GetActualRectangle(3, tops, 94, 10);
                dc.DrawCustomText(thisMiddle, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, middleRect, out _);
                if (isComplex == false)
                    tops += 12;
                else
                    tops += 10;
            }
            if (string.IsNullOrEmpty(lastBold))
                return;
            fontSize = MainGraphics.GetFontSize(8);
            textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            if (isComplex == false)
            {
                dc.DrawCustomText(lastBold, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, simpleBottomRect, out _);
            }
            else
            {
                var thisList = lastBold.Split("|").ToCustomBasicList();
                fontSize = MainGraphics.GetFontSize(10);
                textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
                textPaint.FakeBoldText = true;
                tops = 75;
                foreach (var ThisItem in thisList)
                {
                    var ThisRect = MainGraphics.GetActualRectangle(3, tops, 94, 10);
                    dc.DrawCustomText(ThisItem, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, ThisRect, out _);
                    tops += 11;
                }
            }
        }
        private void DrawImageCards(SKCanvas dc, string fileName, string bottomText)
        {
            fileName = fileName.Replace("My.Resources._", "");
            fileName = fileName.Replace("_", "-");
            fileName += ".png";
            var thisBit = ImageExtensions.GetSkBitmap(_thisAssembly!, fileName);
            var topRect = MainGraphics!.GetActualRectangle(30, 30, 40, 40); // i think
            if ((int)SpecialCategory == (int)EnumSpecialCardCategory.Passport || (int)SpecialCategory == (int)EnumSpecialCardCategory.Degree)
                dc.DrawRect(topRect, _blackPaint);
            dc.DrawBitmap(thisBit, topRect, MainGraphics.BitPaint);
            float fontSize;
            string entireText;
            int heights;
            if (!string.IsNullOrEmpty(bottomText))
            {
                fontSize = MainGraphics.GetFontSize(9);
                heights = 12;
                entireText = Description + "|" + bottomText;
            }
            else
            {
                fontSize = MainGraphics.GetFontSize(10);
                heights = 14;
                entireText = Description;
            }
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            int tops;
            tops = 70; // i think
            var thisList = entireText.Split("|").ToCustomBasicList();
            foreach (var thisText in thisList)
            {
                var thisRect = MainGraphics.GetActualRectangle(3, tops, 94, heights);
                dc.DrawCustomText(thisText, TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, thisRect, out _);
                tops += heights + 1; // i think
            }
        }
        private SKPaint GetMainPaint()
        {
            if (Action != (int)EnumAction.None)
                return _darkOrangePaint!;
            switch (FirstCategory)
            {
                case EnumFirstCardCategory.Adventure:
                    {
                        return _yellowPaint!;
                    }

                case EnumFirstCardCategory.Career:
                    {
                        return _lightBluePaint!;
                    }

                case EnumFirstCardCategory.Family:
                    {
                        return _deepPinkPaint!;
                    }

                case EnumFirstCardCategory.Wealth:
                    {
                        return _limeGreenPaint!;
                    }

                default:
                    {
                        throw new BasicBlankException("Nothing for paints");
                    }
            }
        }
        public void DrawImage(SKCanvas canvas, SKRect rect_Card)
        {
            var thisPaint = GetMainPaint();
            if (Points == 0)
            {
                canvas.DrawBorderText("+10", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, thisPaint, _blackBorder2!, rect_Card);
                return;
            }
            var tempRect = MainGraphics!.GetActualRectangle(3, 3, 94, 109);
            canvas.DrawRect(tempRect, thisPaint);
            canvas.DrawRect(tempRect, _blackBorder2);
            DrawTopPortion(canvas);
            if (Points == 20 && (int)FirstCategory == (int)EnumFirstCardCategory.Career)
            {
                DrawPayday(canvas);
                DrawBorders(canvas, rect_Card); //try this.
                return;
            }
            if (Action != (int)EnumAction.None)
            {
                DrawAction(canvas);
                DrawBorders(canvas, rect_Card); //try this.
                return;
            }
            string bottomText = "";
            string picture;
            switch (SpecialCategory)
            {
                case EnumSpecialCardCategory.Boat:
                    {
                        bottomText = "(Boat)";
                        break;
                    }

                case EnumSpecialCardCategory.Airplane:
                    {
                        bottomText = "(Plane)";
                        break;
                    }

                case EnumSpecialCardCategory.Car:
                    {
                        bottomText = "(Car)";
                        break;
                    }

                case EnumSpecialCardCategory.House:
                    {
                        bottomText = "(House)";
                        break;
                    }

                case EnumSpecialCardCategory.Marriage:
                    {
                        bottomText = "(1 wedding|per life story)";
                        break;
                    }

                case EnumSpecialCardCategory.Passport:
                    {
                        bottomText = "(1 passport|per life story)";
                        break;
                    }
            }
            switch (SwitchCategory)
            {
                case EnumSwitchCategory.Career:
                    {
                        if (!string.IsNullOrEmpty(bottomText))
                            throw new BasicBlankException("Already has " + bottomText);
                        bottomText = "(Career)";
                        break;
                    }

                case EnumSwitchCategory.Baby:
                    {
                        if (!string.IsNullOrEmpty(bottomText))
                            throw new BasicBlankException("Already has " + bottomText);
                        bottomText = "(Baby)";
                        break;
                    }

                case EnumSwitchCategory.Pet:
                    {
                        if (!string.IsNullOrEmpty(bottomText))
                            throw new BasicBlankException("Already has " + bottomText);
                        bottomText = "(Pet)";
                        break;
                    }
            }
            if ((int)Requirement == (int)EnumSpecialCardCategory.House)
                bottomText = "Home improvement";
            switch (Description)
            {
                case "Degree":
                    {
                        picture = "My.Resources._7_University";
                        break;
                    }

                case "Stunt Double":
                    {
                        picture = "My.Resources._40_StuntDouble";
                        break;
                    }

                case "Pop Star":
                    {
                        picture = "My.Resources._11_PopStar";
                        break;
                    }

                case "Jet Pilot":
                    {
                        picture = "My.Resources._27_JetPilot";
                        break;
                    }

                case "Teacher":
                    {
                        picture = "My.Resources._51_Teacher";
                        break;
                    }

                case "Exotic Pet Vet":
                    {
                        picture = "My.Resources._52_ExoticPetVet";
                        break;
                    }

                case "Rocket Scientist":
                    {
                        picture = "My.Resources._49_RocketScientist";
                        break;
                    }

                case "Politician":
                    {
                        picture = "My.Resources._59_Politician";
                        break;
                    }

                case "Monkey":
                    {
                        picture = "My.Resources._43_Monkey";
                        break;
                    }

                case "Shark":
                    {
                        picture = "My.Resources._50_Shark";
                        break;
                    }

                case "Giraffe":
                    {
                        picture = "My.Resources._20_Giraffe";
                        break;
                    }

                case "Baby polar bear":
                    {
                        picture = "My.Resources._16_BabyPolarBear";
                        break;
                    }

                case "Lion":
                    {
                        picture = "My.Resources._15_Lion";
                        break;
                    }

                case "Vegas wedding":
                    {
                        picture = "My.Resources._1_VegasWedding";
                        break;
                    }

                case "Celebrity wedding":
                    {
                        picture = "My.Resources._2_CelebrityWedding";
                        break;
                    }

                case "Underwater wedding":
                    {
                        picture = "My.Resources._3_UnderwaterWedding";
                        break;
                    }

                case "Parachute wedding":
                    {
                        picture = "My.Resources._4_ParachuteWedding";
                        break;
                    }

                case "Beach wedding":
                    {
                        picture = "My.Resources._5_BeachWedding";
                        break;
                    }

                case "Fairytale wedding":
                    {
                        picture = "My.Resources._6_FairytaleWedding";
                        break;
                    }

                case "Golden|anniversary":
                    {
                        picture = "My.Resources._35_DiamondAnniversary";
                        break;
                    }

                case "Diamond|anniversary":
                    {
                        picture = "My.Resources._35_DiamondAnniversary";
                        break;
                    }

                case "Baby girl":
                    {
                        picture = "My.Resources._18_BabyGirl";
                        break;
                    }

                case "Baby triplets":
                    {
                        picture = "My.Resources._41_BabyTriplets";
                        break;
                    }

                case "Baby boy":
                    {
                        picture = "My.Resources._60_BabyBoy";
                        break;
                    }

                case "Baby girl twins":
                    {
                        picture = "My.Resources._54_BabyGirlTwins";
                        break;
                    }

                case "Baby boy twins":
                    {
                        picture = "My.Resources._19_TwinBoys";
                        break;
                    }

                case "Learn to|play the bonjos":
                    {
                        picture = "My.Resources._9_LearnToPlayTheBongos";
                        break;
                    }

                case "See a solar eclipse":
                    {
                        picture = "My.Resources._32_ViewSolarEclipse";
                        break;
                    }

                case "Go diving in|Niagara Falls":
                    {
                        picture = "My.Resources._39_GoDivingInNiagraFalls";
                        break;
                    }

                case "Headline at|a rock concert":
                    {
                        picture = "My.Resources._38_HeadlineAtRockConcert";
                        break;
                    }

                case "Find a message|in a bottle":
                    {
                        picture = "My.Resources._23_FindMessageInABottle";
                        break;
                    }

                case "Go Skydiving":
                    {
                        picture = "My.Resources._37_GoSkydiving";
                        break;
                    }

                case "Ride the tallest|rollercoaster in|the world":
                    {
                        picture = "My.Resources._10_RideTheTallestRollerCoaster";
                        break;
                    }

                case "Swim with dolphins":
                    {
                        picture = "My.Resources._44_SwimWithDolphins";
                        break;
                    }

                case "Win a charity|skateboard contest":
                    {
                        picture = "My.Resources._25_WinACharitySkateboardContest";
                        break;
                    }

                case "Win the jackpot":
                    {
                        picture = "My.Resources._46_WinTheJackpot";
                        break;
                    }

                case "Fly high in a|hot-air balloon":
                    {
                        picture = "My.Resources._8_FlyHighInHotAirBalloon";
                        break;
                    }

                case "Dance at the Rio|Carnival":
                    {
                        picture = "My.Resources._58_DanceAtRioCarnival";
                        break;
                    }

                case "Trek to the North|Pole":
                    {
                        picture = "My.Resources._16_BabyPolarBear";
                        break;
                    }

                case "Explore a|live volcano":
                    {
                        picture = "My.Resources._22_ExploreLiveVolcano";
                        break;
                    }

                case "Dig up dinosaur|fossils":
                    {
                        picture = "My.Resources._48_DigUpDinosaurFossil";
                        break;
                    }

                case "Travel to|the Moon|in a rocket":
                    {
                        picture = "My.Resources._26_TravelToMoonInRocket";
                        break;
                    }

                case "Find Big Foot":
                    {
                        picture = "My.Resources._21_FindBigFoot";
                        break;
                    }

                case "Win the Jungle|Safari Rally":
                    {
                        picture = "My.Resources._56_WinJungleSafariRaffle";
                        break;
                    }

                case "Sail solo around|the world":
                    {
                        picture = "My.Resources._57_SailAroundWorld";
                        break;
                    }

                case "Learn to|loop-the-loop":
                    {
                        picture = "My.Resources._55_LearnTheLoopTheLoop";
                        break;
                    }

                case "Passport":
                    {
                        picture = "My.Resources._12_PictureOfPeopleCarryingLuggage";
                        break;
                    }

                case "Pink Cadillac":
                    {
                        picture = "My.Resources._14_PinkCadillac";
                        break;
                    }

                case "Eco-bubble car":
                    {
                        picture = "My.Resources._17_EcoBubbleCar";
                        break;
                    }

                case "Racing yacht":
                    {
                        picture = "My.Resources._36_RacingYacht";
                        break;
                    }

                case "Bathtub boat":
                    {
                        picture = "My.Resources._34_BathtubBoat";
                        break;
                    }

                case "Private jet":
                    {
                        picture = "My.Resources._47_PrivateJet";
                        break;
                    }

                case "Treehouse":
                    {
                        picture = "My.Resources._13_Treehouse";
                        break;
                    }

                case "Igloo":
                    {
                        picture = "My.Resources._45_Igloo";
                        break;
                    }

                case "Lighthouse":
                    {
                        picture = "My.Resources._30_Lighthouse";
                        break;
                    }

                case "Beach house":
                    {
                        picture = "My.Resources._31_BeachHouse";
                        break;
                    }

                case "Ranch":
                    {
                        picture = "My.Resources._29_RanchHouse";
                        break;
                    }

                case "Eco house":
                    {
                        picture = "My.Resources._28_EcoHouse";
                        break;
                    }

                case "Castle":
                    {
                        picture = "My.Resources._24_Castle";
                        break;
                    }

                case "Build a|swimming pool":
                    {
                        picture = "My.Resources._33_BuildASwimmingPool";
                        break;
                    }

                case "Switch to|natural power":
                    {
                        picture = "My.Resources._53_SwitchToNaturalPower";
                        break;
                    }

                case "Build a multi-screen|cinema":
                    {
                        picture = "My.Resources._41_BuildMultiScreenCinema";
                        break;
                    }

                default:
                    {
                        throw new BasicBlankException("Can't find an image for " + Description);
                    }
            }
            DrawImageCards(canvas, picture, bottomText);
            DrawBorders(canvas, rect_Card); //try this.
        }
    }
}
