using BasicGameFramework.Attributes;
using SkiaSharp;
using SkiaSharpGeneralLibrary.SKExtensions;
namespace TicTacToeCP
{
    [SingletonGame]
    public class TicTacToeGraphicsCP
    {
        public WinInfo ThisWin = new WinInfo();
        private readonly TicTacToeCollection _spaceList;
        private readonly SKPaint _blackPen;
        private readonly SKPaint _whitePaint;
        private readonly SKPaint _winPen;
        private readonly SKPaint _bluePen;
        public int SpaceSize { get; set; }
        public TicTacToeGraphicsCP(TicTacToeMainGameClass mainGame)
        {
            _spaceList = mainGame.SaveRoot!.GameBoard;
            _blackPen = MiscHelpers.GetStrokePaint(SKColors.Black, 2);
            _whitePaint = MiscHelpers.GetSolidPaint(SKColors.White);
            _winPen = MiscHelpers.GetStrokePaint(SKColors.Red, 8);
            _bluePen = MiscHelpers.GetStrokePaint(SKColors.Blue, 2);
        }
        public void DrawSpace(SKCanvas thisCanvas, SpaceInfoCP thisSpace, float width, float height)
        {
            thisCanvas.Clear(); // i think it should clear out not matter what.
            var thisRect = SKRect.Create(0, 0, width, height);
            thisCanvas.DrawRect(thisRect, _whitePaint);
            thisCanvas.DrawRect(thisRect, _blackPen);
            if (ThisWin.WinList.Count > 0)
            {
                var firstSquare = _spaceList[ThisWin.WinList[0].Vector];
                var secondSquare = _spaceList[ThisWin.WinList[1].Vector];
                var thirdSquare = _spaceList[ThisWin.WinList[2].Vector];
                if (thisSpace.Equals(firstSquare) == true || thisSpace.Equals(secondSquare) == true || thisSpace.Equals(thirdSquare) == true)
                {
                    float firstX = 0;
                    float secondX = 0;
                    float firstY = 0;
                    float secondY = 0;
                    switch (ThisWin.Category)
                    {
                        case EnumWinCategory.LeftRight:
                            {
                                firstY = height / 2;
                                secondY = height / 2;
                                firstX = 0;
                                secondX = width;
                                break;
                            }
                        case EnumWinCategory.TopDown:
                            {
                                firstY = 0;
                                secondY = height;
                                firstX = width / 2;
                                secondX = width / 2;
                                break;
                            }
                        case EnumWinCategory.TopLeft:
                            {
                                firstY = 0;
                                firstX = 0;
                                secondY = height;
                                secondX = width;
                                break;
                            }
                        case EnumWinCategory.TopRight:
                            {
                                firstX = width;
                                secondX = 0;
                                secondY = height;
                                break;
                            }
                    }
                    thisCanvas.DrawLine(firstX, firstY, secondX, secondY, _winPen);
                }
            }
            if (thisSpace.Status == EnumSpaceType.O)
                thisCanvas.DrawOval(thisRect, _bluePen); //circle.
            else if (thisSpace.Status == EnumSpaceType.X)
            {
                thisCanvas.DrawLine(5, 5, width - 5, height - 5, _bluePen);
                thisCanvas.DrawLine(width - 5, 5, 5, height - 5, _bluePen);
            }
        }
    }
}