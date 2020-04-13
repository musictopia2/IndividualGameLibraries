using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using SorryCP.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SorryCP.Graphics
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<PawnPiecesCP<EnumColorChoice>>
    {
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(600, 600);
        private readonly Dictionary<int, SKRect> _spaceList = new Dictionary<int, SKRect>();
        private Dictionary<int, SKRect> _homeSpaces = new Dictionary<int, SKRect>();
        private SKRect _rect_Discard = SKRect.Create(0, 0, 1, 1); //hopefully something else happens or will be hosed.
        private SKRect _rect_Deck = SKRect.Create(0, 0, 1, 1);
        private readonly SorryGameContainer _gameContainer;
        public SKPoint LocationOfSpace(int index)
        {
            return _spaceList[index].Location;
        }
        public SKSize CardSize()
        {
            var bounds = GetBounds();
            return new SKSize(50 * (bounds.Size.Width / 390), 105 * (bounds.Size.Height / 390));
        }
        public GameBoardGraphicsCP(SorryGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
            DrawBoardEarly = true;
        }
        public override void DrawGraphicsForBoard(SKCanvas canvas, float width, float height)
        {
            SetUpPaints();
            var thisRect = GetBounds();
            canvas.DrawBitmap(_thisBit, thisRect, _bitPaint);
            base.DrawGraphicsForBoard(canvas, width, height);
        }
        protected override PawnPiecesCP<EnumColorChoice> GetGamePiece(string color, SKPoint location)
        {
            var output = base.GetGamePiece(color, location);
            output.NeedsToClear = false;
            return output;
        }
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override void CreateSpaces()
        {
            var bounds = GetBounds();
            int int_Count;
            SKRect rect;
            SKPoint pt_Center = new SKPoint(bounds.Left + (bounds.Width / 2), bounds.Top + (bounds.Height / 2));
            SKMatrix tmp_Matrix = new SKMatrix();
            SKPath gp;
            SKRect obj_Space;
            _homeSpaces = new Dictionary<int, SKRect>();
            _rect_Deck = SKRect.Create(pt_Center.X - this.CardSize().Width - (this.CardSize().Width / 10), pt_Center.Y - (this.CardSize().Height / 2), this.CardSize().Width, this.CardSize().Height);
            _rect_Discard = SKRect.Create(pt_Center.X + (this.CardSize().Width / 10), pt_Center.Y - (this.CardSize().Height / 2), this.CardSize().Width, this.CardSize().Height);
            SKMatrix.RotateDegrees(ref tmp_Matrix, 90, pt_Center.X, pt_Center.Y);
            int spaceNumber;
            // *** Draw starts
            // *** Draw circles
            gp = new SKPath();
            var thisRect = SKRect.Create(bounds.Left + ((bounds.Width * 41) / 64), bounds.Top + ((bounds.Height * 95) / 200) + ((bounds.Height * 5) / 16), (bounds.Width * 5) / 32, (bounds.Height * 5) / 32);
            gp.AddRect(thisRect);
            obj_Space = SKRect.Create(thisRect.Left, thisRect.Top, thisRect.Width / 2, thisRect.Height / 2);
            spaceNumber = 1;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 2;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 3;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 4;
            _spaceList.Add(spaceNumber, obj_Space);
            gp.Transform(tmp_Matrix);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 5;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 6;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 7;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 8;
            _spaceList.Add(spaceNumber, obj_Space);
            gp.Transform(tmp_Matrix);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 9;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 10;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 11;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 12;
            _spaceList.Add(spaceNumber, obj_Space);
            gp.Transform(tmp_Matrix);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 13;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top, gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 14;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 15;
            _spaceList.Add(spaceNumber, obj_Space);
            obj_Space = SKRect.Create(gp.Bounds.Left + (gp.Bounds.Width / 2), gp.Bounds.Top + (gp.Bounds.Height / 2), gp.Bounds.Width / 2, gp.Bounds.Height / 2);
            spaceNumber = 16;
            _spaceList.Add(spaceNumber, obj_Space);
            gp.Dispose();
            // *********************************************
            // *** Draw Homes
            // *** Draw circles
            gp = new SKPath();
            gp.AddRect(SKRect.Create(bounds.Left + ((bounds.Width * 49) / 64), bounds.Top + ((bounds.Height * 95) / 200), (bounds.Width * 5) / 32, (bounds.Height * 5) / 32));
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width, gp.Bounds.Height);
            spaceNumber = 1;
            _homeSpaces.Add(spaceNumber, obj_Space);
            gp.Transform(tmp_Matrix);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width, gp.Bounds.Height);
            spaceNumber = 2;
            _homeSpaces.Add(spaceNumber, obj_Space);
            gp.Transform(tmp_Matrix);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width, gp.Bounds.Height);
            spaceNumber = 3;
            _homeSpaces.Add(spaceNumber, obj_Space);
            gp.Transform(tmp_Matrix);
            obj_Space = SKRect.Create(gp.Bounds.Left, gp.Bounds.Top, gp.Bounds.Width, gp.Bounds.Height);
            spaceNumber = 4;
            _homeSpaces.Add(spaceNumber, obj_Space);
            for (int_Count = 10; int_Count <= 14; int_Count++)
            {
                // *** Blue
                rect = SKRect.Create(bounds.Left + ((bounds.Width * 13) / 16), bounds.Top + ((bounds.Height * int_Count) / 16), bounds.Width / 16, ((((bounds.Height * (int_Count + 1)) / 16)) - (((bounds.Height * int_Count) / 16))) + 1);
                obj_Space = rect;
                spaceNumber = (77 + 14) - int_Count;
                _spaceList.Add(spaceNumber, obj_Space);
                // *** yellow
                rect = SKRect.Create(bounds.Left + ((bounds.Width * (int_Count - 9)) / 16), bounds.Top + ((bounds.Height * 13) / 16), ((((bounds.Width * (int_Count + 1)) / 16)) - (((bounds.Width * int_Count) / 16))) + 1, bounds.Height / 16);
                obj_Space = rect;
                spaceNumber = 72 + int_Count;
                _spaceList.Add(spaceNumber, obj_Space);
                // *** green
                rect = SKRect.Create(bounds.Left + ((bounds.Width * 2) / 16), bounds.Top + ((bounds.Height * (int_Count - 9)) / 16), bounds.Width / 16, ((((bounds.Height * (int_Count + 1)) / 16)) - (((bounds.Height * int_Count) / 16))) + 1);
                obj_Space = rect;
                spaceNumber = 77 + int_Count;
                _spaceList.Add(spaceNumber, obj_Space);
                // *** red
                rect = SKRect.Create(bounds.Left + ((bounds.Width * int_Count) / 16), bounds.Top + ((bounds.Height * 2) / 16), ((((bounds.Width * (int_Count + 1)) / 16)) - (((bounds.Width * int_Count) / 16))) + 1, bounds.Height / 16);
                obj_Space = rect;
                spaceNumber = (92 + 14) - int_Count;
                _spaceList.Add(spaceNumber, obj_Space);
            }
            for (int_Count = 0; int_Count <= 15; int_Count++)
            {
                if ((int_Count > 0) & (int_Count < 15))
                {
                    // *** Top 
                    rect = SKRect.Create(bounds.Left + ((bounds.Width * int_Count) / 16), bounds.Top, ((((bounds.Width * (int_Count + 1)) / 16)) - (((bounds.Width * int_Count) / 16))) + 1, bounds.Height / 16);
                    obj_Space = rect;
                    spaceNumber = 43 + int_Count;
                    _spaceList.Add(spaceNumber, obj_Space);
                    // *** Bottom
                    rect = SKRect.Create(bounds.Left + ((bounds.Width * int_Count) / 16), bounds.Top + ((bounds.Height * 15) / 16), ((((bounds.Width * (int_Count + 1)) / 16)) - (((bounds.Width * int_Count) / 16))) + 1, bounds.Height / 16);
                    obj_Space = rect;
                    if ((28 - int_Count) < 17)
                        spaceNumber = 88 - int_Count;
                    else
                        spaceNumber = 28 - int_Count;
                    _spaceList.Add(spaceNumber, obj_Space);
                    // *** Left
                    rect = SKRect.Create(bounds.Left, bounds.Top + ((bounds.Height * int_Count) / 16), bounds.Width / 16, ((((bounds.Height * (int_Count + 1)) / 16)) - (((bounds.Height * int_Count) / 16))) + 1);
                    obj_Space = rect;
                    spaceNumber = 43 - int_Count;
                    _spaceList.Add(spaceNumber, obj_Space);
                    // *** Right
                    rect = SKRect.Create(bounds.Left + ((bounds.Width * 15) / 16), bounds.Top + ((bounds.Height * int_Count) / 16), bounds.Width / 16, ((((bounds.Height * (int_Count + 1)) / 16)) - (((bounds.Height * int_Count) / 16))) + 1);
                    obj_Space = rect;
                    spaceNumber = 58 + int_Count;
                    _spaceList.Add(spaceNumber, obj_Space);
                }
                else
                {
                    // *** Top 
                    rect = SKRect.Create(bounds.Left + ((bounds.Width * int_Count) / 16), bounds.Top, (((bounds.Width * (int_Count + 1)) / 16)) - (((bounds.Width * int_Count) / 16)), bounds.Height / 16);
                    obj_Space = rect;
                    spaceNumber = 43 + int_Count;
                    _spaceList.Add(spaceNumber, obj_Space);
                    // *** Bottom
                    rect = SKRect.Create(bounds.Left + ((bounds.Width * int_Count) / 16), bounds.Top + ((bounds.Height * 15) / 16), (((bounds.Width * (int_Count + 1)) / 16)) - (((bounds.Width * int_Count) / 16)), bounds.Height / 16);
                    obj_Space = rect;
                    if ((28 - int_Count) < 17)
                        spaceNumber = 88 - int_Count;
                    else
                        spaceNumber = 28 - int_Count;
                    _spaceList.Add(spaceNumber, obj_Space);
                }
            }
            if (_spaceList.Count == 0)
                throw new BasicBlankException("No spaces was created");
            if (_homeSpaces.Count != 4)
                throw new BasicBlankException("Should be 4 home spaces, not " + _homeSpaces.Count.ToString());
            PieceHeight = bounds.Height / 17f;
            PieceWidth = bounds.Width / 17f;
            gp.Dispose();
        }
        private SKPaint? _bitPaint;
        private SKBitmap? _thisBit;
        private SKPaint? _highlightPaint;
        protected override void SetUpPaints()
        {
            _bitPaint = MiscHelpers.GetBitmapPaint();
            Assembly thisA = Assembly.GetAssembly(GetType());
            _thisBit = ImageExtensions.GetSkBitmap(thisA, "SorryGameBoard.png");
            _highlightPaint = MiscHelpers.GetStrokePaint(SKColors.Fuchsia, 3);
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_gameContainer.Command.IsExecuting)
            {
                return;
            }
            if (_gameContainer.SaveRoot.DidDraw == false)
            {
                if (MiscHelpers.DidClickRectangle(_rect_Deck, thisPoint))
                {
                    if (_gameContainer.DrawClickAsync == null)
                    {
                        throw new BasicBlankException("Nobody is handling the draw click.  Rethink");
                    }
                    await _gameContainer.ProcessCustomCommandAsync(_gameContainer.DrawClickAsync);
                    return;
                }
            }
            if (_gameContainer.SaveRoot.DidDraw == false)
            {
                return;
            }
            var thisRect = _homeSpaces[(int)_gameContainer.SingleInfo!.Color];
            if (MiscHelpers.DidClickRectangle(thisRect, thisPoint))
            {
                if (_gameContainer.HomeClickedAsync == null)
                {
                    throw new BasicBlankException("Nobody is handling the home click.  Rethink");
                }
                await _gameContainer.ProcessCustomCommandAsync(_gameContainer.HomeClickedAsync, _gameContainer.SingleInfo.Color);
                return;
            }
            if (_gameContainer.SpaceClickedAsync == null)
            {
                throw new BasicBlankException("Nobody is handling the space click.  Rethink");
            }

            foreach (var thisSpace in _spaceList.Values)
            {
                SKRect tempRect;
                tempRect = thisSpace;
                if (MiscHelpers.DidClickRectangle(tempRect, thisPoint))
                {
                    int index = _spaceList.GetKey(thisSpace);
                    await _gameContainer.ProcessCustomCommandAsync(_gameContainer.SpaceClickedAsync, index);
                    return; //i think something else will worry about whether its valid.
                }
            }
        }
        private CustomBasicList<SKRect> GetFourHomeRect(SKRect thisHome)
        {
            var firstRect = SKRect.Create(thisHome.Location.X, thisHome.Location.Y, thisHome.Width / 4, thisHome.Height / 4);
            var secondRect = SKRect.Create(thisHome.Location.X + (thisHome.Width / 2), thisHome.Location.Y, thisHome.Width / 4, thisHome.Height / 4);
            var thirdRect = SKRect.Create(thisHome.Location.X, thisHome.Location.Y + (thisHome.Height / 2), thisHome.Width / 4, thisHome.Height / 4);
            var fourthRect = SKRect.Create(thisHome.Location.X + (thisHome.Width / 2), thisHome.Location.Y + (thisHome.Height / 2), thisHome.Width / 4, thisHome.Height / 4);
            return new CustomBasicList<SKRect>() { firstRect, secondRect, thirdRect, fourthRect };
        }
        private void DrawHomePieces(SKCanvas canvas, SorryPlayerItem thisPlayer)
        {
            var thisRect = _homeSpaces[(int)thisPlayer.Color];
            var tempList = GetFourHomeRect(thisRect);
            if (tempList.Count != 4)
                throw new BasicBlankException("There can only be 4 spaces for home");
            int x;
            var loopTo = thisPlayer.HowManyHomePieces;
            for (x = 1; x <= loopTo; x++)
            {
                thisRect = tempList[x - 1]; // becauase 0 based
                var thisPawn = GetGamePiece(thisPlayer.Color.ToColor(), thisRect.Location);
                thisPawn.DrawImage(canvas);
            }
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();

            var tempLocation = new SKPoint(_rect_Discard.Left, _rect_Discard.Bottom + 5);
            var thisPawn = GetGamePiece(_gameContainer.SingleInfo!.Color.ToColor(), tempLocation);
            var tempSize = GetActualSize(70, 70);
            thisPawn.ActualHeight = tempSize.Height;
            thisPawn.ActualWidth = tempSize.Width;
            thisPawn.DrawImage(canvas);
            if (_gameContainer.SaveRoot!.DidDraw)
            {
                CardGraphicsCP thisGraphics = new CardGraphicsCP();
                var thisLocation = _rect_Discard.Location;
                var thisSize = _rect_Discard.Size;
                thisGraphics.Init(); //i think
                if (thisGraphics.MainGraphics == null)
                    throw new BasicBlankException("Did not create graphics.  Rethink");
                thisGraphics.MainGraphics.ActualHeight = thisSize.Height;
                thisGraphics.MainGraphics.ActualWidth = thisSize.Width;
                thisGraphics.MainGraphics.Location = thisLocation;
                thisGraphics.MainGraphics.DrawImage(canvas);
            }
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PieceList.ForEach(thisPiece =>
                {
                    var thisSpace = _spaceList[thisPiece];
                    thisPawn = GetGamePiece(thisPlayer.Color.ToColor(), thisSpace);
                    thisPawn.DrawImage(canvas);
                });
                if (thisPlayer.HowManyHomePieces > 0)
                    DrawHomePieces(canvas, thisPlayer);
            });
            if (_gameContainer.SaveRoot.SpacesLeft > 0)
            {
                var textRect = SKRect.Create(_rect_Discard.Location.X, _rect_Discard.Top - (_rect_Discard.Height / 2), _rect_Discard.Width, _rect_Discard.Height / 2);
                var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, _rect_Discard.Height / 2);
                canvas.DrawCustomText(_gameContainer.SaveRoot.SpacesLeft.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, textRect, out _);
            }
            if (_gameContainer.Animates.AnimationGoing)
            {
                if (_gameContainer.PlayerGoingBack == 0)
                    throw new BasicBlankException("There is no known player going back");
                var tempPlayer = _gameContainer.PlayerList[_gameContainer.PlayerGoingBack];
                thisPawn = GetGamePiece(tempPlayer.Color.ToColor(), _gameContainer.Animates.CurrentLocation);
                thisPawn.DrawImage(canvas);
                return;
            }
            if (_gameContainer.MovePlayer > 0)
            {
                if (_gameContainer.MovePlayer < 100)
                {
                    thisPawn = GetGamePiece(_gameContainer.SingleInfo.Color.ToColor(), _spaceList[_gameContainer.MovePlayer]);
                }
                else
                {
                    var tempRect = _homeSpaces[(int)_gameContainer.SaveRoot.OurColor];
                    thisPawn = GetGamePiece(_gameContainer.SingleInfo.Color.ToColor(), tempRect);
                }
                thisPawn.DrawImage(canvas);
                return;
            }
            _gameContainer.SaveRoot.HighlightList.ForEach(thisItem =>
            {
                SKRect tempRect;
                if (thisItem < 100)
                    tempRect = _spaceList[thisItem];
                else
                    tempRect = _homeSpaces[(int)_gameContainer.SaveRoot.OurColor];
                canvas.DrawRect(tempRect, _highlightPaint);
            });
        }
    }
}
