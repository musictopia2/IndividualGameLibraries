using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TroubleCP.Data;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace TroubleCP.Graphics
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<MarblePiecesCP<EnumColorChoice>>
    {
        private readonly TroubleGameContainer _gameContainer;
        private readonly IBoardPosition? _pos;

        public GameBoardGraphicsCP(TroubleGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
            if (_gameContainer.BasicData!.IsXamarinForms)
            {
                _pos = _gameContainer.Resolver.Resolve<IBoardPosition>();
            }
            DrawBoardEarly = true;
        }

       
        public override void DrawGraphicsForBoard(SKCanvas canvas, float width, float height)
        {
            SetUpPaints();
            var thisRect = GetBounds();
            canvas.DrawBitmap(_thisBit, thisRect, _bitPaint);
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(600, 600);
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override MarblePiecesCP<EnumColorChoice> GetGamePiece(string color, SKPoint location)
        {
            var output = base.GetGamePiece(color, location);
            output.NeedsToClear = false;
            output.UseTrouble = true;
            return output;
        }
        public SKPoint LocationOfSpace(int Index)
        {
            return _spaceList[Index].Location;
        }
        public SKPoint RecommendedPointForDice
        {
            get
            {
                var pt_Center = new SKPoint(_actualWidth / 2, _actualHeight / 2);
                if (_gameContainer.BasicData!.IsXamarinForms == false)
                    return new SKPoint(pt_Center.X - (_actualWidth / 10), pt_Center.Y - (_actualHeight / 12));
                return _pos!.RecommendedPointForDice(pt_Center, _actualWidth, _actualHeight);
            }
        }
        private readonly bool _isTest = false; //for testing,  i can click anywhere.
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_isTest)
            {
                throw new BasicBlankException("Rethink test now.");
            }
            if (_gameContainer.Command.IsExecuting)
            {
                return;
            }
            if (MiscHelpers.DidClickRectangle(_diceRect, thisPoint))
            {
                if (_gameContainer.CanRollDice == null)
                {
                    throw new BasicBlankException("Nobody is handling can roll dice.  Rethink");
                }
                if (_gameContainer.RollDiceAsync == null)
                {
                    throw new BasicBlankException("Nobody is rolling the dice.  Rethink");
                }
                if (_gameContainer.CanRollDice.Invoke() == false)
                {
                    return;
                }
                await _gameContainer.ProcessCustomCommandAsync(_gameContainer.RollDiceAsync);
            }

            if ( _gameContainer.SaveRoot.DiceNumber == 0)
            {
                return; //hopefully this simple here too (?)
            }

           
            //if (_isTest == false)
            //{
            //    if (_thisMod!.SpaceCommand!.CanExecute(0) == false)
            //        return; //can't consider because it can't execute anyways.
            //}
            foreach (var thisSpace in _spaceList.Values)
            {
                SKRect tempRect;
                if (_gameContainer.BasicData!.IsXamarinForms == false || _isTest)
                    tempRect = thisSpace;
                else
                {
                    var lefts = thisSpace.Left; //this part was easy.
                    var originalWidth = thisSpace.Width;
                    var originalHeight = thisSpace.Height;
                    var extraWidth = originalWidth * 2;
                    var extraHeight = originalHeight * 2; //try divided instead of multiply.
                    lefts -= extraWidth;
                    var tops = thisSpace.Top - extraHeight;
                    var widths = thisSpace.Width + extraWidth + extraWidth;
                    var heights = thisSpace.Height + extraHeight + extraHeight;
                    tempRect = SKRect.Create(lefts, tops, widths, heights);
                }
                if (MiscHelpers.DidClickRectangle(tempRect, thisPoint))
                {
                    int index = _spaceList.GetKey(thisSpace);
                    if (_gameContainer.IsValidMove == null)
                    {
                        throw new BasicBlankException("Nobody is handling the isvalidmove.  Rethink");
                    }
                    if (_gameContainer.MakeMoveAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling the makemoveasync.  Rethink");
                    }
                    if (_gameContainer.IsValidMove(index))
                    {
                        await _gameContainer.ProcessCustomCommandAsync(_gameContainer.MakeMoveAsync, index);
                        return;
                    }
                }
            }
        }
        private readonly Dictionary<int, SKRect> _spaceList = new Dictionary<int, SKRect>(); //try this way.
        private SKPaint? _bitPaint;
        private SKBitmap? _thisBit;
        protected override void SetUpPaints()
        {
            _bitPaint = MiscHelpers.GetBitmapPaint();
            Assembly thisA = Assembly.GetAssembly(GetType());
            _thisBit = ImageExtensions.GetSkBitmap(thisA, "troublegameboard.png");
        }
        private int _diffTop;
        private int _diffLeft;
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            MarblePiecesCP<EnumColorChoice> thisMarble;
            if (_isTest)
            {
                foreach (var thisSpace in _spaceList.Values)
                {
                    string color; //originally 35 and 42.  doing this to double check.
                    color = cs.Black;
                    //if (_spaceList.GetKey(thisSpace) == _thisMod.TempSpace)
                    //    color = cs.Orange;
                    //else
                    //    color = cs.Blue;
                    thisMarble = GetGamePiece(color, thisSpace.Location);
                    thisMarble.DrawImage(canvas);
                }
                return;
            }
            thisMarble = GetGamePiece(_gameContainer.SingleInfo!.Color.ToColor(), new SKPoint(0, 0));
            var tempSize = GetActualSize(70, 70);
            thisMarble.ActualHeight = tempSize.Height;
            thisMarble.ActualWidth = tempSize.Width;
            thisMarble.Location = new SKPoint(0, _actualHeight * .65f);
            thisMarble.DrawImage(canvas);
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PieceList.ForEach(thisPiece =>
                {
                    var thisSpace = _spaceList[thisPiece];
                    thisMarble = GetGamePiece(thisPlayer.Color.ToColor(), thisSpace);
                    thisMarble.DrawImage(canvas);
                });
            });
            if (_gameContainer.Animates.AnimationGoing)
            {
                if (_gameContainer.PlayerGoingBack == 0)
                    throw new BasicBlankException("There is no known player going back");
                var tempPlayer = _gameContainer.PlayerList[_gameContainer.PlayerGoingBack];
                thisMarble = GetGamePiece(tempPlayer.Color.ToColor(), _gameContainer.Animates.CurrentLocation);
                thisMarble.DrawImage(canvas);
                return;
            }
            if (_gameContainer.MovePlayer > 0)
            {
                thisMarble = GetGamePiece(_gameContainer.SingleInfo.Color.ToColor(), _spaceList[_gameContainer.MovePlayer]);
                thisMarble.DrawImage(canvas);
                return;
            }
        }
        private float _actualHeight;
        private float _actualWidth;
        private SKRect _diceRect;
        private float _intSize;
        private void AddRectangle(int spacenumber, float left, float width)
        {
            ResetDiffs(spacenumber);
            var output = SKRect.Create(left + _diffLeft, width + _diffTop, _intSize, _intSize);
            _spaceList.Add(spacenumber, output);
        }
        protected override void CreateSpaces()
        {
            _spaceList.Clear();
            var bounds = GetBounds();
            _actualHeight = bounds.Size.Height;
            _actualWidth = bounds.Size.Width;
            SKPoint pt_Center = new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
            _diceRect = SKRect.Create(pt_Center.X - (bounds.Width / 8), pt_Center.Y - (bounds.Height / 8), bounds.Width / 4, bounds.Height / 4);
            float int_Size;
            int int_Count;
            int_Size = bounds.Width / 20;
            _intSize = int_Size;
            float int_Offset;
            float int_OffsetShort;
            // ********************************
            // *** Draw home spaces
            int_Offset = bounds.Width / 6;
            // *** Blue home
            AddRectangle(1, bounds.Width - int_Offset - int_Size * 2, int_Offset - (int_Size * 2));
            AddRectangle(2, bounds.Width - int_Offset - int_Size, int_Offset - (int_Size));
            AddRectangle(3, bounds.Width - int_Offset, int_Offset);
            AddRectangle(4, bounds.Width - int_Offset + int_Size, int_Offset + int_Size);
            // *** green home
            AddRectangle(5, bounds.Width - int_Offset - int_Size * 2, bounds.Height - int_Offset + int_Size);
            AddRectangle(6, bounds.Width - int_Offset - int_Size, bounds.Height - int_Offset);
            AddRectangle(7, bounds.Width - int_Offset, bounds.Height - int_Offset - int_Size);
            AddRectangle(8, bounds.Width - int_Offset + int_Size, bounds.Height - int_Offset - int_Size * 2);
            // *** red home
            AddRectangle(12, int_Offset + int_Size, bounds.Height - int_Offset + int_Size);
            AddRectangle(11, int_Offset, bounds.Height - int_Offset);
            AddRectangle(10, int_Offset - int_Size, bounds.Height - int_Offset - int_Size);
            AddRectangle(9, int_Offset - (int_Size * 2), bounds.Height - int_Offset - int_Size * 2);
            // *** yellow home
            AddRectangle(13, int_Offset - (int_Size * 2), int_Offset + int_Size);
            AddRectangle(14, int_Offset - int_Size, int_Offset);
            AddRectangle(15, int_Offset, int_Offset - int_Size);
            AddRectangle(16, int_Offset + int_Size, int_Offset - (int_Size * 2));
            // ********************************
            // *** Draw edge spaces
            int_Offset = bounds.Width / 7;
            // *** Bottom
            for (int_Count = -2; int_Count <= 2; int_Count++)
            {
                AddRectangle(20 + int_Count, pt_Center.X - (int_Size / 2) - (int_Count * (int_Size * 1.6f)), bounds.Height - int_Offset);
            }
            // *** left
            for (int_Count = -2; int_Count <= 2; int_Count++)
            {
                AddRectangle(27 + int_Count, int_Offset - int_Size, pt_Center.Y - (int_Size / 2) - (int_Count * (int_Size * 1.6f)));
            }
            // *** top
            for (int_Count = -2; int_Count <= 2; int_Count++)
            {
                AddRectangle(34 + int_Count, pt_Center.X - (int_Size / 2) + (int_Count * (int_Size * 1.6f)), int_Offset - int_Size);
            }
            // *** right
            for (int_Count = -2; int_Count <= 2; int_Count++)
            {
                AddRectangle(41 + int_Count, bounds.Width - int_Offset, pt_Center.Y - (int_Size / 2) + (int_Count * (int_Size * 1.6f)));
            }
            // ********************************
            // *** Draw finish spaces
            int_Offset = bounds.Width * 0.22f;
            // *** Green
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(45 + int_Count, bounds.Width - int_Offset - int_Size - (int_Size * int_Count * 0.9f), bounds.Height - int_Offset - int_Size - (int_Size * int_Count * 0.9f));
            }
            // *** red
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(49 + int_Count, int_Offset + (int_Size * int_Count * 0.9f), bounds.Height - int_Offset - int_Size - (int_Size * int_Count * 0.9f));
            }
            // *** yellow
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(53 + int_Count, int_Offset + (int_Size * int_Count * 0.9f), int_Offset + (int_Size * int_Count * 0.9f));
            }
            // *** blue
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(57 + int_Count, bounds.Width - int_Offset - int_Size - (int_Size * int_Count * 0.9f), int_Offset + (int_Size * int_Count * 0.9f));
            }
            // ********************************
            // *** Draw odd spaces
            int_Offset = bounds.Width * 0.24f;
            int_OffsetShort = bounds.Width * 0.14f;
            AddRectangle(17, bounds.Width - int_Offset - int_Size, bounds.Height - int_OffsetShort - int_Size);
            AddRectangle(23, int_Offset, bounds.Height - int_OffsetShort - int_Size);
            AddRectangle(24, int_OffsetShort, bounds.Height - int_Offset - int_Size);
            AddRectangle(30, int_OffsetShort, int_Offset);
            AddRectangle(31, int_Offset, int_OffsetShort);
            AddRectangle(37, bounds.Width - int_Size - int_Offset, int_OffsetShort);
            AddRectangle(38, bounds.Width - int_Size - int_OffsetShort, int_Offset);
            AddRectangle(44, bounds.Width - int_Size - int_OffsetShort, bounds.Height - int_Size - int_Offset);
            if (_spaceList.Count == 0)
                throw new Exception("Failed to create spacelist");
            PieceHeight = _spaceList[1].Height;
            PieceWidth = _spaceList[1].Width;
            if (_gameContainer.PositionDice == null)
            {
                throw new BasicBlankException("Nobody is handling the positioning of dice.  Rethink");
            }
            _gameContainer.PositionDice.Invoke();
        }
        private void ResetDiffs(int spaceNumber)
        {
            _diffLeft = 0;
            _diffTop = 0;
            if (_gameContainer.BasicData!.IsXamarinForms == false)
                return;
            if (spaceNumber == 0)
                throw new BasicBlankException("Space Number Cannot Be 0");
            switch (spaceNumber)
            {
                case 1:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 4);
                        break;
                    }

                case 2:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 6);
                        break;
                    }

                case 3:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 8);
                        break;
                    }

                case 4:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 10);
                        break;
                    }

                case 5:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -2);
                        break;
                    }

                case 6:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -4);
                        _diffLeft = 1;
                        break;
                    }

                case 7:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -6);
                        _diffLeft = 1;
                        break;
                    }

                case 8:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -8);
                        _diffLeft = 1;
                        break;
                    }

                case 9:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -7);
                        break;
                    }

                case 10:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -5);
                        break;
                    }

                case 11:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -3);
                        break;
                    }

                case 12:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -2);
                        break;
                    }

                case 13:
                    {
                        _diffLeft = 1;
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 10);
                        break;
                    }

                case 14:
                    {
                        _diffLeft = 1;
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 8);
                        break;
                    }

                case 15:
                    {
                        _diffLeft = 1;
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 6);
                        break;
                    }

                case 16:
                    {
                        _diffLeft = 1;
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 4);
                        break;
                    }

                case 17:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -4);
                        break;
                    }

                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 28:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -2);
                        break;
                    }

                case 23:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -6);
                        break;
                    }

                case 24:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -10);
                        break;
                    }

                case 25:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 8);
                        break;
                    }

                case 26:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 4);
                        break;
                    }

                case 29:
                    {
                        _diffLeft = 1;
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -4);
                        break;
                    }

                case 30:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 12);
                        break;
                    }

                case 31:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 10);
                        break;
                    }

                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 8);
                        break;
                    }

                case 37:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 10);
                        break;
                    }

                case 38:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 12);
                        break;
                    }

                case 39:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -2);
                        break;
                    }

                case 40:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -4);
                        break;
                    }

                case 42:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 1);
                        break;
                    }

                case 43:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 2);
                        break;
                    }

                case 44:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -12);
                        break;
                    }

                case 45:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -8);
                        _diffLeft = 1;
                        break;
                    }

                case 46:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -10);
                        _diffLeft = 1;
                        break;
                    }

                case 47:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -11);
                        _diffLeft = 1;
                        break;
                    }

                case 48:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -12);
                        _diffLeft = 1;
                        break;
                    }

                case 49:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -8);
                        _diffLeft = 1;
                        break;
                    }

                case 50:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -10);
                        _diffLeft = 1;
                        break;
                    }

                case 51:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -11);
                        _diffLeft = 1;
                        break;
                    }

                case 52:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, -12);
                        _diffLeft = 1;
                        break;
                    }

                case 53:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 10);
                        _diffLeft = 1;
                        break;
                    }

                case 54:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 12);
                        _diffLeft = 1;
                        break;
                    }

                case 55:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 14);
                        _diffLeft = 1;
                        break;
                    }

                case 56:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 16);
                        _diffLeft = 1;
                        break;
                    }

                case 57:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 10);
                        _diffLeft = 1;
                        break;
                    }

                case 58:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 12);
                        _diffLeft = 1;
                        break;
                    }

                case 59:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 14);
                        _diffLeft = 1;
                        break;
                    }

                case 60:
                    {
                        _diffTop = _pos!.CalculateDiffTopRegular(spaceNumber, 16);
                        _diffLeft = 1;
                        break;
                    }
            }
        }
    }
}