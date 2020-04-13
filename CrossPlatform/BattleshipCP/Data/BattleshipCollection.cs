using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using System.Collections;
using System.Collections.Generic;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BattleshipCP.Data
{
    public class BattleshipCollection : IBoardCollection<FieldInfoCP>
    {
        private readonly BoardCollection<FieldInfoCP> _privateBoard;
        public BattleshipCollection()
        {
            _privateBoard = new BoardCollection<FieldInfoCP>(10, 10); //needs to be 10 by 10.
        }
        public void Clear()
        {
            _privateBoard.Clear();
        }
        public void PlayerWaiting()
        {
            foreach (var thisPlayer in _privateBoard)
            {
                thisPlayer.FillColor = cs.Yellow; //i think
            }
        }
        public FieldInfoCP this[Vector thisV] => _privateBoard[thisV];
        public FieldInfoCP this[int row, int column] => _privateBoard[row, column];
        public IEnumerator<FieldInfoCP> GetEnumerator()
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
