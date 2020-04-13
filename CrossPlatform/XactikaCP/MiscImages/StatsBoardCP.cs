using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
using System.Threading.Tasks;
using XactikaCP.Data;

namespace XactikaCP.MiscImages
{
    [SingletonGame]
    public class StatsBoardCP : BaseGameBoardCP
    {
        public StatsBoardCP(IGamePackageResolver mainContainer) : base(mainContainer) { }

        public override string TagUsed => "main";
        protected override SKSize OriginalSize { get; set; } = new SKSize(121, 225); // can adjust as needed
        protected override bool CanStartPaint()
        {
            return true;
        }
        protected override Task ClickProcessAsync(SKPoint thisPoint) { return Task.CompletedTask; } //no clicking.
        protected override void CreateSpaces() { } //do nothing here.
        private SKPaint? _whitePaint;
        private SKPaint? _thickBorder;
        private SKPaint? _lightGrayPaint;
        protected override void SetUpPaints()
        {
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _lightGrayPaint = MiscHelpers.GetSolidPaint(SKColors.LightGray);
            _thickBorder = MiscHelpers.GetStrokePaint(SKColors.Black, 2); // try 2
        }
        private void DrawTopRow(SKCanvas thisCanvas)
        {
            var firstRect = GetActualRectangle(3, 3, 35, 40);
            var secondRect = GetActualRectangle(38, 3, 35, 40);
            var thirdRect = GetActualRectangle(73, 3, 15, 40);
            var fourthRect = GetActualRectangle(88, 3, 15, 40);
            var fifthRect = GetActualRectangle(103, 3, 15, 40);
            DrawBorders(thisCanvas, firstRect);
            DrawBorders(thisCanvas, secondRect);
            DrawBorders(thisCanvas, thirdRect);
            DrawBorders(thisCanvas, fourthRect);
            DrawBorders(thisCanvas, fifthRect);
            var fontSize = firstRect.Height * 0.28f;
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            thisCanvas.DrawCustomText("Value", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, firstRect, out _);
            thisCanvas.DrawCustomText("Cards", TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, secondRect, out _);
            var cubeSize = GetActualSize(12, 12);
            var pointList = ImageHelpers.GetPoints(EnumShapes.Cubes, 1, thirdRect.Location, true, cubeSize.Height);
            foreach (var thisPoint in pointList)
            {
                var tempRect = SKRect.Create(thisPoint, cubeSize);
                ImageHelpers.DrawCube(thisCanvas, tempRect);
            }
            pointList = ImageHelpers.GetPoints(EnumShapes.Cubes, 2, fourthRect.Location, true, cubeSize.Height);
            foreach (var thisPoint in pointList)
            {
                var tempRect = SKRect.Create(thisPoint, cubeSize);
                ImageHelpers.DrawCube(thisCanvas, tempRect);
            }
            pointList = ImageHelpers.GetPoints(EnumShapes.Cubes, 3, fifthRect.Location, true, cubeSize.Height);
            foreach (var thisPoint in pointList)
            {
                var tempRect = SKRect.Create(thisPoint, cubeSize);
                ImageHelpers.DrawCube(thisCanvas, tempRect);
            }
        }
        private void DrawBorders(SKCanvas thisCanvas, SKRect thisRect)
        {
            thisCanvas.DrawRect(thisRect, _thickBorder);
        }
        private int GetTextValue(int column, int row)
        {
            if (column == 1)
                return row;
            if (column == 2)
            {
                if (row == 4 || row == 12)
                    return 1;
                if (row == 5 || row == 11)
                    return 4;
                if (row == 6 || row == 10)
                    return 10;
                if (row == 7 || row == 9)
                    return 16;
                if (row == 8)
                    return 19;
                throw new BasicBlankException("Unknown");
            }
            if (column == 3)
            {
                if (row == 4 || row == 10)
                    return 1;
                if (row == 5 || row == 9)
                    return 3;
                if (row == 6 || row == 8)
                    return 6;
                if (row == 7)
                    return 7;
                return 0;
            }
            if (column == 4)
            {
                if (row == 5 || row == 11)
                    return 1;
                if (row == 6 || row == 10)
                    return 3;
                if (row == 7 || row == 9)
                    return 6;
                if (row == 8)
                    return 7;
                return 0;
            }
            if (column == 5)
            {
                if (row == 6 || row == 12)
                    return 1;
                if (row == 7 || row == 11)
                    return 3;
                if (row == 8 || row == 10)
                    return 6;
                if (row == 9)
                    return 7;
                return 0;
            }
            throw new BasicBlankException("Unknown");
        }
        protected override void DrawBoard(SKCanvas canvas)
        {
            var tempRect = GetBounds();
            canvas.DrawRect(tempRect, _whitePaint);
            DrawTopRow(canvas);
            int x;
            int y;
            int tops;
            tops = 43;
            int lefts;
            var diff1X = 35;
            var diff2X = 15;
            var diffY = 20;
            bool doPaint = true;
            var fontSize = GetFontSize(15); // can adjust as needed
            var textPaint = MiscHelpers.GetTextPaint(SKColors.Black, fontSize);
            textPaint.FakeBoldText = true;
            for (x = 4; x <= 12; x++)
            {
                lefts = 3;
                for (y = 1; y <= 5; y++)
                {
                    int currentX;
                    if (y == 1 || y == 2)
                        currentX = diff1X;
                    else
                        currentX = diff2X;
                    var ThisRect = GetActualRectangle(lefts, tops, currentX, diffY);
                    if (doPaint == true)
                        canvas.DrawRect(ThisRect, _lightGrayPaint);
                    DrawBorders(canvas, ThisRect);
                    var displayValue = GetTextValue(y, x);
                    if (displayValue > 0)
                        canvas.DrawCustomText(displayValue.ToString(), TextExtensions.EnumLayoutOptions.Center, TextExtensions.EnumLayoutOptions.Center, textPaint, ThisRect, out _);

                    lefts += currentX;
                }
                tops += diffY;
                doPaint = !doPaint;
            }
        }
    }
}
