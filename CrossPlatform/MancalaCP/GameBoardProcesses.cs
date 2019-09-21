using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MancalaCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private bool _didReverse;
        private PlayerPieceData? _currentPiece;
        private MancalaPlayerItem? _currentPlayer;
        readonly MancalaMainGameClass _mainGame;
        public GameBoardProcesses(MancalaMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public void LoadSavedBoard()
        {
            PopulateBoard();
            _mainGame.RepaintBoard();
        }
        private void PopulateBoard()
        {
            int x = 0;
            bool wasReversed;
            int index;
            int counts2 = _mainGame.SpaceList!.Values.Sum(items => items.Pieces);
            if (counts2 != 48 && counts2 > 0)
                throw new BasicBlankException($"Count Of {counts2} does not reconcile with 48 Part 4");
            int counts1 = _mainGame.PlayerList.First().ObjectList.Sum(Items => Items.HowManyPieces) + _mainGame.PlayerList.First().HowManyPiecesAtHome;
            counts2 = _mainGame.PlayerList.Last().ObjectList.Sum(Items => Items.HowManyPieces) + _mainGame.PlayerList.Last().HowManyPiecesAtHome;
            int Totals = counts1 + counts2;
            if (Totals != 48)
                throw new BasicBlankException($"Count Of {Totals} does not reconcile with 48 part 7");
            foreach (var Space in _mainGame.SpaceList.Values)
            {
                Space.Pieces = 0;
            }
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                x++;
                wasReversed = NeedsReversed(x);
                if (wasReversed == false)
                    index = 7;
                else
                    index = 14;
                if (index > 0)
                {
                    var tempSpace = _mainGame.SpaceList[index];
                    tempSpace.Pieces = thisPlayer.HowManyPiecesAtHome;
                }
                thisPlayer.ObjectList.ForEach(ThisObject =>
                {
                    if (wasReversed == false)
                        index = ThisObject.Index;
                    else
                        index = ThisObject.Index + 7;
                    var TempSpace = _mainGame.SpaceList[index];
                    TempSpace.Pieces = ThisObject.HowManyPieces;
                });
            });
            int counts = _mainGame.SpaceList.Values.Sum(Items => Items.Pieces);
            if (counts != 48)
                throw new BasicBlankException($"Count Of {counts} does not reconcile with 48 Part 5");
        }
        public void ClearBoard()
        {
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.HowManyPiecesAtHome = 0;
                thisPlayer.ObjectList = new CustomBasicList<PlayerPieceData>();
                int x;
                for (x = 1; x <= 6; x++)
                {
                    PlayerPieceData thisData = new PlayerPieceData();
                    thisData.Index = x;
                    thisData.HowManyPieces = 4;
                    thisPlayer.ObjectList.Add(thisData);
                }
            });
            PopulateBoard();
            _mainGame.RepaintBoard();
        }
        internal void Reset()
        {
            _mainGame.SpaceSelected = 0;
            _mainGame.SpaceStarted = 0;
        }
        public async Task StartNewTurnAsync()
        {
            _mainGame.SaveRoot!.IsStart = false; //not anymore.
            PopulateBoard();
            _mainGame.ThisMod!.PiecesAtStart = 0;
            _mainGame.ThisMod.PiecesLeft = 0;
            Reset();
            await _mainGame.ContinueTurnAsync();
        }
        private bool NeedsReversed(int playerConsidered)
        {
            if (_mainGame.ThisData!.MultiPlayer == true)
            {
                var tempPlayer = _mainGame.PlayerList![playerConsidered];
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman)
                    return true;
                return false;
            }
            if (_mainGame.WhoTurn == playerConsidered)
                return false;
            return true;
        }
        public async Task AnimateMoveAsync(int index)
        {
            _didReverse = NeedsReversed(_mainGame.WhoTurn);
            _mainGame.SpaceStarted = index;
            int nums;
            int whatNum;
            whatNum = index;
            var thisSpace = _mainGame.SpaceList![index]; // i think
            nums = thisSpace.Pieces;
            if (nums == 0)
                throw new BasicBlankException("Can't have 0 when animating move.");
            _mainGame.ThisMod!.PiecesAtStart = nums;
            thisSpace.Pieces = 0;
            if (_currentPiece == null == true)
            {
                if (_didReverse == false)
                    _currentPiece = (from Items in _mainGame.SingleInfo!.ObjectList
                                     where Items.Index == index
                                     select Items).Single();
                else
                    _currentPiece = (from Items in _mainGame.SingleInfo!.ObjectList
                                     where Items.Index == (index - 7)
                                     select Items).Single();
                _currentPlayer = _mainGame.SingleInfo;
            }
            _currentPlayer!.ObjectList.RemoveSpecificItem(_currentPiece!);
            _currentPiece = null; // something else has to come along to fill that piece now.
            _currentPlayer = null;
            do
            {
                whatNum += 1;
                if (whatNum > 14)
                    whatNum = 1;
                if (CanProcess(whatNum) == true)
                {
                    ProcessMove(whatNum);
                    nums -= 1;
                    _mainGame.ThisMod.PiecesLeft = nums;
                    if (_mainGame!.ThisTest!.NoAnimations == false)
                        await _mainGame.Delay!.DelayMilli(400);
                    if (nums == 0)
                    {
                        await EndProcessesAsync(whatNum);
                        return;
                    }
                }
            }
            while (true); // well see how this goes.  maybe half a second (?)
        }
        private void ProcessMove(int index)
        {
            _mainGame.SpaceSelected = index;
            SpaceInfo thisSpace;
            thisSpace = _mainGame.SpaceList![index];
            bool isOpponent;
            if (index != 14 && index != 7)
            {
                int yourSpace;
                if (_didReverse == false)
                {
                    yourSpace = index;
                    if (yourSpace > 6)
                    {
                        yourSpace = index - 7;
                        isOpponent = true; // forgot this part.
                    }
                    else
                        isOpponent = false;
                }
                else
                {
                    yourSpace = index - 7;
                    if (yourSpace < 0)
                    {
                        yourSpace = index;
                        isOpponent = true;
                    }
                    else
                        isOpponent = false;
                }
                if (yourSpace < 1 || yourSpace > 6)
                    throw new BasicBlankException("The space must be 1 to 6, not " + yourSpace);
                if (isOpponent == false)
                    _currentPlayer = _mainGame.SingleInfo;
                else
                {
                    if (_mainGame.WhoTurn == 1)
                        _currentPlayer = _mainGame.PlayerList![2];
                    else
                        _currentPlayer = _mainGame.PlayerList![1];
                }
                _currentPiece = (from items in _currentPlayer!.ObjectList
                                 where items.Index == yourSpace
                                 select items).SingleOrDefault();
                if (_currentPiece == null == true)
                {
                    _currentPiece = new PlayerPieceData();
                    _currentPiece.Index = yourSpace; // i forgot this part.
                    _currentPlayer.ObjectList.Add(_currentPiece);
                }
            }
            thisSpace.Pieces += 1; //decided to transfer after its done.  because sometimes it got hosed.
            _mainGame.RepaintBoard();
        }
        internal void TransferBeadsToPlayers()
        {
            if (_mainGame.SpaceList!.Values.Sum(Items => Items.Pieces) != 48)
                throw new BasicBlankException("Does not reconcile before transferring beads to players");
            _mainGame.PlayerList!.ForEach(ThisPlayer =>
            {
                bool isReversed = NeedsReversed(ThisPlayer.Id);
                ThisPlayer.ObjectList.Clear();
                if (isReversed == false)
                    ThisPlayer.HowManyPiecesAtHome = _mainGame.SpaceList[7].Pieces;
                else
                    ThisPlayer.HowManyPiecesAtHome = _mainGame.SpaceList[14].Pieces;
                for (int i = 1; i <= 6; i++)
                {
                    int index;
                    if (isReversed == false)
                        index = i;
                    else
                        index = i + 7;
                    if (_mainGame.SpaceList[index].Pieces > 0)
                    {
                        PlayerPieceData thisPiece = new PlayerPieceData();
                        thisPiece.HowManyPieces = _mainGame.SpaceList[index].Pieces;
                        thisPiece.Index = i; //i think.
                        ThisPlayer.ObjectList.Add(thisPiece);
                    }
                }
            });
        }
        private async Task EndProcessesAsync(int index)
        {
            if (index == 7 || index == 14)
            {
                await ContinueProcessesAsync(false);
                return;
            }
            SpaceInfo thisSpace;
            thisSpace = _mainGame.SpaceList![index];
            if (thisSpace.Pieces > 1)
            {
                await AnimateMoveAsync(index);
                return;
            }
            if (IsOnOwnSide(index) == false)
            {
                await ContinueProcessesAsync(true);
                return;
            }
            int nums;
            nums = 14 - index;
            nums = Math.Abs(nums);
            thisSpace = _mainGame.SpaceList[nums];
            if (thisSpace.Pieces == 0)
            {
                await ContinueProcessesAsync(true);
                return;
            }
            await AnimateMoveAsync(nums);
        }
        private async Task ContinueProcessesAsync(bool endTurn)
        {
            TransferBeadsToPlayers();
            if (AnyOnSide(true) == false)
            {
                await GameOverProcessesAsync();
                return;
            }
            if (AnyOnSide(false) == false)
            {
                await GameOverProcessesAsync();
                return;
            }
            if (endTurn == true)
            {
                _currentPiece = null;
                await _mainGame.EndTurnAsync();
                return;
            }
            _currentPiece = null; // because you have to make another move
            await _mainGame.ContinueTurnAsync();
        }
        private async Task GameOverProcessesAsync()
        {
            _mainGame.ThisMod!.Instructions = "";
            if (_mainGame.PlayerList.First().HowManyPiecesAtHome == _mainGame.PlayerList.Last().HowManyPiecesAtHome)
            {
                await _mainGame.ShowTieAsync();
                return;
            }
            _mainGame.SingleInfo = _mainGame.PlayerList.OrderByDescending(Items => Items.HowManyPiecesAtHome).First();
            await _mainGame.ShowWinAsync();
        }
        private bool CanProcess(int whatNum)
        {
            if (_didReverse == false)
            {
                if (whatNum == 14)
                    return false;
                return true;
            }
            if (whatNum == 7)
                return false;
            return true;
        }
        private bool IsOnOwnSide(int index)
        {
            if (_didReverse == false && index <= 6)
                return true;
            if (_didReverse == true && index > 7)
                return true;
            return false;
        }
        private bool AnyOnSide(bool firstSide)
        {
            int y;
            if (firstSide == true)
                y = 1;
            else
                y = 8;
            int x;
            SpaceInfo thisSpace;
            var loopTo = y + 5;
            for (x = y; x <= loopTo; x++)
            {
                thisSpace = _mainGame.SpaceList![x];
                if (thisSpace.Pieces > 0)
                    return true;
            }
            return false;
        }
    }
}