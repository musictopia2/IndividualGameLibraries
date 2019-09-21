using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP
{
    [SingletonGame]
    public class GameBoardProcesses : ISpacePosition
    {
        private readonly EventAggregator _thisE;
        private GameBoardGraphicsCP? _graphicsBoard;
        private readonly LifeBoardGameMainGameClass _mainGame;
        private readonly GlobalVariableClass _thisGlobal;
        private readonly CustomBasicList<ShortcutInfo> _shortCutList = new CustomBasicList<ShortcutInfo>();
        private readonly CustomBasicList<int> _stops = new CustomBasicList<int>();
        public GameBoardProcesses(LifeBoardGameMainGameClass mainGame, GlobalVariableClass thisGlobal, EventAggregator thisE)
        {
            _mainGame = mainGame;
            _thisGlobal = thisGlobal;
            _stops = new CustomBasicList<int>() { 25, 36, 81, 147 };
            _shortCutList = new CustomBasicList<ShortcutInfo>
            {
                new ShortcutInfo() { FromSpace = 91, ToSpace = 96 },
                new ShortcutInfo() { FromSpace = 55, ToSpace = 62 },
                new ShortcutInfo() { FromSpace = 11, ToSpace = 15 }
            };
            _thisE = thisE;
            thisGlobal.Pos = new PositionPieces();
        }
        public int NewPosition
        {
            get
            {
                return _mainGame.SaveRoot!.NewPosition;
            }
            set
            {
                _mainGame.SaveRoot!.NewPosition = value;
            }
        }
        public int NumberRolled
        {
            get
            {
                return _mainGame.SaveRoot!.NumberRolled;
            }
            set
            {
                _mainGame.SaveRoot!.NumberRolled = value;
            }
        }
        public int PayDaysPassed { get; set; }
        public bool CheckMove(int positionClicked) // may need it in case.
        {
            if (_mainGame.SingleInfo!.OptionChosen == EnumStart.None)
                return false;
            if (positionClicked == 0)
                return false;
            if (positionClicked == FirstPossiblePosition)
                return true;
            if (positionClicked == SecondPossiblePosition)
                return true;
            return false;
        }
        private void Init()
        {
            if (_graphicsBoard != null)
                return;
            _graphicsBoard = _mainGame.MainContainer.Resolve<GameBoardGraphicsCP>(); //to stop the overflow errors.
        }
        private void RepaintBoard()
        {
            _thisE.RepaintBoard(); //i think.   
        }
        public EnumViewCategory GetViewForPosition(int currentPosition)
        {
            if (currentPosition <= 36)
                return EnumViewCategory.StartGame;
            else if (currentPosition <= 61)
                return EnumViewCategory.SpinAfterHouse;
            else if (currentPosition <= 120)
                return EnumViewCategory.AfterFirstSetChoices;
            else
                return EnumViewCategory.EndGame;
        }
        public EnumViewCategory GetViewForPlayer
        {
            get
            {
                if (_thisGlobal.GameStatus == EnumWhatStatus.NeedChooseFirstOption)
                    return EnumViewCategory.StartGame;
                if (_mainGame.SingleInfo!.LastMove != EnumFinal.None)
                    return EnumViewCategory.EndGame;
                return GetViewForPosition(_mainGame.SingleInfo.Position);
            }
        }
        public int PlayerInLead
        {
            get
            {
                var tempList = _mainGame.PlayerList.OrderByDescending(items => items.Distance).Take(2).ToCustomBasicList();
                if (tempList.First().Distance == tempList.Last().Distance)
                    return 0;
                return tempList.First().Id;
            }
        }
        public async Task AnimateMoveAsync(bool useSecond)
        {
            Init();
            PayDaysPassed = 0;
            int currentPosition = _mainGame.SingleInfo!.Position;
            if (currentPosition == 147 || NewPosition == 0)
                return;
            if (NewPosition > 147)
                throw new BasicBlankException("The highest supported for new position is 147");
            SpaceInfo thisSpace;
            _thisGlobal.GameStatus = EnumWhatStatus.MakingMove;
            _graphicsBoard!.GoingTo = NewPosition;
            int x = 0;
            _thisGlobal.CurrentSelected = 0;
            do
            {
                x++;
                if (x > 16)
                    throw new BasicBlankException("Can't move more than 16 spaces.  This means its hosed."); //if testing, rethink
                if (currentPosition == 0 && _mainGame.SingleInfo.OptionChosen == EnumStart.Career)
                    currentPosition = 12;
                else if (currentPosition == 0 && _mainGame.SingleInfo.OptionChosen == EnumStart.College)
                    currentPosition = 1;
                else if (currentPosition == 0)
                    throw new BasicBlankException("Current position cannot be 0 if not choosing college or career");
                else if (currentPosition == 48 && useSecond)
                    currentPosition = 56;
                else if (currentPosition == 84 && useSecond)
                    currentPosition = 92;
                else if (x == 1 && _stops.Any(items => items == currentPosition))
                    currentPosition++;
                else
                    currentPosition = NextAvailableSpace(currentPosition);
                _mainGame.SingleInfo.Distance++;
                thisSpace = _thisGlobal.SpaceList![currentPosition - 1]; //because 0 based;
                if (thisSpace.ActionInfo == EnumActionType.GetPaid)
                    PayDaysPassed++;
                _mainGame.SingleInfo.Position = currentPosition;
                _thisGlobal.CurrentView = GetViewForPosition(currentPosition);
                RepaintBoard();
                if (_mainGame.ThisTest!.NoAnimations == false)
                    await _mainGame.Delay!.DelaySeconds(.1);
                if (currentPosition == NewPosition)
                {
                    _graphicsBoard.GoingTo = 0;
                    RepaintBoard();
                    return;
                }
            } while (true);

        }
        private bool SomeoneOnSpace(int currentPosition)
        {
            return _mainGame.PlayerList.Any(items => items.Position == currentPosition);
        }
        private int NextAvailableSpace(int currentPosition)
        {
            do
            {
                if (_stops.Any(items => items == currentPosition))
                    return currentPosition;
                if (SomeoneOnSpace(currentPosition) == false)
                    return currentPosition;
                currentPosition = PositionNext(currentPosition);
            } while (true);
        }
        private int PositionNext(int currentPosition)
        {
            if (!_shortCutList.Any(items => items.FromSpace == currentPosition))
                return currentPosition + 1;
            return (_shortCutList.Where(items => items.FromSpace == currentPosition).Select(items => items.ToSpace).Single());
        }
        public int FirstPossiblePosition //done.
        {
            get
            {
                if (_mainGame.SingleInfo!.OptionChosen == EnumStart.None)
                    return 0;
                if (_mainGame.SingleInfo.Position == 147)
                    return 0;
                int currentPosition;
                if (_mainGame.SingleInfo.OptionChosen == EnumStart.Career && _mainGame.SingleInfo.Position < 15)
                {
                    currentPosition = _mainGame.SingleInfo.Position;
                    var loopTo = (12 + NumberRolled) - 1;
                    for (var x = 12; x <= loopTo; x++)
                    {
                        if (currentPosition == 0)
                            currentPosition = 12;
                        else
                            currentPosition += 1;
                    }
                    currentPosition = NextAvailableSpace(currentPosition);
                    return currentPosition;
                }
                if (_mainGame.SingleInfo.Position == 11)
                {
                    currentPosition = 14;
                    var loopTo1 = NumberRolled;
                    for (var x = 1; x <= loopTo1; x++)
                        currentPosition += 1;
                    currentPosition = NextAvailableSpace(currentPosition);
                    return currentPosition;
                }
                if (_mainGame.SingleInfo.Position < 15)
                {
                    currentPosition = _mainGame.SingleInfo.Position;
                    var loopTo2 = NumberRolled;
                    for (var x = 1; x <= loopTo2; x++)
                    {
                        currentPosition += 1;
                        if (currentPosition == 11)
                            return 11;
                    }
                    currentPosition = NextAvailableSpace(currentPosition);
                    return currentPosition;
                }
                currentPosition = _mainGame.SingleInfo.Position;
                var loopTo3 = NumberRolled;
                for (var x = 1; x <= loopTo3; x++)
                {
                    currentPosition = PositionNext(currentPosition);
                    if (_stops.Any(items => items == currentPosition))
                        return currentPosition; //because of stop.
                }
                return NextAvailableSpace(currentPosition);
            }
        }
        public int SecondPossiblePosition //done.
        {
            get
            {
                if (_mainGame.SingleInfo!.OptionChosen == EnumStart.None)
                    return 0;
                if (_mainGame.SingleInfo.Position == 147)
                    return 0;
                bool hasShortcut = false;
                var loopTo = NumberRolled;
                int currentPosition = _mainGame.SingleInfo.Position;
                for (int x = 1; x <= loopTo; x++)
                {
                    int newNum;
                    if (!_shortCutList.Any(items => items.FromSpace == currentPosition))
                        newNum = 0;
                    else
                        newNum = _shortCutList.Where(items => items.FromSpace == currentPosition).Select(items => items.ToSpace).Single();
                    if (currentPosition == 48)
                    {
                        hasShortcut = true;
                        currentPosition = 56;
                    }
                    else if (currentPosition == 84)
                    {
                        hasShortcut = true;
                        currentPosition = 92;
                    }
                    else if (currentPosition == 148)
                        return 0;
                    else
                        currentPosition = PositionNext(currentPosition);
                    if (_stops.Any(items => items == currentPosition))
                        return 0; //because of stop.
                }
                if (hasShortcut == false)
                    return 0;
                return NextAvailableSpace(currentPosition);
            }
        }
        public void LoadSavedGame()
        {
            _thisGlobal.CurrentView = GetViewForPlayer;
            RepaintBoard();
        }
        public void NewTurn()
        {
            _thisGlobal.CurrentView = GetViewForPlayer;
            _thisGlobal.CurrentSelected = 0;
            RepaintBoard();
        }
        public void ResetBoard()
        {
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Distance = 0;
                thisPlayer.OptionChosen = EnumStart.None;
                thisPlayer.LastMove = EnumFinal.None;
                thisPlayer.Position = 0;
                thisPlayer.Gender = EnumGender.None;
                thisPlayer.CarIsInsured = false;
                thisPlayer.DegreeObtained = false;
                thisPlayer.FirstStock = 0;
                thisPlayer.SecondStock = 0;
                thisPlayer.Hand.Clear();
                thisPlayer.HouseIsInsured = false;
                thisPlayer.Loans = 0;
                thisPlayer.MoneyEarned = 10000;
                thisPlayer.Salary = 0; //forgot that too.
                thisPlayer.HouseName = "";
                thisPlayer.WhatTurn = EnumTurnInfo.NormalTurn;
                thisPlayer.TilesCollected = 0;
                thisPlayer.TileList.Clear();
            });
        }
    }
}