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
using BasicGameFrameworkLibrary.Attributes;
using MancalaCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.CommonInterfaces;
//i think this is the most common things i like to do
namespace MancalaCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {
        private bool _didReverse;
        private PlayerPieceData? _currentPiece;
        private MancalaPlayerItem? _currentPlayer;
        private readonly IEventAggregator _aggregator;
        private readonly MancalaVMData _vmdata;
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly IAsyncDelayer _delayer;

        public GameBoardProcesses(IEventAggregator aggregator,
            MancalaVMData vmdata,
            BasicData basicData,
            TestOptions test,
            IAsyncDelayer delayer
            )
        {
            _aggregator = aggregator;
            _vmdata = vmdata;
            _basicData = basicData;
            _test = test;
            _delayer = delayer;
        }
        #region "delegates"
        //this is to stop the overflows.  this is what the main game process has to populate.  if i sent in the game, then overflow since the game needs this one too.
        public Func<PlayerCollection<MancalaPlayerItem>>? PlayerList { get; set; }
        public Func<MancalaPlayerItem>? SingleInfo { get; set; }
        public Action<MancalaPlayerItem>? SetCurrentPlayer { get; set; }
        public Func<Task>? EndTurnAsync { get; set; }
        public Func<int>? WhoTurn { get; set; }
        public Func<Task>? ContinueTurnAsync { get; set; }
        public Func<Task>? ShowTieAsync { get; set; }
        public Func<Task>? ShowWinAsync { get; set; }
        public Func<MancalaSaveInfo>? SaveRoot { get; set; }
        #endregion
        public void RepaintBoard()
        {
            _aggregator.RepaintBoard();
        }

