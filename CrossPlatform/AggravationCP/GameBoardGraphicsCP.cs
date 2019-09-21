using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Reflection;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace AggravationCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : BaseGameBoardCP<MarblePiecesCP<EnumColorChoice>>
    {
        private readonly GlobalVariableClass _thisGlobal;
        private readonly AggravationViewModel _thisMod;
        private readonly AggravationMainGameClass _mainGame;
        private readonly IBoardPosition? _pos;
        public GameBoardGraphicsCP(IGamePackageResolver mainContainer) : base(mainContainer)
        {
            _thisGlobal = mainContainer.Resolve<GlobalVariableClass>(); //i think.
            _thisMod = mainContainer.Resolve<AggravationViewModel>();
            _mainGame = mainContainer.Resolve<AggravationMainGameClass>();
            if (_mainGame.ThisData!.IsXamarinForms)
                _pos = mainContainer.Resolve<IBoardPosition>();
            DrawBoardEarly = true;
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(600, 600);
        protected override bool CanStartPaint()
        {
            return _mainGame.DidChooseColors;
        }
        protected override MarblePiecesCP<EnumColorChoice> GetGamePiece(string color, SKPoint location)
        {
            var output = base.GetGamePiece(color, location);
            output.NeedsToClear = false;
            return output;
        }
        public SKPoint LocationOfSpace(int index)
        {
            return _spaceList[index].Location;
        }
        private readonly bool _isTest = false; //for testing,  i can click anywhere.

        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (_isTest == false)
            {
                if (_thisMod!.SpaceCommand!.CanExecute(0) == false)
                    return; //can't consider because it can't execute anyways.
            }
            foreach (var thisSpace in _spaceList.Values)
            {
                SKRect tempRect;
                if (_mainGame.ThisData!.IsXamarinForms == false || _isTest)
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
                    if (_isTest)
                    {
                        _thisMod.TempSpace = index;
                        _thisMod.GameBoard1!.Repaint();
                        return;
                    }
                    if (_thisMod.GameBoard1!.IsValidMove(index))
                    {
                        _thisMod.SpaceCommand!.Execute(index);
                        return;
                    }
                }
            }
        }
        private readonly Dictionary<int, SKRect> _spaceList = new Dictionary<int, SKRect>(); //try this way.
        private SKPaint? _bitPaint;
        private SKBitmap? _thisBit;
        private SKPaint? _limeGreenBorder;
        protected override void SetUpPaints()
        {
            _bitPaint = MiscHelpers.GetBitmapPaint();
            _limeGreenBorder = MiscHelpers.GetStrokePaint(SKColors.LimeGreen, 3);
            Assembly thisA = Assembly.GetAssembly(GetType());
            _thisBit = ImageExtensions.GetSkBitmap(thisA, "aggravationgameboard.png");
        }
        public override void DrawGraphicsForBoard(SKCanvas canvas, float width, float height)
        {
            SetUpPaints();
            var thisRect = GetBounds();
            canvas.DrawBitmap(_thisBit, thisRect, _bitPaint);
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
                    if (_spaceList.GetKey(thisSpace) == 5 || _spaceList.GetKey(thisSpace) == 5)
                        color = cs.Green;
                    else if (_spaceList.GetKey(thisSpace) == _thisMod.TempSpace)
                        color = cs.Orange;
                    else
                        color = cs.Blue;
                    thisMarble = GetGamePiece(color, thisSpace.Location);
                    thisMarble.DrawImage(canvas);
                }
                return;
            }
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PieceList.ForEach(thisPiece =>
                {
                    var thisSpace = _spaceList[thisPiece];
                    thisMarble = GetGamePiece(thisPlayer.Color.ToColor(), thisSpace);
                    thisMarble.DrawImage(canvas);
                });
            });
            if (_thisGlobal.Animates.AnimationGoing)
            {
                if (_thisGlobal.PlayerGoingBack == 0)
                    throw new BasicBlankException("There is no known player going back");
                var tempPlayer = _mainGame.PlayerList[_thisGlobal.PlayerGoingBack];
                thisMarble = GetGamePiece(tempPlayer.Color.ToColor(), _thisGlobal.Animates.CurrentLocation);
                thisMarble.DrawImage(canvas);
                return;
            }
            if (_thisGlobal.MovePlayer > 0)
            {
                thisMarble = GetGamePiece(_mainGame.SingleInfo!.Color.ToColor(), _spaceList[_thisGlobal.MovePlayer]);
                thisMarble.DrawImage(canvas);
                return;
            }
            if (_mainGame.SaveRoot!.PreviousSpace == 0)
                return;
            var newRect = _spaceList[_mainGame.SaveRoot.PreviousSpace];
            canvas.DrawRect(newRect, _limeGreenBorder);
        }
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
            SKPoint pt_Center = new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
            float int_CenterOffset;
            int int_Count;
            int_CenterOffset = bounds.Width / 15;
            _intSize = bounds.Width / 30;
            float int_Size = _intSize; //for compatibility.
            AddRectangle(9, pt_Center.X  - int_Size * 4, bounds.Top + (bounds.Width / 100));
            AddRectangle(10, pt_Center.X - int_Size * 1.75f, bounds.Top + (bounds.Width / 100));
            AddRectangle(11, pt_Center.X + int_Size * 0.75f, bounds.Top + (bounds.Width / 100));
            AddRectangle(12, pt_Center.X + int_Size * 3, bounds.Top + (bounds.Width / 100));
            float Pointx;
            Pointx = bounds.Width - int_Size - (bounds.Width / 100);
            Pointx += bounds.Left; // i think
            AddRectangle(13, Pointx, (pt_Center.Y) - int_Size * 4);
            AddRectangle(14, Pointx, pt_Center.Y - int_Size * 1.75f);
            AddRectangle(15, Pointx, pt_Center.Y + int_Size * 0.75f);
            AddRectangle(16, Pointx, pt_Center.Y + int_Size * 3);
            AddRectangle(4, pt_Center.X - int_Size * 4, bounds.Height - int_Size - (bounds.Width / 100));
            AddRectangle(3, pt_Center.X - int_Size * 1.75f, bounds.Height - int_Size - (bounds.Width / 100));
            AddRectangle(2, pt_Center.X + int_Size * 0.75f, bounds.Height - int_Size - (bounds.Width / 100));
            AddRectangle(1, pt_Center.X + int_Size * 3, bounds.Height + -int_Size - (bounds.Width / 100));
            AddRectangle(8, bounds.Left + (bounds.Width / 100), pt_Center.Y - int_Size * 4);
            AddRectangle(7, bounds.Left + (bounds.Width / 100), pt_Center.Y - int_Size * 1.75f);
            AddRectangle(6, bounds.Left + (bounds.Width / 100), pt_Center.Y + int_Size * 0.75f);
            AddRectangle(5, bounds.Left + (bounds.Width / 100), pt_Center.Y + int_Size * 3);
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(68 - int_Count, pt_Center.X - (int_Size / 2), pt_Center.Y + (int_CenterOffset + (int_Size)) + (int_Count * (int_Size * 2)));
            }
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(72 - int_Count, pt_Center.X - (int_CenterOffset + (int_Size * 2)) - (int_Count * (int_Size * 2)), pt_Center.Y - (int_Size / 2));
            }
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(76 - int_Count, pt_Center.X - (int_Size / 2), pt_Center.Y - (int_CenterOffset + (int_Size * 2)) - (int_Count * (int_Size * 2)));
            }
            for (int_Count = 0; int_Count <= 3; int_Count++)
            {
                AddRectangle(80 - int_Count, pt_Center.X  + (int_CenterOffset + (int_Size)) + (int_Count * (int_Size * 2)), pt_Center.Y - (int_Size / 2));
            }
            // ************************************************************
            // *** Center space
            AddRectangle(81, pt_Center.X - (int_Size / 2), pt_Center.Y - (int_Size / 2));
            // ************************************************************
            // *** Normal spaces
            for (int_Count = 1; int_Count <= 5; int_Count++)
            {
                AddRectangle(58 + int_Count, pt_Center.X + int_CenterOffset, pt_Center.Y + int_CenterOffset + int_Count * (int_Size * 2));
            }
            AddRectangle(64, pt_Center.X  - (int_Size / 2), pt_Center.Y + int_CenterOffset + (5 * (int_Size * 2)));
            for (int_Count = 0; int_Count <= 5; int_Count++)
            {
                AddRectangle(22 - int_Count, pt_Center.X  - int_CenterOffset - int_Size, pt_Center.Y + int_CenterOffset + int_Count * (int_Size * 2));
            }
            for (int_Count = 1; int_Count <= 5; int_Count++)
            {
                AddRectangle(22 + int_Count, pt_Center.X - int_Size - int_CenterOffset - int_Count * (int_Size * 2), pt_Center.Y + int_CenterOffset);
            }
            AddRectangle(28, pt_Center.X - int_Size - int_CenterOffset - (5 * (int_Size * 2)), pt_Center.Y - (int_Size / 2));
            for (int_Count = 0; int_Count <= 5; int_Count++)
            {
                AddRectangle(34 - int_Count,  pt_Center.X - int_Size - int_CenterOffset - int_Count * (int_Size * 2), pt_Center.Y - int_CenterOffset - int_Size);
            }
            for (int_Count = 1; int_Count <= 5; int_Count++)
            {
                AddRectangle(34 + int_Count, pt_Center.X  - int_CenterOffset - int_Size, pt_Center.Y - int_Size - int_CenterOffset - int_Count * (int_Size * 2));
            }
            AddRectangle(40, pt_Center.X - (int_Size / 2), pt_Center.Y - int_Size - int_CenterOffset - (5 * (int_Size * 2)));
            for (int_Count = 0; int_Count <= 5; int_Count++)
            {
                AddRectangle(46 - int_Count, pt_Center.X  + int_CenterOffset, pt_Center.Y  - int_Size - int_CenterOffset - int_Count * (int_Size * 2));
            }
            for (int_Count = 1; int_Count <= 5; int_Count++)
            {
                AddRectangle(46 + int_Count, pt_Center.X  + int_CenterOffset + int_Count * (int_Size * 2), pt_Center.Y - int_CenterOffset - int_Size);
            }
            AddRectangle(52, pt_Center.X + int_CenterOffset + (5 * (int_Size * 2)), pt_Center.Y - (int_Size / 2));
            for (int_Count = 0; int_Count <= 5; int_Count++)
            {
                AddRectangle(58 - int_Count, pt_Center.X + int_CenterOffset + int_Count * (int_Size * 2), pt_Center.Y  + int_CenterOffset);
            }
            if (_spaceList.Count == 0)
                throw new BasicBlankException("Failed to create _spaceList");
            PieceHeight = _spaceList[1].Height;
            PieceWidth = _spaceList[1].Width;
        }
        private int GetDiffLeft(int howMany)
        {
            return howMany;
        }
        private void ResetDiffs(int spaceNumber)
        {
            _diffLeft = 0;
            _diffTop = 0;
            if (_mainGame.ThisData!.IsXamarinForms == false)
                return;
            if (spaceNumber == 0)
                throw new BasicBlankException("Space Number Cannot Be 0");
            if (_pos == null)
                throw new BasicBlankException("Never created the position interface needed.  Rethink");
            switch (spaceNumber)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    _diffTop = _pos.GetDiffTopEdges(spaceNumber);
                    break;
                case 65:
                    {
                        _diffTop = _pos.GetDiffTopHome(4);
                        break;
                    }

                case 66:
                    {
                        _diffTop = _pos.GetDiffTopHome(3);
                        break;
                    }

                case 67:
                    {
                        _diffTop = _pos.GetDiffTopHome(2);
                        break;
                    }

                case 68:
                    {
                        _diffTop = _pos.GetDiffTopHome(1); //least means least increase.
                        break;
                    }

                case 73:
                    {
                        _diffTop = _pos.GetDiffTopHome(-4);
                        break;
                    }

                case 74:
                    {
                        _diffTop = _pos.GetDiffTopHome(-3);
                        break;
                    }

                case 75:
                    {
                        _diffTop = _pos.GetDiffTopHome(-2);
                        break;
                    }

                case 76:
                    {
                        _diffTop = _pos.GetDiffTopHome(-1);
                        break;
                    }
                case 69:
                    {
                        _diffLeft = GetDiffLeft(2);
                        break;
                    }

                case 70:
                    {
                        _diffLeft = GetDiffLeft(1);
                        break;
                    }

                case 78:
                    {
                        _diffLeft = GetDiffLeft(-1);
                        break;
                    }

                case 77:
                    {
                        _diffLeft = GetDiffLeft(-1);
                        break;
                    }
                case 28:
                    {
                        _diffLeft = GetDiffLeft(1);
                        break;
                    }

                case 52:
                    {
                        _diffLeft = GetDiffLeft(-1);
                        break;
                    }

                case 29:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(1);
                        break;
                    }

                case 30:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(2);
                        break;
                    }

                case 31:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(1);
                        break;
                    }

                case 32:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(1);
                        break;
                    }

                case 33:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        break;
                    }

                case 34:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        break;
                    }

                case 46:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        break;
                    }

                case 47:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        break;
                    }

                case 48:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(-1);
                        break;
                    }

                case 49:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(-2);
                        break;
                    }

                case 50:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(-2);
                        break;
                    }

                case 51:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-1);
                        _diffLeft = GetDiffLeft(-1);
                        break;
                    }

                case 27:
                    {
                        _diffLeft = GetDiffLeft(1);
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        break;
                    }

                case 26:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        _diffLeft = GetDiffLeft(2);
                        break;
                    }

                case 25:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        _diffLeft = GetDiffLeft(2);
                        break;
                    }

                case 24:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        _diffLeft = GetDiffLeft(1);
                        break;
                    }

                case 23:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        break;
                    }

                case 22:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        break;
                    }

                case 58:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        break;
                    }

                case 57:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        break;
                    }

                case 56:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        _diffLeft = GetDiffLeft(-1);
                        break;
                    }

                case 55:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        _diffLeft = GetDiffLeft(-2);
                        break;
                    }

                case 54:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        _diffLeft = GetDiffLeft(-2);
                        break;
                    }

                case 53:
                    {
                        _diffLeft = GetDiffLeft(-1);
                        _diffTop = _pos.GetDiffTopMiddle(1);
                        break;
                    }

                case 21:
                case 59:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(2);
                        break;
                    }
                case 20:
                case 60:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(3);
                        break;
                    }
                case 19:
                case 61:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(4);
                        break;
                    }
                case 18:
                case 62:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(5);
                        break;
                    }
                case 17:
                case 63:
                case 64:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(6);
                        break;
                    }
                case 35:
                case 45:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-2);
                        break;
                    }
                case 36:
                case 44:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-3);
                        break;
                    }
                case 37:
                case 43:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-4);
                        break;
                    }
                case 38:
                case 42:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-5);
                        break;
                    }
                case 39:
                case 40:
                case 41:
                    {
                        _diffTop = _pos.GetDiffTopMiddle(-6);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}