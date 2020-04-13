using BackgammonCP.Data;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.GameGraphicsCP.MiscClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BackgammonCP.Graphics
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerChoiceCP<EnumColorChoice>>
    {

        private readonly BackgammonGameContainer _gameContainer;

        public GameBoardGraphicsCP(BackgammonGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
        }
        public override string TagUsed => "main";
        protected override SKSize OriginalSize { get; set; } = new SKSize(571, 436);
        private SKRect _rect1;
        private SKRect _rect2;
        private SKRect _rect3;
        private SKRect _rect_Inner1;
        private SKRect _rect_Inner2;
        private readonly Dictionary<int, SpaceCP> _spaceList = new Dictionary<int, SpaceCP>(); // this is the space

        protected override bool CanStartPaint()
        {
            return true;
        }
        public SKRect GetRectangleSpace(int space)
        {
            SpaceCP obj_Space;
            if (_spaceList.ContainsKey(space))
            {
                obj_Space = _spaceList[space];
                return obj_Space.Bounds; // i think its this simple.
            }
            else
            {
                return default;
            }
        }
        private bool _alreadyCreated;
        internal void SavedBoard()
        {
            CreateSpaces();
        }
        protected override void CreateSpaces()
        {
            if (_alreadyCreated == true)
                return;
            _spaceList.Clear();
            var thisSize = GetActualSize(30, 30);
            PieceHeight = thisSize.Height;
            PieceWidth = thisSize.Width;
            var tempbounds = GetBounds(); // i think
            SKRect bounds;
            int x;
            _rect1 = SKRect.Create(tempbounds.Left, tempbounds.Top, tempbounds.Width * 0.45f, tempbounds.Height);
            _rect2 = SKRect.Create(tempbounds.Left + _rect1.Width, tempbounds.Top, tempbounds.Width * 0.45f, tempbounds.Height);
            _rect3 = SKRect.Create(tempbounds.Left + _rect1.Width + _rect2.Width, tempbounds.Top, tempbounds.Width * 0.1f, tempbounds.Height);
            _rect_Inner1 = SKRect.Create(_rect1.Left + (_rect1.Width / 20), _rect1.Top + (_rect1.Width / 20), (_rect1.Width * 9) / 10, _rect1.Height - (_rect1.Width / 10));
            _rect_Inner2 = SKRect.Create(_rect2.Left + (_rect2.Width / 20), _rect2.Top + (_rect2.Width / 20), (_rect2.Width * 9) / 10, _rect2.Height - (_rect2.Width / 10));
            SpaceCP obj_Space;
            SKRect tempRect;
            for (x = 1; x <= 2; x++)
            {
                if (x == 1)
                    bounds = _rect_Inner1;
                else
                    bounds = _rect_Inner2;
                int int_Count;
                SKPath gp1;
                SKPath gp2;
                float int_Width = bounds.Width / 6;
                int_Width -= 1;
                int int_SpaceNumber;
                for (int_Count = 1; int_Count <= 6; int_Count++)
                {
                    gp1 = new SKPath();
                    gp1.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 1)), bounds.Top), new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.4f)), true);
                    gp1.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.4f)), new SKPoint(bounds.Left + (int_Width * (int_Count)), bounds.Top));
                    gp1.Close();
                    if (x == 2)
                        int_SpaceNumber = 7 - int_Count;
                    else
                        int_SpaceNumber = 13 - int_Count;
                    obj_Space = new SpaceCP();
                    obj_Space.Path = gp1;
                    tempRect = obj_Space.Path.Bounds;
                    obj_Space.Bounds = tempRect;
                    _spaceList.Add(int_SpaceNumber, obj_Space);
                    gp2 = new SKPath();
                    gp2.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 1)), bounds.Top + bounds.Height), new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.6f)), true);
                    gp2.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.6f)), new SKPoint(bounds.Left + (int_Width * (int_Count)), bounds.Top + bounds.Height));
                    gp2.Close();
                    if (x == 2)
                        int_SpaceNumber = 18 + int_Count;
                    else
                        int_SpaceNumber = 12 + int_Count;
                    obj_Space = new SpaceCP();
                    obj_Space.Path = gp2;
                    tempRect = obj_Space.Path.Bounds;
                    obj_Space.Bounds = tempRect;
                    _spaceList.Add(int_SpaceNumber, obj_Space);
                }
            }
            SKPath gp;
            obj_Space = new SpaceCP();
            SKRect tempInner;
            tempInner = SKRect.Create(_rect3.Left + (_rect2.Width / 20), _rect3.Top + (_rect2.Width / 20), _rect3.Width - (_rect2.Width / 10), (_rect2.Height / 2) - (_rect2.Width / 10));
            gp = new SKPath();
            gp.AddRect(tempInner);
            obj_Space.Path = gp;
            tempRect = tempInner;
            obj_Space.Bounds = tempRect;
            _spaceList.Add(26, obj_Space);
            tempInner = SKRect.Create(_rect3.Left + (_rect2.Width / 20), _rect3.Top + (_rect3.Height / 2) + (_rect2.Width / 20), _rect3.Width - (_rect2.Width / 10), (_rect2.Height / 2) - (_rect2.Width / 10));
            gp = new SKPath();
            gp.AddRect(tempInner);
            obj_Space = new SpaceCP();
            obj_Space.Path = gp;
            tempRect = obj_Space.Path.Bounds;
            obj_Space.Bounds = tempRect;
            _spaceList.Add(25, obj_Space);
            gp = new SKPath();
            gp.AddRect(SKRect.Create(_rect1.Left + _rect1.Width - (_rect1.Width / 20) - (_rect1.Width * 0.5f), _rect1.Top + (_rect1.Height / 2) - (_rect1.Height / 30), _rect1.Width * 0.5f, (_rect1.Height / 15)));
            obj_Space = new SpaceCP();
            obj_Space.Path = gp;
            tempRect = obj_Space.Path.Bounds;
            obj_Space.Bounds = tempRect;
            _spaceList.Add(27, obj_Space);
            gp = new SKPath();
            gp.AddRect(SKRect.Create(_rect1.Left + _rect1.Width + (_rect1.Width / 20), _rect1.Top + (_rect1.Height / 2) - (_rect1.Height / 30), _rect1.Width * 0.5f, (_rect1.Height / 15)));
            obj_Space = new SpaceCP();
            obj_Space.Path = gp;
            tempRect = obj_Space.Path.Bounds;
            obj_Space.Bounds = SKRect.Create(tempRect.Left, tempRect.Top, tempRect.Width, tempRect.Height);
            _spaceList.Add(0, obj_Space);
            _alreadyCreated = true;
        }
        private SKPaint? _siennaPaint;
        private SKPaint? _blackBorder;
        private SKPaint? _oldLacePaint;
        private SKPaint? _redBorder;
        private SKPaint? _limeBorder;
        private SKPaint? _violetBorder;
        protected override void SetUpPaints()
        {
            _siennaPaint = MiscHelpers.GetSolidPaint(SKColors.Sienna);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2); // try 2 for this
            _oldLacePaint = MiscHelpers.GetSolidPaint(SKColors.OldLace);
            _redBorder = MiscHelpers.GetStrokePaint(SKColors.Red, 3);
            _limeBorder = MiscHelpers.GetStrokePaint(SKColors.LimeGreen, 3);
            _violetBorder = MiscHelpers.GetStrokePaint(SKColors.Violet, 3);
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_gameContainer.Command.IsExecuting)
            {
                return;
            }
            if (_gameContainer.MoveInProgress || _gameContainer.MoveList.Count == 0 || _gameContainer.SaveRoot.GameStatus == EnumGameStatus.EndingTurn)
            {
                return;
            }
            if (_gameContainer.MakeMoveAsync == null)
            {
                throw new BasicBlankException("Nobody is handling make move.  Rethink");
            }
            foreach (var thisItem in _spaceList.Values)
            {
                if (MiscHelpers.DidClickRectangle(thisItem.Bounds, thisPoint) == true)
                {
                    var thisNum = _spaceList.GetKey(thisItem); //hopefully it works this time.
                    if (thisNum == 26 || thisNum == 27)
                    {
                        return;// you can never click on the start or stack of the opponent
                    }
                    await _gameContainer.ProcessCustomCommandAsync(_gameContainer.MakeMoveAsync, thisNum);
                    break;
                }
            }
        }
        private void DrawSpaces(SKCanvas thisCanvas, SKRect bounds, SKColor clr_1, SKColor clr_2)
        {
            int int_Count;
            SKPath gp1;
            SKPath gp2;
            float int_Width = bounds.Width / 6;
            int_Width -= 1;
            bounds = SKRect.Create(bounds.Location.X + 2, bounds.Location.Y, bounds.Width - 4, bounds.Height);
            for (int_Count = 1; int_Count <= 6; int_Count++)
            {
                gp1 = new SKPath();
                gp1.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 1)), bounds.Top), new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.4f)), true);
                gp1.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.4f)), new SKPoint(bounds.Left + (int_Width * (int_Count)), bounds.Top));
                gp1.Close();
                gp2 = new SKPath();
                gp2.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 1)), bounds.Top + bounds.Height), new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.6f)), true);
                gp2.AddLine(new SKPoint(bounds.Left + (int_Width * (int_Count - 0.5f)), bounds.Top + (bounds.Height * 0.6f)), new SKPoint(bounds.Left + (int_Width * (int_Count)), bounds.Top + bounds.Height));
                gp2.Close();
                var Color1 = MiscHelpers.GetSolidPaint(clr_1);
                var Color2 = MiscHelpers.GetSolidPaint(clr_2);
                if ((int_Count % 2) == 0)
                {
                    thisCanvas.DrawPath(gp1, Color2);
                    thisCanvas.DrawPath(gp2, Color1);
                }
                else
                {
                    thisCanvas.DrawPath(gp2, Color2);
                    thisCanvas.DrawPath(gp1, Color1);
                }
            }
        }
        private void DrawOutline(SKCanvas thisCanvas) // at this time, we already have the holders in place
        {
            var bounds = GetBounds(); // i proved by drawing the entire rectangle it works,.
            SKPaint br_Fill;
            SKPaint pn_Fill;
            thisCanvas.DrawRect(bounds, _siennaPaint);
            thisCanvas.DrawRect(_rect1, _blackBorder);
            thisCanvas.DrawRect(_rect2, _blackBorder);
            thisCanvas.DrawRect(_rect3, _blackBorder);
            thisCanvas.DrawRect(_rect_Inner1, _oldLacePaint);
            thisCanvas.DrawRect(_rect_Inner2, _oldLacePaint);
            DrawSpaces(thisCanvas, _rect_Inner1, SKColors.Black, SKColors.DarkRed);
            DrawSpaces(thisCanvas, _rect_Inner2, SKColors.DarkRed, SKColors.Black);
            pn_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.SaddleBrown, SKColors.BurlyWood, SKRect.Create(_rect_Inner1.Location.X + (bounds.Width / 300), _rect_Inner1.Location.Y - (bounds.Width / 300), _rect_Inner1.Width + (bounds.Width / 150), _rect_Inner1.Height + (bounds.Width / 150)), MiscHelpers.EnumLinearGradientPercent.Angle45);
            pn_Fill.Style = SKPaintStyle.Stroke;
            pn_Fill.StrokeWidth = bounds.Width / 150;
            thisCanvas.DrawRect(_rect_Inner1, pn_Fill); // i think
            pn_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.SaddleBrown, SKColors.BurlyWood, SKRect.Create(_rect_Inner2.Location.X + (bounds.Width / 300), _rect_Inner2.Location.Y - (bounds.Width / 300), _rect_Inner2.Width + (bounds.Width / 150), _rect_Inner2.Height + (bounds.Width / 150)), MiscHelpers.EnumLinearGradientPercent.Angle45);
            pn_Fill.Style = SKPaintStyle.Stroke;
            pn_Fill.StrokeWidth = bounds.Width / 150; // try 2.  can decrease if necessary
            thisCanvas.DrawRect(_rect_Inner2, pn_Fill);
            var newInner = SKRect.Create(_rect3.Location.X + (_rect2.Width / 20), _rect3.Location.Y + (_rect2.Width / 20), _rect3.Width - (_rect2.Width / 10), (_rect2.Height / 2) - (_rect2.Width / 10));
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.SaddleBrown, newInner, MiscHelpers.EnumLinearGradientPercent.Angle45);
            thisCanvas.DrawRoundRect(newInner, newInner.Width * 0.4f, newInner.Width * 0.4f, br_Fill);
            pn_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.SaddleBrown, SKColors.BurlyWood, SKRect.Create(newInner.Location.X + (bounds.Width / 300), newInner.Location.Y - (bounds.Width / 300), _rect_Inner1.Width + (bounds.Width / 150), newInner.Height + (bounds.Width / 150)), MiscHelpers.EnumLinearGradientPercent.Angle45);
            pn_Fill.Style = SKPaintStyle.Stroke;
            pn_Fill.StrokeWidth = bounds.Width / 150;
            thisCanvas.DrawRoundRect(newInner, newInner.Width * 0.4f, newInner.Width * 0.4f, pn_Fill);
            newInner = SKRect.Create(_rect3.Location.X + (_rect2.Width / 20), _rect3.Location.Y + (_rect3.Height / 2) + (_rect2.Width / 20), _rect3.Width - (_rect2.Width / 10), (_rect2.Height / 2) - (_rect2.Width / 10));
            br_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.Black, SKColors.SaddleBrown, newInner, MiscHelpers.EnumLinearGradientPercent.Angle45);
            thisCanvas.DrawRoundRect(newInner, newInner.Width * 0.4f, newInner.Width * 0.4f, br_Fill);
            pn_Fill = MiscHelpers.GetLinearGradientPaint(SKColors.SaddleBrown, SKColors.BurlyWood, SKRect.Create(newInner.Location.X + (bounds.Width / 300), newInner.Location.Y - (bounds.Width / 300), _rect_Inner1.Width + (bounds.Width / 150), newInner.Height + (bounds.Width / 150)), MiscHelpers.EnumLinearGradientPercent.Angle45);
            pn_Fill.Style = SKPaintStyle.Stroke;
            pn_Fill.StrokeWidth = bounds.Width / 150;
            thisCanvas.DrawRoundRect(newInner, newInner.Width * 0.4f, newInner.Width * 0.4f, pn_Fill);
            var tempRect = GetActualRectangle(360, 2, 60, 10);
            var thisPaint = MiscHelpers.GetSolidPaint(SKColors.Aqua);
            thisCanvas.DrawArrow(tempRect, thisPaint, EnumArrowDirection.Left);
            tempRect = GetActualRectangle(100, 2, 60, 10);
            thisCanvas.DrawArrow(tempRect, thisPaint, EnumArrowDirection.Left);
            tempRect = GetActualRectangle(100, 424, 60, 10);
            thisCanvas.DrawArrow(tempRect, thisPaint, EnumArrowDirection.Right);
            tempRect = GetActualRectangle(360, 424, 60, 10);
            thisCanvas.DrawArrow(tempRect, thisPaint, EnumArrowDirection.Right);
        }
        //internal bool Saved;
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            //if colors was not chosen, this would not even run anymore.
            DrawOutline(canvas);
            string thisColor;
            CheckerPiecesCP thisPiece;
            var tempSize = GetActualSize(50, 50); // can adjust as needed (?)
            thisColor = _gameContainer.SingleInfo!.Color.ToColor(); //i think.
            var thisLocation = GetActualPoint(20, 190);
            thisPiece = GetGamePiece(thisColor, thisLocation);
            thisPiece.ActualHeight = tempSize.Height; // this is the only exception.
            thisPiece.ActualWidth = tempSize.Width;
            thisPiece.DrawImage(canvas); // this is the player whose turn it is.
            BackgammonPlayerItem currentPlayer;
            int index;
            foreach (var thisTriangle in _gameContainer.TriangleList.Values)
            {
                if (thisTriangle.NumberOfTiles > 0)
                {
                    currentPlayer = _gameContainer.PlayerList![thisTriangle.PlayerOwns];
                    thisColor = currentPlayer.Color.ToColor();
                    index = _gameContainer.TriangleList.GetKey(thisTriangle); //hopefully this works too.
                    bool alreadyDrew;
                    int newID;
                    newID = 0;
                    alreadyDrew = false;
                    if (index == 0 || index == 27)
                    {
                        if (thisTriangle.NumberOfTiles > 4)
                        {
                            var thisPoint = thisTriangle.Locations[2];
                            SKPaint textPaint;
                            var thisSize = GetActualSize(100, 50);
                            var thisRect = SKRect.Create(thisPoint, thisSize);
                            textPaint = MiscHelpers.GetTextPaint(thisColor.ToSKColor(), thisSize.Height * 0.9f);
                            canvas.DrawCustomText(thisTriangle.NumberOfTiles.ToString(), TextExtensions.EnumLayoutOptions.Start, TextExtensions.EnumLayoutOptions.Start, textPaint, thisRect, out _);
                            alreadyDrew = true;
                        }
                    }
                    foreach (var thisLoc in thisTriangle.Locations)
                    {
                        newID += 1;
                        if (alreadyDrew == false || newID == 1)
                        {
                            thisPiece = GetGamePiece(thisColor, thisLoc);
                            if (index == 25 || index == 26)
                                thisPiece.FlatPiece = true;
                            else
                                thisPiece.FlatPiece = false;
                            thisPiece.DrawImage(canvas);
                        }
                    }
                }
            }
            if (_gameContainer.Animates!.AnimationGoing == true)
            {
                var thisPlayer = _gameContainer.PlayerList![_gameContainer.WhoTurn];
                thisColor = thisPlayer.Color.ToColor();
                thisPiece = GetGamePiece(thisColor, _gameContainer.Animates.CurrentLocation);
                thisPiece.DrawImage(canvas);
                return;
            }
            if (_gameContainer.SaveRoot!.GameStatus == EnumGameStatus.EndingTurn || _gameContainer.MoveInProgress)
                return;
            if (_gameContainer.MoveList.Count == 0 && _gameContainer.SaveRoot.SpaceHighlighted > -1)
                throw new BasicBlankException("No space can be highlighted if there are no moves.");
            if (_gameContainer.MoveList.Count == 0)
                return;
            if (_gameContainer.SaveRoot.SpaceHighlighted > -1)
            {
                var thisList = _gameContainer.MoveList.Where(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted).ToCustomBasicList();
                if (thisList.Count == 0)
                    throw new BasicBlankException("There was no moves. Therefore, nothing should have been highlighted");
                var currentSpace = _spaceList[_gameContainer.SaveRoot.SpaceHighlighted];
                canvas.DrawPath(currentSpace.Path, _redBorder);
                foreach (var thisMove in thisList)
                {
                    var newSpace = _spaceList[thisMove.SpaceTo];
                    canvas.DrawPath(newSpace.Path, _limeBorder);
                }
                return;
            }
            foreach (var thisMove in _gameContainer.MoveList)
            {
                var currentSpace = _spaceList[thisMove.SpaceFrom];
                canvas.DrawPath(currentSpace.Path, _violetBorder);
            }
        }
    }
}