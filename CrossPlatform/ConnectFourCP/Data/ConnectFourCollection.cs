using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using System.Collections.Generic;
using System.Collections;
//i think this is the most common things i like to do
namespace ConnectFourCP.Data
{
    public class ConnectFourCollection : IBoardCollection<SpaceInfoCP>
    {
        private readonly BoardCollection<SpaceInfoCP> _privateBoard;
        private CustomBasicList<CustomBasicList<SpaceInfoCP>>? _winList;
        public SpaceInfoCP this[Vector thisV] => _privateBoard[thisV];
        public SpaceInfoCP this[int row, int column] => _privateBoard[row, column];
        public ConnectFourCollection()
        {
            _privateBoard = new BoardCollection<SpaceInfoCP>(6, 7);
            FinishInit();
        }
        public WinInfo GetWin()
        {
            WinInfo output = new WinInfo();
            output.WinList = _privateBoard.GetWinCombo(_winList!);
            output.IsDraw = _privateBoard.IsAllFilled();
            return output;
        }
        public void Clear()
        {
            _privateBoard.Clear();
        }
        public bool IsFilled(int column)
        {
            CustomBasicList<SpaceInfoCP> thisList = _privateBoard.GetAllRows(column);
            return (thisList.All(items => items.IsFilled() == true));
        }
        public Vector GetBottomSpace(int column)
        {
            if (column == 0)
                throw new BasicBlankException("Column cannot be 0");
            CustomBasicList<SpaceInfoCP> thisList = _privateBoard.GetAllRows(column);
            return thisList.Where(items => items.HasImage == false).OrderByDescending(items => items.Vector.Row).Take(1).Single().Vector;
        }
        private void FinishInit()
        {
            _winList = _privateBoard.GetPossibleCombinations(4);
            _privateBoard.MainObjectSelector = items => items.Player;
        }
        public ConnectFourCollection(IEnumerable<SpaceInfoCP> previousList)
        {
            _privateBoard = new BoardCollection<SpaceInfoCP>(previousList);
            FinishInit();
        }
        public IEnumerator<SpaceInfoCP> GetEnumerator()
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
