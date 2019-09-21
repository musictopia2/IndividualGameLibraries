using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectTheDotsCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly GlobalVariableClass _thisGlobal;
        private readonly EventAggregator _thisE;
        private readonly ConnectTheDotsMainGameClass _mainGame;
        public GameBoardProcesses(GlobalVariableClass thisGlobal, EventAggregator thisE, ConnectTheDotsMainGameClass mainGame)
        {
            _thisGlobal = thisGlobal;
            _thisE = thisE;
            _mainGame = mainGame;
        }
        public void ClearBoard()
        {
            if (_thisGlobal.DotList is null)
                GameBoardGraphicsCP.InitBoard(_thisGlobal); //i think.
            _thisGlobal.PreviousLine = new LineInfo();
            _thisGlobal.PreviousDot = new DotInfo(); //i think.
            foreach (var thisSquare in _thisGlobal.SquareList!.Values)
            {
                thisSquare.Color = 0;
                thisSquare.Player = 0;
            }
            foreach (var thisLine in _thisGlobal.LineList!.Values)
            {
                thisLine.IsTaken = false;
            }
            foreach (var thisDot in _thisGlobal.DotList!.Values)
            {
                thisDot.IsSelected = false;
            }
            _mainGame.PlayerList!.ForEach(thisPlayer => thisPlayer.Score = 0);
            _thisE.RepaintBoard();
        }
        private LineInfo FindLine(int row, int column, bool horizontal)
        {
            return _thisGlobal.LineList!.Values.Single(items => items.Row == row && items.Column == column && items.Horizontal == horizontal);
        }
        private bool IsGameOver => _thisGlobal.SquareList!.Values.All(items => items.Player > 0);

        private int CalculateTotalPoints
        {
            get
            {
                int output = _thisGlobal.SquareList!.Values.Count(items => items.Player == _mainGame.SaveRoot!.PlayOrder.WhoTurn);
                if (output == 0)
                    throw new BasicBlankException("Must have at least one point");
                return output; //i think.
            }
        }
        private void GetSavedDot(int row, int column)
        {
            _thisGlobal.PreviousDot = _thisGlobal.DotList!.Values.Single(items => items.Row == row && items.Column == column);
        }
        public void SaveGame()
        {
            SavedBoardData thisData = new SavedBoardData();
            thisData.LineList = _thisGlobal.LineList!.Values.Select(items => items.IsTaken).ToCustomBasicList();
            thisData.DotList = _thisGlobal.DotList!.Values.Select(items => items.IsSelected).ToCustomBasicList();
            thisData.SquarePlayerList = _thisGlobal.SquareList!.Values.Select(items => items.Player).ToCustomBasicList();
            thisData.PreviousColumn = _thisGlobal.PreviousDot.Column;
            thisData.PreviousRow = _thisGlobal.PreviousDot.Row;
            thisData.PreviousLine = _thisGlobal.PreviousLine.Index;
            _mainGame.SaveRoot!.BoardData = thisData;
        }
        public void LoadGame()
        {
            if (_mainGame.DidChooseColors == false)
                return;
            GameBoardGraphicsCP.InitBoard(_thisGlobal); //i think here for sure.
            //not sure if it needed to be somewhere else (?)
            int x = 0;
            _mainGame.SaveRoot!.BoardData!.DotList.ForEach(thisDot =>
            {
                x++;
                _thisGlobal.DotList![x].IsSelected = thisDot;
            });
            x = 0;
            _mainGame.SaveRoot.BoardData.LineList.ForEach(thisLine =>
            {
                x++;
                _thisGlobal.LineList![x].IsTaken = thisLine;
            });
            x = 0;
            _mainGame.SaveRoot.BoardData.SquarePlayerList.ForEach(thisSquare =>
            {
                x++;
                _thisGlobal.SquareList![x].Player = thisSquare;
                if (thisSquare > 0)
                {
                    var tempPlayer = _mainGame.PlayerList![thisSquare];
                    _thisGlobal.SquareList[x].Color = (int)tempPlayer.Color; //hopefully this simple (?)
                }
            });
            if (_mainGame.SaveRoot.BoardData.PreviousColumn > 0 && _mainGame.SaveRoot.BoardData.PreviousRow > 0)
                GetSavedDot(_mainGame.SaveRoot.BoardData.PreviousRow, _mainGame.SaveRoot.BoardData.PreviousColumn);
            else
                _thisGlobal.PreviousDot = new DotInfo();
            if (_mainGame.SaveRoot.BoardData.PreviousLine > 0)
                _thisGlobal.PreviousLine = _thisGlobal.LineList![_mainGame.SaveRoot.BoardData.PreviousLine];
            else
                _thisGlobal.PreviousLine = new LineInfo();
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
            if (_mainGame!.ThisTest!.DoubleCheck == true)
                return false; //try this way.
            CustomBasicList<SquareInfo> winList = new CustomBasicList<SquareInfo>();
            foreach (var thisSquare in _thisGlobal.SquareList!.Values)
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
            foreach (var thisLine in _thisGlobal.LineList!.Values)
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
            DotInfo thisDot = _thisGlobal.DotList![dot];
            if (thisDot.Equals(_thisGlobal.PreviousDot))
                return true; //because undoing move.
            if (_thisGlobal.PreviousDot.Column == 0 && _thisGlobal.PreviousDot.Row == 0)
                return true; //because starting new move.
            bool doesConnect = HasConnectedDot(_thisGlobal.PreviousDot, thisDot);
            if (doesConnect == false)
                return false;
            LineInfo thisLine = GetConnectedLine(_thisGlobal.PreviousDot, thisDot);
            return !thisLine.IsTaken;
        }
        public async Task MakeMoveAsync(int dot)
        {
            DotInfo thisDot = _thisGlobal.DotList![dot];
            if (thisDot.Equals(_thisGlobal.PreviousDot))
            {
                thisDot.IsSelected = false;
                _thisGlobal.PreviousDot = new DotInfo();
                _thisE.RepaintBoard();
                await _mainGame.SaveStateAsync(); //this too.
                await _mainGame.ContinueTurnAsync();
                return;
            }
            if (_thisGlobal.PreviousDot.Column == 0 && _thisGlobal.PreviousDot.Row == 0)
            {
                thisDot.IsSelected = true;
                _thisGlobal.PreviousDot = thisDot;
                _thisE.RepaintBoard();
                await _mainGame.SaveStateAsync(); //this too.
                await _mainGame.ContinueTurnAsync();
                return;
            }
            LineInfo thisLine = GetConnectedLine(_thisGlobal.PreviousDot, thisDot);
            thisLine.IsTaken = true;
            _thisGlobal.PreviousDot.IsSelected = false;
            _thisGlobal.PreviousDot = new DotInfo();
            bool wins = DidWinSquare(_mainGame.SaveRoot!.PlayOrder.WhoTurn, (int)_mainGame.SingleInfo!.Color);
            _thisE.RepaintBoard();
            if (wins == false)
            {
                await _mainGame.EndTurnAsync();
                return;
            }
            int totalPoints = CalculateTotalPoints;
            _mainGame.SingleInfo.Score = totalPoints;
            if (IsGameOver)
            {
                _mainGame.SingleInfo = _mainGame.PlayerList.OrderByDescending(items => items.Score).First();
                await _mainGame.ShowWinAsync();
                return;
            }
            await _mainGame.SaveStateAsync(); //this too.
            await _mainGame.ContinueTurnAsync(); //you get another turn for winning a square.
        }
    }
}