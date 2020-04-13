using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Collections;
using System.Collections.Generic;
namespace BingoCP.Data
{
    public class PlayerBingo : IBoardCollection<BingoItem>
    {
        private readonly BoardCollection<BingoItem> _privateBoard;
        public PlayerBingo()
        {
            _privateBoard = new BoardCollection<BingoItem>(5, 5);
            Init();
        }
        public PlayerBingo(IEnumerable<BingoItem> previousList)
        {
            _privateBoard = new BoardCollection<BingoItem>(previousList);
            Init();
        }
        CustomBasicList<CustomBasicList<BingoItem>>? _winList;
        private void Init()
        {
            _winList = _privateBoard.GetPossibleCombinations(5);
            _privateBoard.MainObjectSelector = Items => Items.DidGet;
        }

        public bool HasBingo
        {
            get
            {
                var PossList = _privateBoard.GetWinCombo(_winList!);
                return PossList.Count != 0;
            }
        }
        public void ClearBoard()
        {
            _privateBoard.Clear(); //hopefully this simple.
        }

        public BingoItem this[Vector ThisV] => _privateBoard[ThisV];

        public BingoItem this[int Row, int Column] => _privateBoard[Row, Column];

        public IEnumerator<BingoItem> GetEnumerator()
        {
            return _privateBoard.GetEnumerator();
        }

        public int GetTotalColumns()
        {
            return _privateBoard.GetTotalColumns();
        }

        public int GetTotalRows()
        {
            return _privateBoard.GetTotalRows();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _privateBoard.GetEnumerator();
        }
    }
}
