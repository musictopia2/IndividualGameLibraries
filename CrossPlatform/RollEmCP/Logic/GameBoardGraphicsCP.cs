using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using RollEmCP.Data;
using SkiaSharp;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RollEmCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerPiecesCP>
    {
        private readonly RollEmGameContainer _gameContainer;
        public GameBoardGraphicsCP(RollEmGameContainer gameContainer) : base(gameContainer.Resolver)
        {
            _gameContainer = gameContainer;
        }
        private FrameGraphics? _thisFrame;
        public static void CreateNumberList(RollEmGameContainer gameContainer) //hopefully that is okay.
        {
            int x;
            int y;
            int z = 0;
            gameContainer.NumberList = new Dictionary<int, NumberInfo>();
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 3; y++)
                {
                    z++;
                    var thisNumber = new NumberInfo();
                    thisNumber.Number = z;
                    gameContainer.NumberList.Add(z, thisNumber);
                }
            }
        }
        protected override void CreateSpaces()
        {
            var thisSize = GetActualSize(71, 51);
            int x;
            int y;
            int z = 0;
            float tops = 13;
            float lefts;
            _thisFrame = new FrameGraphics(null!); //hopefully that works.  could be iffy.
            _thisFrame.Visible = true;
            _thisFrame.Text = "Number List";
            var bounds = GetBounds();
            _thisFrame.ActualHeight = bounds.Height;
            _thisFrame.ActualWidth = bounds.Width;
            for (x = 1; x <= 4; x++)
            {
                lefts = 8;
                for (y = 1; y <= 3; y++)
                {
                    z += 1;
                    NumberInfo thisNumber = _gameContainer.NumberList![z];
                    thisNumber.Bounds = SKRect.Create(lefts, tops, thisSize.Width, thisSize.Height);
                    thisNumber.Number = z;
                    lefts += thisSize.Width + 3;
                }
                tops += thisSize.Height + 3;
            }
        }
        private SKPaint? _limeBorder;
        private SKPaint? _redBorder;
        private SKPaint? _bluePaint;
        private SKPaint? _blackBorder;
        protected override void SetUpPaints()
        {
            _limeBorder = MiscHelpers.GetStrokePaint(SKColors.LimeGreen, 10);
            _redBorder = MiscHelpers.GetStrokePaint(SKColors.Red, 10);
            _bluePaint = MiscHelpers.GetSolidPaint(SKColors.Blue);
            _blackBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 1);
        }
        protected override async Task ClickProcessAsync(SKPoint thisPoint)
        {
            if (CanEnableMove == false)
            {
                return;
            }
            if (_gameContainer.MakeMoveAsync == null)
            {
                throw new BasicBlankException("Nobody is handling make move.  Rethink");
            }
            var thisList = _gameContainer.NumberList!.Values.Where(Items => Items.IsCrossed == false && Items.Recently == false).ToCustomBasicList();
            foreach (var thisNumber in thisList)
            {
                if (MiscHelpers.DidClickRectangle(thisNumber.Bounds, thisPoint) == true)
                {
                    await _gameContainer.ProcessCustomCommandAsync(_gameContainer.MakeMoveAsync, _gameContainer.NumberList.GetKey(thisNumber));
                    return;
                }
            }
        }
        private bool CanEnableMove
        {
            get
            {
                if (_gameContainer.SaveRoot.GameStatus == EnumStatusList.NeedRoll)
                {
                    return false;
                }
                return !_gameContainer.Command.IsExecuting;
            }
        }
        public override string TagUsed => ""; //try this way.  hopefully that works.
        protected override SKSize OriginalSize { get; set; } = new SKSize(230, 230); // can adjust as needed
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            _thisFrame!.IsEnabled = CanEnableMove;
            _thisFrame.DrawFrame(canvas);
            var thisSize = GetActualSize(23, 23);
            foreach (var thisNumber in _gameContainer.NumberList!.Values)
            {
                canvas.DrawOval(thisNumber.Bounds, _bluePaint);
                float fontSize = thisNumber.Bounds.Height * .8f;
                var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
                canvas.DrawBorderText(thisNumber.Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder!, thisNumber.Bounds);
                if (thisNumber.IsCrossed == true || thisNumber.Recently == true)
                {
                    SKPaint thisPen;
                    if (thisNumber.IsCrossed == true)
                        thisPen = _redBorder!;
                    else
                        thisPen = _limeBorder!;
                    var y = thisNumber.Bounds.Top + thisSize.Height;
                    var FirstX = thisNumber.Bounds.Left;
                    var SecondX = thisNumber.Bounds.Right;
                    canvas.DrawLine(FirstX, y, SecondX, y, thisPen);
                }
            }
        }
    }
}