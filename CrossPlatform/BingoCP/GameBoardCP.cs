using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SkiaSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static SkiaSharpGeneralLibrary.SKExtensions.MiscHelpers;
namespace BingoCP
{
    //gameboard will not be responsible for whether somebody wins.
    public class GameBoardCP : IBoardCollection<SpaceInfoCP>
    {
        private readonly BoardCollection<SpaceInfoCP> _privateBoard;
        public GameBoardCP()
        {
            _privateBoard = new BoardCollection<SpaceInfoCP>(6, 5); //because we have header.  hopefully that works.
        }
        public GameBoardCP(IEnumerable<SpaceInfoCP> previousList)
        {
            _privateBoard = new BoardCollection<SpaceInfoCP>(previousList);
        }

        private bool _didInit;

        public SpaceInfoCP this[int row, int column] => _privateBoard[row, column];

        public SpaceInfoCP this[Vector thisV] => _privateBoard[thisV];

        private SKPaint? _borderPaint;
        private SKPaint? _bingoPaint;
        private SKPaint? _fillPaint;
        private SKPaint? _enablePaint;


        public void LoadBoard()
        {
            if (_didInit == true)
                return;
            _borderPaint = GetStrokePaint(SKColors.Black, 4);
            _fillPaint = GetSolidPaint(SKColors.White);
            _enablePaint = GetSolidPaint(SKColors.Black);
            var tempColor = SKColors.Blue;
            var thisColor = new SKColor(tempColor.Red, tempColor.Green, tempColor.Blue, 150); // i think
            _bingoPaint = GetSolidPaint(thisColor);
            var TempList = _privateBoard.Where(Items => Items.Vector.Row == 1).ToCustomBasicList();
            TempList.ForEach(ThisItem => ThisItem.IsEnabled = false);
            _privateBoard[4, 3].Text = "Free";
            _didInit = true;
        }
        public void ClearBoard(BingoViewModel thisMod)
        {
            _privateBoard.Clear(); //hopefully this works
            thisMod.NumberCalled = "";
        }
        public int GetTotalColumns()
        {
            return _privateBoard.GetTotalColumns();
        }

        public int GetTotalRows()
        {
            return _privateBoard.GetTotalRows();
        }

        public IEnumerator<SpaceInfoCP> GetEnumerator()
        {
            return _privateBoard.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _privateBoard.GetEnumerator();
        }
        public void DrawBingoPiece(SKCanvas thisCanvas, float width, float height, SpaceInfoCP thisSpace)
        {
            thisCanvas.Clear();
            var thisRect = SKRect.Create(0, 0, width, height);
            if (thisSpace.IsEnabled == false)
            {
                thisCanvas.DrawRect(thisRect, _enablePaint);
                return;
            }
            thisCanvas.DrawRect(thisRect, _fillPaint);
            thisCanvas.DrawRect(thisRect, _borderPaint);
            if (thisSpace.AlreadyMarked == true)
            {
                thisRect = SKRect.Create(4, 4, width - 8, height - 8);
                thisCanvas.DrawOval(thisRect, _bingoPaint);
            }
        }
    }
}