        public void LoadSavedBoard()
        {
            PopulateBoard();
            RepaintBoard();
        }
        private void PopulateBoard()
        {
            int x = 0;
            bool wasReversed;
            int index;
            int counts2 = _vmdata.SpaceList!.Values.Sum(items => items.Pieces);
            if (counts2 != 48 && counts2 > 0)
                throw new BasicBlankException($"Count Of {counts2} does not reconcile with 48 Part 4");
            int counts1 = PlayerList!.Invoke().First().ObjectList.Sum(Items => Items.HowManyPieces) + PlayerList.Invoke().First().HowManyPiecesAtHome;
            counts2 = PlayerList!.Invoke().Last().ObjectList.Sum(Items => Items.HowManyPieces) + PlayerList.Invoke().Last().HowManyPiecesAtHome;
            int Totals = counts1 + counts2;
            if (Totals != 48)
                throw new BasicBlankException($"Count Of {Totals} does not reconcile with 48 part 7");
            foreach (var Space in _vmdata.SpaceList.Values)
            {
                Space.Pieces = 0;
            }
            PlayerList!.Invoke().ForEach(thisPlayer =>
            {
                x++;
                wasReversed = NeedsReversed(x);
                if (wasReversed == false)
                    index = 7;
                else
                    index = 14;
                if (index > 0)
                {
                    var tempSpace = _vmdata.SpaceList[index];
                    tempSpace.Pieces = thisPlayer.HowManyPiecesAtHome;
                }
                thisPlayer.ObjectList.ForEach(ThisObject =>
                {
                    if (wasReversed == false)
                        index = ThisObject.Index;
                    else
                        index = ThisObject.Index + 7;
                    var TempSpace = _vmdata.SpaceList[index];
                    TempSpace.Pieces = ThisObject.HowManyPieces;
                });
            });
            int counts = _vmdata.SpaceList.Values.Sum(Items => Items.Pieces);
            if (counts != 48)
                throw new BasicBlankException($"Count Of {counts} does not reconcile with 48 Part 5");
        }
        public void ClearBoard()
        {
            PlayerList!.Invoke().ForEach(thisPlayer =>
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
            RepaintBoard();
        }
        internal void Reset()
        {
            _vmdata.SpaceSelected = 0;
            _vmdata.SpaceStarted = 0;
        }
        public async Task StartNewTurnAsync()
        {
            SaveRoot!.Invoke().IsStart = false; //not anymore.
            PopulateBoard();
            _vmdata.PiecesAtStart = 0;
            _vmdata.PiecesLeft = 0;
            Reset();
            await ContinueTurnAsync!.Invoke();
        }
        private bool NeedsReversed(int playerConsidered)
        {
            if (_basicData.MultiPlayer == true)
            {
                var tempPlayer = PlayerList!.Invoke()[playerConsidered];
                if (tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman)
                    return true;
                return false;
            }
            if (WhoTurn!.Invoke() == playerConsidered)
                return false;
            return true;
        }
        public async Task AnimateMoveAsync(int index)
        {
            _didReverse = NeedsReversed(WhoTurn!.Invoke());
            _vmdata.SpaceStarted = index;
            int nums;
            int whatNum;
            whatNum = index;
            var thisSpace = _vmdata.SpaceList![index]; // i think
            nums = thisSpace.Pieces;
            if (nums == 0)
                throw new BasicBlankException("Can't have 0 when animating move.");
            _vmdata.PiecesAtStart = nums;
            thisSpace.Pieces = 0;
            if (_currentPiece == null == true)
            {
                if (_didReverse == false)
                    _currentPiece = (from x in SingleInfo!.Invoke().ObjectList
                                     where x.Index == index
                                     select x).Single();
                else
                    _currentPiece = (from x in SingleInfo!.Invoke().ObjectList
                                     where x.Index == (index - 7)
                                     select x).Single();
                _currentPlayer = SingleInfo.Invoke();
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
                    _vmdata.PiecesLeft = nums;
                    if (_test.NoAnimations == false)
                        await _delayer.DelayMilli(400);
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
            _vmdata.SpaceSelected = index;
            SpaceInfo thisSpace;
            thisSpace = _vmdata.SpaceList![index];
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
                    _currentPlayer = SingleInfo!.Invoke();
                else
                {
                    if (WhoTurn!.Invoke() == 1)
                        _currentPlayer = PlayerList!.Invoke()[2];
                    else
                        _currentPlayer = PlayerList!.Invoke()[1];
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
            RepaintBoard();
        }
        internal void TransferBeadsToPlayers()
        {
            if (_vmdata.SpaceList!.Values.Sum(Items => Items.Pieces) != 48)
                throw new BasicBlankException("Does not reconcile before transferring beads to players");
            PlayerList!.Invoke().ForEach(ThisPlayer =>
            {
                bool isReversed = NeedsReversed(ThisPlayer.Id);
                ThisPlayer.ObjectList.Clear();
                if (isReversed == false)
                    ThisPlayer.HowManyPiecesAtHome = _vmdata.SpaceList[7].Pieces;
                else
                    ThisPlayer.HowManyPiecesAtHome = _vmdata.SpaceList[14].Pieces;
                for (int i = 1; i <= 6; i++)
                {
                    int index;
                    if (isReversed == false)
                        index = i;
                    else
                        index = i + 7;
                    if (_vmdata.SpaceList[index].Pieces > 0)
                    {
                        PlayerPieceData thisPiece = new PlayerPieceData();
                        thisPiece.HowManyPieces = _vmdata.SpaceList[index].Pieces;
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
            thisSpace = _vmdata.SpaceList![index];
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
            thisSpace = _vmdata.SpaceList[nums];
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
                await EndTurnAsync!.Invoke();
                return;
            }
            _currentPiece = null; // because you have to make another move
            await ContinueTurnAsync!.Invoke();
        }
        private async Task GameOverProcessesAsync()
        {
            _vmdata.Instructions = "";
            if (PlayerList!.Invoke().First().HowManyPiecesAtHome == PlayerList!.Invoke().Last().HowManyPiecesAtHome)
            {
                await ShowTieAsync!.Invoke();
                return;
            }
            SetCurrentPlayer!(PlayerList.Invoke().OrderByDescending(Items => Items.HowManyPiecesAtHome).First());
            await ShowWinAsync!.Invoke();
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
                thisSpace = _vmdata.SpaceList![x];
                if (thisSpace.Pieces > 0)
                    return true;
            }
            return false;
        }
    }
}
