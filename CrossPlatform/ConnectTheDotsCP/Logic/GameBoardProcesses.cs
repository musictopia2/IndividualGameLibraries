using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using ConnectTheDotsCP.Data;
using ConnectTheDotsCP.Graphics;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectTheDotsCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {

        private readonly ConnectTheDotsGameContainer _gameContainer;
        public GameBoardProcesses(ConnectTheDotsGameContainer gameContainer)
        {
            _gameContainer = gameContainer;
        }
        public void ClearBoard()
        {
            if (_gameContainer.DotList is null)
                GameBoardGraphicsCP.InitBoard(_gameContainer); //i think.
            _gameContainer.PreviousLine = new LineInfo();
            _gameContainer.PreviousDot = new DotInfo(); //i think.
            foreach (var thisSquare in _gameContainer.SquareList!.Values)
            {
                thisSquare.Color = 0;
                thisSquare.Player = 0;
            }
            foreach (var thisLine in _gameContainer.LineList!.Values)
            {
                thisLine.IsTaken = false;
            }
            foreach (var thisDot in _gameContainer.DotList!.Values)
            {
                thisDot.IsSelected = false;
            }
            _gameContainer.PlayerList!.ForEach(thisPlayer => thisPlayer.Score = 0);

            _gameContainer.RepaintBoard();
        }
        private LineInfo FindLine(int row, int column, bool horizontal)
        {
            return _gameContainer.LineList!.Values.Single(items => items.Row == row && items.Column == column && items.Horizontal == horizontal);
        }
        private bool IsGameOver => _gameContainer.SquareList!.Values.All(items => items.Player > 0);

        private int CalculateTotalPoints
        {
            get
            {
                int output = _gameContainer.SquareList!.Values.Count(items => items.Player == _gameContainer.SaveRoot!.PlayOrder.WhoTurn);
                if (output == 0)
                    throw new BasicBlankException("Must have at least one point");
                return output; //i think.
            }
        }
        private void GetSavedDot(int row, int column)
        {
            _gameContainer.PreviousDot = _gameContainer.DotList!.Values.Single(items => items.Row == row && items.Column == column);
        }
        public void SaveGame()
        {
            SavedBoardData thisData = new SavedBoardData();
            thisData.LineList = _gameContainer.LineList!.Values.Select(items => items.IsTaken).ToCustomBasicList();
            thisData.DotList = _gameContainer.DotList!.Values.Select(items => items.IsSelected).ToCustomBasicList();
            thisData.SquarePlayerList = _gameContainer.SquareList!.Values.Select(items => items.Player).ToCustomBasicList();
            thisData.PreviousColumn = _gameContainer.PreviousDot.Column;
            thisData.PreviousRow = _gameContainer.PreviousDot.Row;
            thisData.PreviousLine = _gameContainer.PreviousLine.Index;
            _gameContainer.SaveRoot!.BoardData = thisData;
        }
        public void LoadGame()
        {
            //if (_gameContainer.DidChooseColors == false)
            //    return;
            GameBoardGraphicsCP.InitBoard(_gameContainer); //i think here for sure.
            //not sure if it needed to be somewhere else (?)
            int x = 0;
            _gameContainer.SaveRoot!.BoardData!.DotList.ForEach(thisDot =>
            {
                x++;
                _gameContainer.DotList![x].IsSelected = thisDot;
            });
            x = 0;
            _gameContainer.SaveRoot.BoardData.LineList.ForEach(thisLine =>
            {
                x++;
                _gameContainer.LineList![x].IsTaken = thisLine;
            });
            x = 0;
            _gameContainer.SaveRoot.BoardData.SquarePlayerList.ForEach(thisSquare =>
            {
                x++;
                _gameContainer.SquareList![x].Player = thisSquare;
                if (thisSquare > 0)
                {
                    var tempPlayer = _gameContainer.PlayerList![thisSquare];
                    _gameContainer.SquareList[x].Color = (int)tempPlayer.Color; //hopefully this simple (?)
                }
            });
            if (_gameContainer.SaveRoot.BoardData.PreviousColumn > 0 && _gameContainer.SaveRoot.BoardData.PreviousRow > 0)
                GetSavedDot(_gameContainer.SaveRoot.BoardData.PreviousRow, _gameContainer.SaveRoot.BoardData.PreviousColumn);
            else
                _gameContainer.PreviousDot = new DotInfo();
            if (_gameContainer.SaveRoot.BoardData.PreviousLine > 0)
                _gameContainer.PreviousLine = _gameContainer.LineList![_gameContainer.SaveRoot.BoardData.PreviousLine];
            else
                _gameContainer.PreviousLine = new LineInfo();
        }
        private bool WonSelectedList(CustomBasicList<LineInfo> thisList)
        {
            return thisList.All(items => items.IsTaken == true);
        }
        private CustomBasicList<LineInfo> GetLineList(SquareInfo thisSquare)
        {
            CustomBasicList<LineInfo> output = new CustomBasicList<LineInfo>();
            var thisLine = FindLine(thisSquare.Row, thisSquare.Column, true);
            output.Add(thisLine);
            thisLine = FindLine(thisSquare.Row, thisSquare.Column, false);
            output.Add(thisLine);
            thisLine = FindLine(thisSquare.Row + 1, thisSquare.Column, true);
            output.Add(thisLine);
            thisLine = FindLine(thisSquare.Row, thisSquare.Column + 1, false);
            output.Add(thisLine);
            return output;
        }
        private bool DidWinSquare(int player, int color)
        {
            if (_gameContainer!.Test!.DoubleCheck == true)
                return false; //try this way.
            CustomBasicList<SquareInfo> winList = new CustomBasicList<SquareInfo>();
            foreach (var thisSquare in _gameContainer.SquareList!.Values)
            {
                if (thisSquare.Player == 0)
                {
                    var tempList = GetLineList(thisSquare);
                    if (WonSelectedList(tempList))
                        winList.Add(thisSquare);
                }
            }
            if (winList.Count == 0)
                return false;
            winList.ForEach(thisSquare =>
            {
                thisSquare.Color = color;
                thisSquare.Player = player;
            });
            return true;
        }
        private bool HasConnectedDot(DotInfo previousDot, DotInfo newDot)
        {
            if (previousDot.Equals(newDot))
                return false;
            if ((previousDot.Column == newDot.Column) & (previousDot.Row == newDot.Row))
                return false;
            if (((previousDot.Column + 1) == newDot.Column) & (previousDot.Row == newDot.Row))
                return true;
            if (((previousDot.Column - 1) == newDot.Column) & (previousDot.Row == newDot.Row))
                return true;
            if ((previousDot.Column == newDot.Column) & ((previousDot.Row + 1) == newDot.Row))
                return true;
            if ((previousDot.Column == newDot.Column) & ((previousDot.Row - 1) == newDot.Row))
                return true;
            return false;
        }
        private LineInfo GetConnectedLine(DotInfo previousDot, DotInfo newDot)
        {
            if (HasConnectedDot(previousDot, newDot) == false)
                throw new BasicBlankException("There is no connected dots here");
            foreach (var thisLine in _gameContainer.LineList!.Values)
            {
                if (thisLine.DotColumn1 == newDot.Column && thisLine.DotColumn2 == previousDot.Column && thisLine.DotRow1 == newDot.Row && thisLine.DotRow2 == previousDot.Row)
                    return thisLine;
                if (thisLine.DotColumn2 == newDot.Column && thisLine.DotColumn1 == previousDot.Column && thisLine.DotRow2 == newDot.Row && thisLine.DotRow1 == previousDot.Row)
                    return thisLine;
            }
            throw new BasicBlankException("Cannot find the connected dot.  Rethink");
        }
        public bool IsValidMove(int dot)
        {
            DotInfo thisDot = _gameContainer.DotList![dot];
            if (thisDot.Equals(_gameContainer.PreviousDot))
                return true; //because undoing move.
            if (_gameContainer.PreviousDot.Column == 0 && _gameContainer.PreviousDot.Row == 0)
                return true; //because starting new move.
            bool doesConnect = HasConnectedDot(_gameContainer.PreviousDot, thisDot);
            if (doesConnect == false)
                return false;
            LineInfo thisLine = GetConnectedLine(_gameContainer.PreviousDot, thisDot);
            return !thisLine.IsTaken;
        }
        public async Task MakeMoveAsync(int dot)
        {
            DotInfo thisDot = _gameContainer.DotList![dot];
            if (thisDot.Equals(_gameContainer.PreviousDot))
            {
                thisDot.IsSelected = false;
                _gameContainer.PreviousDot = new DotInfo();
                _gameContainer.RepaintBoard();
                //await _gameContainer.SaveStateAsync(); //this too.
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (_gameContainer.PreviousDot.Column == 0 && _gameContainer.PreviousDot.Row == 0)
            {
                thisDot.IsSelected = true;
                _gameContainer.PreviousDot = thisDot;
                _gameContainer.RepaintBoard();
                //await _gameContainer.SaveStateAsync(); //this too.
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            LineInfo thisLine = GetConnectedLine(_gameContainer.PreviousDot, thisDot);
            thisLine.IsTaken = true;
            _gameContainer.PreviousDot.IsSelected = false;
            _gameContainer.PreviousDot = new DotInfo();
            bool wins = DidWinSquare(_gameContainer.SaveRoot!.PlayOrder.WhoTurn, (int)_gameContainer.SingleInfo!.Color);
            _gameContainer.RepaintBoard();
            if (_gameContainer.Test.ImmediatelyEndGame)
            {
                await _gameContainer.ShowWinAsync!.Invoke();
                return;
            }
            if (wins == false)
            {
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            int totalPoints = CalculateTotalPoints;
            _gameContainer.SingleInfo.Score = totalPoints;
            if (IsGameOver)
            {
                _gameContainer.SingleInfo = _gameContainer.PlayerList.OrderByDescending(items => items.Score).First();
                await _gameContainer.ShowWinAsync!.Invoke();
                return;
            }
            //hopefully no need to savestate since continueturn should do it anyways.
            //await _gameContainer.SaveStateAsync(); //this too.
            await _gameContainer.ContinueTurnAsync!.Invoke(); //you get another turn for winning a square.
        }
    }
}
