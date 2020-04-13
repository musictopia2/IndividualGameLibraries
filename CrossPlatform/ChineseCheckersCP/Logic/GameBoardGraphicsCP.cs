using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using ChineseCheckersCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ChineseCheckersCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<MarblePiecesCP<EnumColorChoice>>
    {
        private readonly Dictionary<int, SKRect> _spaceList = new Dictionary<int, SKRect>(); // i think this is fine for this one.
        private readonly ChineseCheckersGameContainer _gameContainer;

        public GameBoardGraphicsCP(ChineseCheckersGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer; //hopefully no need to draw board early.  hopefully won't regret this.
            DrawBoardEarly = true; //looks like i do have this as well.
        }
        public override string TagUsed => "main";
        protected override SKSize OriginalSize { get; set; } = new SKSize(600, 600);
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            SKRect tempRect;
            await Task.CompletedTask;
            if (_gameContainer.CanMove == null)
            {
                throw new BasicBlankException("Nobody processed the canmove.  Rethink");
            }
            if (_gameContainer.MakeMoveAsync == null)
            {
                throw new BasicBlankException("Nobody processed the makemoveasync.  Rethink");
            }
            foreach (var thisSpace in _spaceList.Values)
            {
                if (_gameContainer.BasicData!.IsXamarinForms == false)
                    tempRect = thisSpace; // i think only small phone needs the extras (well see)
                else
                {
                    float lefts;
                    float tops;
                    float widths;
                    float heights;
                    lefts = thisSpace.Left;
                    float originalWidth;
                    originalWidth = thisSpace.Width;
                    float originalHeight;
                    originalHeight = thisSpace.Height;
                    float extraWidth;
                    float extraHeight;
                    extraWidth = originalWidth / 3;
                    extraHeight = originalHeight / 3; // try this way even so no gaps.
                    lefts -= extraWidth;
                    tops = thisSpace.Top - extraHeight;
                    widths = thisSpace.Width + extraWidth + extraWidth;
                    heights = thisSpace.Height + extraHeight + extraHeight;
                    tempRect = SKRect.Create(lefts, tops, widths, heights);
                }
                if (MiscHelpers.DidClickRectangle(tempRect, thisPoint) == true)
                {
                    var index = _spaceList.GetKey(thisSpace);
                    if (_gameContainer.CanMove())
                    {
                        await _gameContainer.MakeMoveAsync.Invoke(index);
                        return;
                    }
                }
            }
        }
        #region "Positioning Info"
        private int _int_SpaceCount = 0;
        public SKPoint LocationOfSpace(int index)
        {
            return _spaceList[index].Location;
        }
        protected override void CreateSpaces()
        {
            _spaceList.Clear();
            var bounds = GetBounds();
            _int_SpaceCount = 1; // start with one.  because this is one based
            AddLine(1, 1, true, bounds);
            AddLine(2, 2, false, bounds);
            AddLine(3, 3, true, bounds);
            AddLine(4, 4, false, bounds);
            AddLine(5, 13, true, bounds);
            AddLine(6, 12, false, bounds);
            AddLine(7, 11, true, bounds);
            AddLine(8, 10, false, bounds);
            AddLine(9, 9, true, bounds);
            AddLine(10, 10, false, bounds);
            AddLine(11, 11, true, bounds);
            AddLine(12, 12, false, bounds);
            AddLine(13, 13, true, bounds);
            AddLine(14, 4, false, bounds);
            AddLine(15, 3, true, bounds);
            AddLine(16, 2, false, bounds);
            AddLine(17, 1, true, bounds);
            if (_spaceList.Count == 0)
                throw new BasicBlankException("Failed to create spacelist");
            PieceHeight = (int)_spaceList[1].Height;
            PieceWidth = (int)_spaceList[1].Width;
        }
        private void AddLine(int int_Row, int int_Spaces, bool bln_HasCenter, SKRect bounds)
        {
            double int_Size;
            double int_OffsetX;
            double int_OffsetY;
            SKPoint pt_Center = new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
            int_Size = (int)bounds.Width / 32;
            int_OffsetX = int_Size;
            int_OffsetY = int_Size / 0.577350269; // divide by the tangent of 30 degrees
            var loopTo = int_Spaces;
            int int_Count;
            for (int_Count = 1; int_Count <= loopTo; int_Count++)
            {
                double int_OffsetFactorY = -(int_OffsetY * (int_Row - 9));
                int int_OffsetFactorX;
                SKRect rect;
                if (bln_HasCenter)
                {
                    int_OffsetFactorX = (int_Count - ((int_Spaces + 1) / 2)) * 2;
                    rect = SKRect.Create(pt_Center.X - ((float)int_Size / 2) + ((float)int_Size * int_OffsetFactorX), pt_Center.Y - ((float)int_Size / 2) + (float)int_OffsetFactorY, (float)int_Size, (float)int_Size);
                }
                else
                {
                    int_OffsetFactorX = int_Count - (int_Spaces / 2);
                    rect = SKRect.Create(pt_Center.X - ((float)int_Size / 2) - (float)int_OffsetX + ((float)int_OffsetX * int_OffsetFactorX * 2), pt_Center.Y - ((float)int_Size / 2) + (float)int_OffsetFactorY, (float)int_Size, (float)int_Size);
                }
                _spaceList.Add(_int_SpaceCount, rect);
                _int_SpaceCount += 1;
            }
        }
        #endregion
        #region "Paint Processes"
        private SKPaint? _smokePaint;
        private SKPaint? _limeGreenBorder;
        protected override void SetUpPaints()
        {
            _smokePaint = MiscHelpers.GetSolidPaint(SKColors.WhiteSmoke);
            _limeGreenBorder = MiscHelpers.GetStrokePaint(SKColors.LimeGreen, 4);
        }
        protected override bool CanStartPaint()
        {
            return true; //i think.
        }
        public override void DrawGraphicsForBoard(SKCanvas canvas, float width, float height)
        {
            SetUpPaints();
            CreateSpaces();
            var thisRect = GetBounds();
            DrawCompleteBoard(canvas, thisRect);
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            var thisColor = _gameContainer.SingleInfo!.Color;
            MarblePiecesCP<EnumColorChoice> thisMarble = new MarblePiecesCP<EnumColorChoice>();
            thisMarble.NeedsToClear = false;
            var thisSize = GetActualSize(80, 80); // well see
            var thisLocation = GetActualPoint(new SKPoint(10, 260)); // can experiment
            thisMarble.ActualHeight = thisSize.Height;
            thisMarble.ActualWidth = thisSize.Width;
            thisMarble.Location = thisLocation;
            thisMarble.MainColor = thisColor.ToColor(); //i think.
            thisMarble.DrawImage(canvas); // will draw this one.  however, there are more
            foreach (var thisPlayer in _gameContainer.PlayerList!) // would have to remove temporarily
            {
                foreach (var thisPiece in thisPlayer.PieceList)
                {
                    var thisSpace = _spaceList[thisPiece];
                    thisMarble = GetGamePiece(thisPlayer.Color.ToColor(), thisSpace.Location);
                    thisMarble.DrawImage(canvas);
                }
            }
            if (_gameContainer.Animates!.AnimationGoing == true)
            {
                thisMarble = GetGamePiece(_gameContainer.SingleInfo.Color.ToColor(), _gameContainer.Animates.CurrentLocation);
                thisMarble.DrawImage(canvas);
                return;
            }
            if (_gameContainer.SaveRoot!.PreviousSpace == 0)
                return;
            var newRect = _spaceList[_gameContainer.SaveRoot.PreviousSpace];
            canvas.DrawOval(newRect, _limeGreenBorder);
        }
        #endregion
        protected override MarblePiecesCP<EnumColorChoice> GetGamePiece(string color, SKPoint location)
        {
            var thisPiece = base.GetGamePiece(color, location);
            thisPiece.NeedsToClear = false;
            return thisPiece;
        }
        #region "Draw Routines"
        private void FillTriangle(SKCanvas canvas, int int_Space1, int int_Space2, int int_Space3, SKColor clr_Triangle)
        {
            SKPoint[] pts = new SKPoint[3];
            SKPath gp = new SKPath();
            var obj_Space1 = _spaceList[int_Space1];
            var obj_Space2 = _spaceList[int_Space2];
            var obj_Space3 = _spaceList[int_Space3];
            pts[0] = new SKPoint(obj_Space1.Left + (obj_Space1.Width / 2), obj_Space1.Top + (obj_Space1.Height / 2));
            pts[1] = new SKPoint(obj_Space2.Left + (obj_Space2.Width / 2), obj_Space2.Top + (obj_Space2.Height / 2));
            pts[2] = new SKPoint(obj_Space3.Left + (obj_Space3.Width / 2), obj_Space3.Top + (obj_Space3.Height / 2));
            var thisPaint = MiscHelpers.GetSolidPaint(clr_Triangle);
            gp.AddPoly(pts, true);
            canvas.DrawPath(gp, thisPaint);
        }
        private void DrawTriangle(SKCanvas canvas, SKRect obj_Space1, SKRect obj_Space2, SKRect obj_Space3)
        {
            SKPoint[] pts = new SKPoint[3];
            SKPath gp = new SKPath();
            pts[0] = new SKPoint(obj_Space1.Left + (obj_Space1.Width / 2), obj_Space1.Top + (obj_Space1.Height / 2));
            pts[1] = new SKPoint(obj_Space2.Left + (obj_Space2.Width / 2), obj_Space2.Top + (obj_Space2.Height / 2));
            pts[2] = new SKPoint(obj_Space3.Left + (obj_Space3.Width / 2), obj_Space3.Top + (obj_Space3.Height / 2));
            var thisPaint = MiscHelpers.GetStrokePaint(SKColors.Black, obj_Space1.Width / 20);
            gp.AddPoly(pts, true);
            canvas.DrawPath(gp, thisPaint);
        }
        private void DrawOutlineStar(SKCanvas canvas, SKRect bounds)
        {
            int int_Size;
            SKPoint pt_Center = new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
            SKMatrix tmp_Matrix = new SKMatrix();
            SKRect obj_Space;
            SKPoint[] pts = new SKPoint[3];
            SKPath gp = new SKPath();
            SKPath gp_Temp = new SKPath();
            SKMatrix.RotateDegrees(ref tmp_Matrix, 60, pt_Center.X, pt_Center.Y);
            int_Size = (int)bounds.Width / 32;
            var pn_Star = MiscHelpers.GetStrokePaint(SKColors.Black, int_Size / 10);
            obj_Space = _spaceList[103];
            pts[0] = new SKPoint((obj_Space.Left + (obj_Space.Width / 2) - ((int_Size / 2) * 0.577350269f)), obj_Space.Top);
            obj_Space = _spaceList[107];
            pts[2] = new SKPoint(obj_Space.Left + (obj_Space.Width / 2) + ((int_Size / 2) * 0.577350269f), obj_Space.Top);
            pts[1] = new SKPoint(pt_Center.X, pts[2].Y - ((pts[2].X - pt_Center.X) / 0.577350269f));
            gp_Temp.AddLine(pts[0], pts[1], true);
            gp_Temp.AddLine(pts[1], pts[2]);
            gp.AddPath(gp_Temp, SKPathAddMode.Extend); // 
            gp_Temp.Transform(tmp_Matrix);
            gp.AddPath(gp_Temp, SKPathAddMode.Extend);
            gp_Temp.Transform(tmp_Matrix);
            gp.AddPath(gp_Temp, SKPathAddMode.Extend);
            gp_Temp.Transform(tmp_Matrix);
            gp.AddPath(gp_Temp, SKPathAddMode.Extend);
            gp_Temp.Transform(tmp_Matrix);
            gp.AddPath(gp_Temp, SKPathAddMode.Extend);
            gp_Temp.Transform(tmp_Matrix);
            gp.AddPath(gp_Temp, SKPathAddMode.Extend);
            canvas.DrawPath(gp, pn_Star);
        }
        private void DrawBoardOutline(SKCanvas canvas, SKRect bounds)
        {
            int int_Row1Count;
            var br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.White, SKColors.LightGray, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawOval(bounds, br_Fill);
            canvas.DrawOval(SKRect.Create(bounds.Left + (bounds.Width / 40), bounds.Top + (bounds.Height / 40), (bounds.Width * 19) / 20, (bounds.Height * 19) / 20), _smokePaint);
            DrawOutlineStar(canvas, bounds);
            FillTriangle(canvas, 121, 103, 107, SKColors.Blue);
            FillTriangle(canvas, 111, 65, 107, SKColors.Yellow);
            FillTriangle(canvas, 23, 19, 65, SKColors.Gray);
            FillTriangle(canvas, 1, 15, 19, SKColors.Green);
            FillTriangle(canvas, 11, 15, 57, SKColors.Purple);
            FillTriangle(canvas, 99, 57, 103, SKColors.Red);
            int int_Row2Count;
            for (int_Row2Count = 2; int_Row2Count <= 2; int_Row2Count++)
                DrawTriangle(canvas, _spaceList[1], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
            int_Row1Count = 2;
            for (int_Row2Count = 4; int_Row2Count <= 5; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 4;
            for (int_Row2Count = 7; int_Row2Count <= 9; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 7;
            for (int_Row2Count = 15; int_Row2Count <= 18; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 24;
            for (int_Row2Count = 11; int_Row2Count <= 22; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 36;
            for (int_Row2Count = 24; int_Row2Count <= 34; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 47;
            for (int_Row2Count = 36; int_Row2Count <= 45; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 57;
            for (int_Row2Count = 47; int_Row2Count <= 55; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 48;
            for (int_Row2Count = 57; int_Row2Count <= 64; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 57;
            for (int_Row2Count = 66; int_Row2Count <= 74; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 66;
            for (int_Row2Count = 76; int_Row2Count <= 85; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 76;
            for (int_Row2Count = 87; int_Row2Count <= 97; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 87;
            for (int_Row2Count = 99; int_Row2Count <= 110; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 112;
            for (int_Row2Count = 103; int_Row2Count <= 106; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 116;
            for (int_Row2Count = 112; int_Row2Count <= 114; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 119;
            for (int_Row2Count = 116; int_Row2Count <= 117; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            int_Row1Count = 121;
            for (int_Row2Count = 119; int_Row2Count <= 119; int_Row2Count++)
            {
                DrawTriangle(canvas, _spaceList[int_Row1Count], _spaceList[int_Row2Count], _spaceList[int_Row2Count + 1]);
                int_Row1Count += 1;
            }
            var loopTo = _spaceList.Count;
            int int_Count;
            for (int_Count = 1; int_Count <= loopTo; int_Count++)
                DrawSpace(canvas, _spaceList[int_Count]);
        }
        private void DrawSpace(SKCanvas canvas, SKRect bounds)
        {
            var br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.SlateGray, bounds, MiscHelpers.EnumLinearGradientPercent.Angle45);
            canvas.DrawOval(bounds, br_Fill);
        }
        private void DrawSpaces(SKCanvas canvas)
        {
            var loopTo = _spaceList.Count;
            int int_Count;
            for (int_Count = 1; int_Count <= loopTo; int_Count++)
                DrawSpace(canvas, _spaceList[int_Count]);
        }
        private void DrawCompleteBoard(SKCanvas thisCanvas, SKRect bounds)
        {
            DrawBoardOutline(thisCanvas, bounds);
            DrawSpaces(thisCanvas);
        }
        #endregion
    }
}