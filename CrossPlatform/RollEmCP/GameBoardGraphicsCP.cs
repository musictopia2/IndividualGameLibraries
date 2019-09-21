using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.BasicGameBoards;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.GeneralGraphics;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Collections.Generic;
using System.Linq;
namespace RollEmCP
{
    [SingletonGame]
    public class GameBoardGraphicsCP : BaseGameBoardCP<CheckerPiecesCP>
    {
        private readonly RollEmMainGameClass _mainGame;
        public GameBoardGraphicsCP(IGamePackageResolver mainContainer) : base(mainContainer)
        {
            _mainGame = mainContainer.Resolve<RollEmMainGameClass>();
        }
        private FrameGraphics? thisFrame;
        public static void CreateNumberList(RollEmMainGameClass mainGame)
        {
            int x;
            int y;
            int z = 0;
            mainGame.NumberList = new Dictionary<int, NumberInfo>();
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 3; y++)
                {
                    z++;
                    var thisNumber = new NumberInfo();
                    thisNumber.Number = z;
                    mainGame.NumberList.Add(z, thisNumber);
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
            thisFrame = new FrameGraphics(null); //hopefully that works.  could be iffy.
            thisFrame.Visible = true;
            thisFrame.Text = "Number List";
            var bounds = GetBounds();
            thisFrame.ActualHeight = bounds.Height;
            thisFrame.ActualWidth = bounds.Width;
            for (x = 1; x <= 4; x++)
            {
                lefts = 8;
                for (y = 1; y <= 3; y++)
                {
                    z += 1;
                    NumberInfo thisNumber = _mainGame.NumberList![z];
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
        protected override void ClickProcess(SKPoint thisPoint)
        {
            if (_mainGame.ThisMod!.SpaceCommand!.CanExecute(0) == false)
                return;
            var thisList = _mainGame.NumberList!.Values.Where(Items => Items.IsCrossed == false && Items.Recently == false).ToCustomBasicList();
            foreach (var thisNumber in thisList)
            {
                if (MiscHelpers.DidClickRectangle(thisNumber.Bounds, thisPoint) == true)
                {
                    _mainGame.ThisMod.SpaceCommand.Execute(_mainGame.NumberList.GetKey(thisNumber));
                    return;
                }
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
            thisFrame!.IsEnabled = _mainGame.ThisMod!.SpaceCommand!.CanExecute(0);
            thisFrame.DrawFrame(canvas);
            var thisSize = GetActualSize(23, 23);
            foreach (var thisNumber in _mainGame.NumberList!.Values)
            {
                canvas.DrawOval(thisNumber.Bounds, _bluePaint);
                float fontSize = thisNumber.Bounds.Height * .8f;
                var textPaint = MiscHelpers.GetTextPaint(SKColors.White, fontSize);
                canvas.DrawBorderText(thisNumber.Number.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, _blackBorder, thisNumber.Bounds);
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