using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using ConnectTheDotsCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectTheDotsCP.Graphics
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerPiecesCP>
    {

        public GameBoardGraphicsCP(ConnectTheDotsGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(480, 480); // can adjust as needed
        private float _squareHeight;
        private float _spaceHeight;
        SKRect _whiteRect;
        internal static void InitBoard(ConnectTheDotsGameContainer gameContainer)
        {
            InitDots(gameContainer);
            InitSquares(gameContainer);
            InitLines(gameContainer);
        }
        private void LoadDots() //this loads in the dot and bounds.
        {

            if (_gameContainer.DotList!.Count == 0)
                throw new BasicBlankException("Should have initialized dots first.  Rethink");
            int z = 0;
            var dotSize = GetActualSize(10, 10);
            float currentHiddenLeft;
            float currentHiddenTop;
            float diffs = _spaceHeight - _squareHeight;
            float currentTop;
            float currentLeft;
            var bigHeight = GetActualSize(40, 40);
            currentHiddenTop = diffs + _whiteRect.Location.Y - diffs;
            currentTop = diffs + _whiteRect.Location.Y;
            for (int x = 1; x <= 8; x++)
            {
                currentHiddenLeft = _whiteRect.Location.X - diffs;
                currentLeft = diffs + _whiteRect.Location.X;
                for (int y = 1; y <= 8; y++)
                {
                    z++;
                    DotInfo thisDot = _gameContainer.DotList[z];

                    thisDot.Dot = SKRect.Create(currentLeft, currentTop, dotSize.Width, dotSize.Height);
                    thisDot.Bounds = SKRect.Create(currentLeft - (bigHeight.Width / 3), currentTop - (bigHeight.Height / 3), bigHeight.Width, bigHeight.Height);
                    currentLeft += _spaceHeight;
                    currentHiddenLeft += _spaceHeight;
                }
                currentTop += _spaceHeight;
                currentHiddenTop += _spaceHeight;
            }
        }
        private static void InitDots(ConnectTheDotsGameContainer gameContainer)
        {
            gameContainer.DotList = new System.Collections.Generic.Dictionary<int, DotInfo>();
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    DotInfo thisDot = new DotInfo();
                    thisDot.Row = x;
                    thisDot.Column = y;
                    gameContainer.DotList.Add(gameContainer.DotList.Count + 1, thisDot);
                }
            }
        }
        private void LoadSquares()
        {
            if (_gameContainer.SquareList!.Count == 0)
                throw new BasicBlankException("Should have loaded the squares first.  Rethink");
            int z = 0;
            float currentLeft;
            float currentTop;
            var startSize = GetActualSize(15, 15);
            currentTop = startSize.Height + _whiteRect.Location.Y;
            for (int x = 1; x <= 7; x++)
            {
                currentLeft = startSize.Width + _whiteRect.Location.X;
                for (int y = 1; y <= 7; y++)
                {
                    z++;
                    SquareInfo thisSquare = _gameContainer.SquareList[z];
                    thisSquare.Rectangle = SKRect.Create(currentLeft, currentTop, _squareHeight, _squareHeight);
                    currentLeft += _spaceHeight;
                }
                currentTop += _spaceHeight;
            }
        }
        private static void InitSquares(ConnectTheDotsGameContainer gameContainer)
        {
            gameContainer.SquareList = new System.Collections.Generic.Dictionary<int, SquareInfo>();
            for (int x = 1; x <= 7; x++)
            {
                for (int y = 1; y <= 7; y++)
                {
                    SquareInfo thisSquare = new SquareInfo();
                    thisSquare.Row = x;
                    thisSquare.Column = y;
                    thisSquare.Color = 0; //i think.
                    gameContainer.SquareList.Add(gameContainer.SquareList.Count + 1, thisSquare);
                }
            }
        }
        private void LoadLines()
        {
            if (_gameContainer.LineList!.Count == 0)
                throw new BasicBlankException("Should have loaded the lines.  Rethink");
            int z = 0;
            LineInfo thisLine;
            float oldTop;
            float oldLeft;
            var lineSize = GetActualSize(10, 10);
            float newTop;
            oldTop = lineSize.Width + _whiteRect.Location.Y;
            for (int x = 1; x <= 8; x++)
            {
                oldLeft = lineSize.Width + _whiteRect.Location.X;
                for (int y = 1; y <= 7; y++)
                {
                    z++;
                    thisLine = _gameContainer.LineList[z];
                    thisLine.StartingPoint = new SKPoint(oldLeft, oldTop);
                    oldLeft += _spaceHeight;
                    thisLine.FinishingPoint = new SKPoint(oldLeft, oldTop);
                }
                oldLeft = lineSize.Width + _whiteRect.Location.X;
                if (x != 8)
                {
                    for (int y = 1; y <= 8; y++)
                    {
                        z++;
                        thisLine = _gameContainer.LineList[z];
                        thisLine.StartingPoint = new SKPoint(oldLeft, oldTop);
                        newTop = oldTop + _spaceHeight;
                        thisLine.FinishingPoint = new SKPoint(oldLeft, newTop);
                        oldLeft += _spaceHeight;
                    }
                }
                oldTop += _spaceHeight;
            }
        }
        private static void InitLines(ConnectTheDotsGameContainer gameContainer)
        {
            gameContainer.LineList = new System.Collections.Generic.Dictionary<int, LineInfo>();
            LineInfo thisLine;
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 7; y++)
                {
                    thisLine = new LineInfo();
                    thisLine.Horizontal = true;
                    thisLine.Row = x;
                    thisLine.Column = y;
                    thisLine.DotRow1 = x;
                    thisLine.DotColumn1 = y;
                    thisLine.DotRow2 = x;
                    thisLine.DotColumn2 = y + 1;
                    thisLine.Index = gameContainer.LineList.Count + 1;
                    gameContainer.LineList.Add(thisLine.Index, thisLine);
                }
                if (x != 8)
                {
                    for (int y = 1; y <= 8; y++)
                    {
                        thisLine = new LineInfo();
                        thisLine.Horizontal = false;
                        thisLine.Row = x;
                        thisLine.Column = y;
                        thisLine.DotRow1 = x;
                        thisLine.DotColumn1 = y;
                        thisLine.DotRow2 = x + 1;
                        thisLine.DotColumn2 = y;
                        thisLine.Index = gameContainer.LineList.Count + 1;
                        gameContainer.LineList.Add(thisLine.Index, thisLine);
                    }
                }
            }
        }
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (_gameContainer.Command.IsExecuting)
            {
                return;
            }

            if (_gameContainer.MakeMoveAsync == null)
            {
                throw new BasicBlankException("MakeMoveAsync not populated.  Rethink");
            }

            foreach (var thisDot in _gameContainer.DotList!.Values)
            {
                if (MiscHelpers.DidClickRectangle(thisDot.Bounds, thisPoint))
                {
                    await _gameContainer.MakeMoveAsync(_gameContainer.DotList.GetKey(thisDot));
                    return;
                }
            }
        }
        protected override void CreateSpaces()
        {
            _whiteRect = GetActualRectangle(10, 10, 470, 470);
            var thisSize = GetActualSize(60, 60);
            _spaceHeight = thisSize.Height;
            thisSize = GetActualSize(53, 53);
            _squareHeight = thisSize.Height;
            LoadSquares();
            LoadDots();
            LoadLines();
        }
        private SKPaint? _brownLine;
        private SKPaint? _greenLine;
        private SKPaint? _redSolid;
        private SKPaint? _blueSolid;
        private SKPaint? _whitePaint;
        private SKPaint? _blackPaint;
        private SKPaint? _thickSelect;
        private SKPaint? _yellowPaint;
        private readonly ConnectTheDotsGameContainer _gameContainer;

        protected override void SetUpPaints()
        {
            _brownLine = MiscHelpers.GetStrokePaint(SKColors.Brown, 2);
            _greenLine = MiscHelpers.GetStrokePaint(SKColors.Green, 2);
            _redSolid = MiscHelpers.GetSolidPaint(SKColors.Red);
            _blueSolid = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _blackPaint = MiscHelpers.GetSolidPaint(SKColors.Black);
            _thickSelect = MiscHelpers.GetStrokePaint(SKColors.Black, 4);
            _yellowPaint = MiscHelpers.GetSolidPaint(SKColors.Yellow);
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            canvas.Clear();
            canvas.DrawRect(_whiteRect, _whitePaint);
            foreach (var thisLine in _gameContainer.LineList!.Values)
            {
                SKPaint thisPen;
                if (thisLine.IsTaken)
                {
                    if (thisLine.Equals(_gameContainer.PreviousLine))
                        thisPen = _greenLine!;
                    else
                        thisPen = _brownLine!;
                    canvas.DrawLine(thisLine.StartingPoint, thisLine.FinishingPoint, thisPen);
                }
            }
            var tempList = _gameContainer.SquareList!.Values.Where(items => items.Player > 0).ToCustomBasicList();
            tempList.ForEach(thisSquare =>
            {
                SKPaint thisPaint;
                if (thisSquare.Color == 1)
                    thisPaint = _blueSolid!;
                else if (thisSquare.Color == 2)
                    thisPaint = _redSolid!;
                else
                    throw new BasicBlankException("Must fill in color");
                canvas.DrawOval(thisSquare.Rectangle, thisPaint);
            });
            foreach (var thisDot in _gameContainer.DotList!.Values)
            {
                if (thisDot.IsSelected)
                {
                    canvas.DrawOval(thisDot.Bounds, _yellowPaint);
                    canvas.DrawOval(thisDot.Bounds, _thickSelect);
                }
                else
                {
                    canvas.DrawOval(thisDot.Dot, _blackPaint);
                }
            }
        }
    }
}
