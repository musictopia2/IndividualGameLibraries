using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Linq;
namespace ConnectTheDotsCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerPiecesCP>
    {
        private readonly GlobalVariableClass _thisGlobal;
        private readonly ConnectTheDotsViewModel _thisMod;
        private readonly ConnectTheDotsMainGameClass _mainGame;
        public GameBoardGraphicsCP(IGamePackageResolver MainContainer) : base(MainContainer)
        {
            _thisGlobal = MainContainer.Resolve<GlobalVariableClass>(); //i think.
            _thisMod = MainContainer.Resolve<ConnectTheDotsViewModel>();
            _mainGame = MainContainer.Resolve<ConnectTheDotsMainGameClass>();
        }
        public override string TagUsed => "";
        protected override SKSize OriginalSize { get; set; } = new SKSize(480, 480); // can adjust as needed
        private float _squareHeight;
        private float _spaceHeight;
        SKRect _whiteRect;
        internal static void InitBoard(GlobalVariableClass thisGlobal)
        {
            InitDots(thisGlobal);
            InitSquares(thisGlobal);
            InitLines(thisGlobal);
        }
        private void LoadDots() //this loads in the dot and bounds.
        {

            if (_thisGlobal.DotList!.Count == 0)
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
                    DotInfo thisDot = _thisGlobal.DotList[z];

                    thisDot.Dot = SKRect.Create(currentLeft, currentTop, dotSize.Width, dotSize.Height);
                    thisDot.Bounds = SKRect.Create(currentLeft - (bigHeight.Width / 3), currentTop - (bigHeight.Height / 3), bigHeight.Width, bigHeight.Height);
                    currentLeft += _spaceHeight;
                    currentHiddenLeft += _spaceHeight;
                }
                currentTop += _spaceHeight;
                currentHiddenTop += _spaceHeight;
            }
        }
        private static void InitDots(GlobalVariableClass thisGlobal)
        {
            thisGlobal.DotList = new System.Collections.Generic.Dictionary<int, DotInfo>();
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    DotInfo thisDot = new DotInfo();
                    thisDot.Row = x;
                    thisDot.Column = y;
                    thisGlobal.DotList.Add(thisGlobal.DotList.Count + 1, thisDot);
                }
            }
        }
        private void LoadSquares()
        {
            if (_thisGlobal.SquareList!.Count == 0)
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
                    SquareInfo thisSquare = _thisGlobal.SquareList[z];
                    thisSquare.Rectangle = SKRect.Create(currentLeft, currentTop, _squareHeight, _squareHeight);
                    currentLeft += _spaceHeight;
                }
                currentTop += _spaceHeight;
            }
        }
        private static void InitSquares(GlobalVariableClass thisGlobal)
        {
            thisGlobal.SquareList = new System.Collections.Generic.Dictionary<int, SquareInfo>();
            for (int x = 1; x <= 7; x++)
            {
                for (int y = 1; y <= 7; y++)
                {
                    SquareInfo thisSquare = new SquareInfo();
                    thisSquare.Row = x;
                    thisSquare.Column = y;
                    thisSquare.Color = 0; //i think.
                    thisGlobal.SquareList.Add(thisGlobal.SquareList.Count + 1, thisSquare);
                }
            }
        }
        private void LoadLines()
        {
            if (_thisGlobal.LineList!.Count == 0)
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
                    thisLine = _thisGlobal.LineList[z];
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
                        thisLine = _thisGlobal.LineList[z];
                        thisLine.StartingPoint = new SKPoint(oldLeft, oldTop);
                        newTop = oldTop + _spaceHeight;
                        thisLine.FinishingPoint = new SKPoint(oldLeft, newTop);
                        oldLeft += _spaceHeight;
                    }
                }
                oldTop += _spaceHeight;
            }
        }
        private static void InitLines(GlobalVariableClass thisGlobal)
        {
            thisGlobal.LineList = new System.Collections.Generic.Dictionary<int, LineInfo>();
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
                    thisLine.Index = thisGlobal.LineList.Count + 1;
                    thisGlobal.LineList.Add(thisLine.Index, thisLine);
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
                        thisLine.Index = thisGlobal.LineList.Count + 1;
                        thisGlobal.LineList.Add(thisLine.Index, thisLine);
                    }
                }
            }
        }
        protected override bool CanStartPaint()
        {
            return _mainGame.DidChooseColors;
        }
        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (_thisMod.SpaceCommand!.CanExecute(0) == false)
                return;

            foreach (var thisDot in _thisGlobal.DotList!.Values)
            {
                if (MiscHelpers.DidClickRectangle(thisDot.Bounds, thisPoint))
                    _thisMod.SpaceCommand.Execute(_thisGlobal.DotList.GetKey(thisDot));
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
            foreach (var thisLine in _thisGlobal.LineList!.Values)
            {
                SKPaint thisPen;
                if (thisLine.IsTaken)
                {
                    if (thisLine.Equals(_thisGlobal.PreviousLine))
                        thisPen = _greenLine!;
                    else
                        thisPen = _brownLine!;
                    canvas.DrawLine(thisLine.StartingPoint, thisLine.FinishingPoint, thisPen);
                }
            }
            var tempList = _thisGlobal.SquareList!.Values.Where(items => items.Player > 0).ToCustomBasicList();
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
            foreach (var thisDot in _thisGlobal.DotList!.Values)
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