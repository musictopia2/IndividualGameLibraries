using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MiscProcesses;
using BasicGameFramework.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BattleshipCP
{
    [SingletonGame]
    public class BattleshipComputerAI
    {
        private Vector _firstHit;
        private readonly GameBoardCP _gameBoard1;
        private readonly RandomGenerator _rs;
        private readonly IAsyncDelayer _delay;
        private readonly TestOptions _thisTest;
        public BattleshipComputerAI(GameBoardCP gameBoard1,
            IAsyncDelayer delay, TestOptions thisTest)
        {
            _gameBoard1 = gameBoard1;
            _rs = new RandomGenerator(); //since we are not unit testing, no problem to just create a new random generator for computer player this time
            _delay = delay;
            _thisTest = thisTest;
        }
        public void StartNewGame()
        {
            _firstHit = new Vector();
        }
        public bool HasHit(Vector space)
        {
            FieldInfoCP thisField;
            thisField = _gameBoard1.ComputerList![space];
            if (thisField.ShipNumber > 0)
                return true;
            return false;
        }
        public Vector ComputerMove()
        {
            Vector moves;
            if (_firstHit.Column == 0)
                // just at random
                return RandomMove();
            moves = SmartHit();
            if (moves.Column > 0)
                return moves;
            return RandomMove();
        }
        private Vector RandomMove()
        {
            do
            {
                Vector space = GetRandomVector();
                if (IsLegal(space) == true)
                    return space;
            } while (true);
        }
        public void MarkHit(Vector space)
        {
            if (_firstHit.Column == 0)
                _firstHit = space;
        }
        private bool IsLegal(Vector space)
        {
            if (space.Column == 0 && space.Row == 0)
                return false;
            FieldInfoCP thisField = _gameBoard1.ComputerList![space];
            return thisField.Hit == EnumWhatHit.None;
        }
        public async Task ComputerPlaceShipsAsync()
        {
            if (_thisTest.NoAnimations == false)
                await _delay.DelaySeconds(.3);
            CustomBasicList<int> shipList = new CustomBasicList<int>()
            { 5, 4, 3, 2, 2};
            CustomBasicList<int> chooseList = shipList.ToCustomBasicList();
            chooseList.ShuffleList();
            int ComputerShip;
            Vector thisSpace;
            bool Lefts;
            FieldInfoCP thisField;
            Vector latestSpace;
            5.Times(x =>
            {
                do
                {
                    Lefts = _rs.NextBool();
                    thisSpace = GetRandomVector();
                    ComputerShip = chooseList[x - 1];
                    if (CanComputerPlaceShip(thisSpace, Lefts, ComputerShip))
                        break;

                } while (true);
                latestSpace = thisSpace;
                if (ComputerShip == 1)
                    throw new BasicBlankException("Problem With Computer Placing Ship");
                ComputerShip.Times(y =>
                {
                    thisField = _gameBoard1.ComputerList![latestSpace];
                    thisField.ShipNumber = 1;
                    if (Lefts == true)
                        latestSpace.Column++;
                    else
                        latestSpace.Row++;
                });
            });
            int counts = _gameBoard1.ComputerList.Count(Items => Items.ShipNumber == 1);
            if (counts != 16)
                throw new BasicBlankException($"Must have 16 ships to begin with, not {counts}");
            if (_thisTest.NoAnimations == false)
                await _delay.DelaySeconds(.5);
        }
        private Vector SmartHit()
        {
            CustomBasicList<Vector> offSetList = new CustomBasicList<Vector>
            {
                new Vector(0, -1), //this means minus one column
                new Vector(0, 1),
                new Vector(-1, 0),
                new Vector(1, 0)
            };
            bool foundGoodSpot = false;
            Vector nextHit = _firstHit;
            Vector whatMove = new Vector();
            int offsetIdx = 1;
            while (!foundGoodSpot & (_firstHit.Column != 0))
            {
                bool goneTooFar = false;
                AddVector(ref nextHit, offSetList[offsetIdx - 1]); //because 0 based.
                FieldInfoCP thisField;
                if (nextHit.Column <= 0 || nextHit.Column > 10 || nextHit.Row <= 0 || nextHit.Row > 10)
                {
                    goneTooFar = true;
                }
                else
                {
                    thisField = _gameBoard1.ComputerList![nextHit];
                    if (thisField.Hit == EnumWhatHit.Miss)
                        goneTooFar = true;
                }
                if (goneTooFar == true)
                {
                    offsetIdx++;
                    nextHit = _firstHit;
                    if (offsetIdx > 4)
                        _firstHit = new Vector();
                }
                thisField = _gameBoard1.ComputerList![nextHit];
                if (thisField.Hit == EnumWhatHit.None)
                {
                    whatMove = nextHit;
                    foundGoodSpot = true;
                }
            }
            return whatMove;
        }
        private void AddVector(ref Vector firstVector, Vector addVector)
        {
            firstVector.Row += addVector.Row;
            firstVector.Column += addVector.Column;
        }
        private Vector GetRandomVector()
        {
            int row = _rs.GetRandomNumber(10);
            int column = _rs.GetRandomNumber(10);
            return new Vector(row, column);
        }
        private bool CanComputerPlaceShip(Vector space, bool lefts, int typeship)
        {
            int maxRows;
            int maxCols;
            FieldInfoCP newField;
            maxCols = 10; // that needs to change as well
            maxRows = 10; // needs to change as well
            int x;
            if (lefts == true)
            {
                if (space.Column + typeship - 1 > maxCols)
                {
                    return false;
                }
                var loopTo = typeship;
                for (x = 1; x <= loopTo; x++)
                {
                    newField = _gameBoard1.ComputerList![space.Row, space.Column + x - 1];
                    if (newField.ShipNumber != 0)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (((space.Row + typeship) - 1) > maxRows)
                {
                    return false;
                }
                var loopTo1 = typeship;
                for (x = 1; x <= loopTo1; x++)
                {
                    newField = _gameBoard1.ComputerList![space.Row + x - 1, space.Column];
                    if (newField.ShipNumber != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}