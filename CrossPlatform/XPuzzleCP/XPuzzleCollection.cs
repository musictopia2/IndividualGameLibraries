using BasicGameFramework.DIContainers;
using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace XPuzzleCP
{
    public class XPuzzleCollection : IBoardCollection<XPuzzleSpaceInfo>
    {
        private readonly BoardCollection<XPuzzleSpaceInfo> _privateBoard;
        public void SetContainer(IGamePackageResolver thisCon)
        {
            _privateBoard.MainContainer = thisCon;
        }
        public XPuzzleCollection()
        {
            _privateBoard = new BoardCollection<XPuzzleSpaceInfo>(3, 3);
            FinishInit();
            LoadBoard();
        }
        public XPuzzleCollection(IEnumerable<XPuzzleSpaceInfo> previousList)
        {
            _privateBoard = new BoardCollection<XPuzzleSpaceInfo>(previousList);
            FinishInit();
        }
        internal void ClearBoard()
        {
            _privateBoard.Clear();
        }
        private void FinishInit()
        {
            _privateBoard.BoardResultSelector = items =>
            {
                return items.Text.Single(); //i think
            };
        }
        private void LoadBoard()
        {
            _privateBoard.ForEach(thisSpace =>
            {
                thisSpace.Text = "";
                if (!(thisSpace.Vector.Column == 3 && thisSpace.Vector.Row == 3))
                    thisSpace.Color = cs.Navy;
                else
                    thisSpace.Color = cs.Black;
            });
        }
        public XPuzzleSpaceInfo this[int row, int column]
        {
            get
            {
                return _privateBoard[row, column];
            }
        }
        public XPuzzleSpaceInfo this[Vector thisV] //when you click, it has to be a vector.
        {
            get
            {
                return _privateBoard[thisV];
            }
        }

        public void PopulateBoard(CustomBasicList<int> thisList) //this means i can test some conditions.
        {
            if (thisList.Count != 8)
                throw new BasicBlankException("The list must be 8 items");
            _privateBoard.ForEach(thisSpace =>
            {
                if (!(thisSpace.Vector.Column == 3 && thisSpace.Vector.Row == 3))
                {
                    thisSpace.Text = thisList.First().ToString();
                    thisList.RemoveFirstItem();
                    thisSpace.Color = cs.Navy;
                }
                else
                {
                    thisSpace.Text = ""; //just in case i did not clear the board.
                    thisSpace.Color = cs.Black;
                }

            });
            if (thisList.Count != 0)
                throw new BasicBlankException("Did not use up all the list. Rethink");
        }
        public void PrintBoard()
        {
            _privateBoard.PrintBoard();
        }
        public IEnumerator<XPuzzleSpaceInfo> GetEnumerator()
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