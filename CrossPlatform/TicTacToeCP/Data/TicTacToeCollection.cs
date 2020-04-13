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
using System.Collections.Generic;
using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using System.Collections;
//i think this is the most common things i like to do
namespace TicTacToeCP.Data
{
    public class TicTacToeCollection : IEnumerable<SpaceInfoCP>, IBoardCollection<SpaceInfoCP>
    {
        private readonly BoardCollection<SpaceInfoCP> _privateBoard;
        private CustomBasicList<CustomBasicList<SpaceInfoCP>>? _winList;
        public TicTacToeCollection()
        {
            _privateBoard = new BoardCollection<SpaceInfoCP>(3, 3);
            FinishInit();
        }
        private void FinishInit()
        {
            _winList = _privateBoard.GetPossibleCombinations(3);
            _privateBoard.MainObjectSelector = items => items.Status;
            _privateBoard.BoardResultSelector = items =>
            {
                if (_privateBoard.ConsoleInfo!.ExtraSpaces == false)
                {
                    if (items.Status == EnumSpaceType.O)
                        return "O".Single();
                    return "X".Single();
                }
                if (items.Status == EnumSpaceType.O)
                    return "o".Single();
                return "x".Single();
            };
        }
        public TicTacToeCollection(IEnumerable<SpaceInfoCP> previousList)
        {
            _privateBoard = new BoardCollection<SpaceInfoCP>(previousList);
            FinishInit();
        }
        public WinInfo GetWin()
        {
            WinInfo output = new WinInfo();
            output.WinList = _privateBoard.GetWinCombo(_winList!);
            output.IsDraw = _privateBoard.IsAllFilled();
            if (output.WinList.Count > 0)
            {
                if (output.WinList.Count != 3)
                    throw new BasicBlankException("Must have 3 to win");
                Vector FirstSpace = output.WinList.First().Vector;
                Vector SecondSpace = output.WinList[1].Vector;
                if (FirstSpace.Column == SecondSpace.Column)
                    output.Category = EnumWinCategory.TopDown;
                else if (FirstSpace.Row == SecondSpace.Row)
                    output.Category = EnumWinCategory.LeftRight;
                else if (SecondSpace.Column > FirstSpace.Column)
                    output.Category = EnumWinCategory.TopLeft;
                else
                    output.Category = EnumWinCategory.TopRight;
            }
            return output;
        }
        public CustomBasicList<SpaceInfoCP> GetAlmostWinList()
        {
            return _privateBoard.GetAlmostWinList(_winList!);
        }
        public void Clear()
        {
            _privateBoard.Clear();
        }
        public void PrintBoard()
        {
            _privateBoard.PrintBoard();
        }
        public SpaceInfoCP this[int row, int column]
        {
            get
            {
                return _privateBoard[row, column];
            }
        }
        public SpaceInfoCP this[Vector thisV] //when you click, it has to be a vector.
        {
            get
            {
                return _privateBoard[thisV];
            }
        }
        public bool IsFilled(Vector thisV)
        {
            return _privateBoard.IsFilled(thisV);
        }
        public bool IsFilled(int row, int column)
        {
            return _privateBoard.IsFilled(row, column);
        }
        public IEnumerator<SpaceInfoCP> GetEnumerator()
        {
            return _privateBoard.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
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
    }
}
