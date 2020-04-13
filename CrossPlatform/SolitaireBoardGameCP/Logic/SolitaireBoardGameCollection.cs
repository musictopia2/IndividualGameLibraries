using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.Exceptions;
using SolitaireBoardGameCP.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace SolitaireBoardGameCP.Logic
{
    public class SolitaireBoardGameCollection : IBoardCollection<GameSpace>
    {
        private readonly Dictionary<Vector, GameSpace> _privateDict;

        public SolitaireBoardGameCollection()
        {
            _privateDict = new Dictionary<Vector, GameSpace>();
        }
        public SolitaireBoardGameCollection(IEnumerable<GameSpace> ThisList)
        {
            _privateDict = ThisList.ToDictionary(Items => Items.Vector);
        }

        private void Check()
        {
            if (_privateDict.Count == 0)
                throw new BasicBlankException("You have to have at least one item for your dictionary");
        }

        private Vector GetVector(int row, int column)
        {
            Vector ThisV = new Vector(row, column);
            return ThisV;
        }

        public GameSpace this[int row, int column]
        {
            get
            {
                Check();
                Vector ThisGrid = GetVector(row, column);
                return _privateDict[ThisGrid];
            }
        }
        public GameSpace this[Vector thisV]
        {
            get
            {
                Check();
                return _privateDict[thisV];
            }
        }

        public void Add(GameSpace thisSpace)
        {
            _privateDict.Add(thisSpace.Vector, thisSpace);
        }

        public int GetTotalColumns()
        {
            return 7;
        }

        public int GetTotalRows()
        {
            return 7;
        }
        public IEnumerator<GameSpace> GetEnumerator()
        {
            return _privateDict.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _privateDict.Values.GetEnumerator();
        }
    }
}