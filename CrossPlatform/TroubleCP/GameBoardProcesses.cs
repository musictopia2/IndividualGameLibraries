using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace TroubleCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly TroubleMainGameClass _mainGame;
        private readonly GameBoardGraphicsCP _graphicsBoard;
        private readonly GlobalVariableClass _thisGlobal;
        private readonly EventAggregator _thisE;
        private System.Collections.Generic.Dictionary<int, SpaceInfo>? _spaceList;
        private CustomBasicList<MoveInfo> MoveList
        {
            get
            {
                return _mainGame.SaveRoot!.MoveList;
            }
            set
            {
                _mainGame.SaveRoot!.MoveList = value;
            }
        }
        private EnumColorChoice OurColor
        {
            get
            {
                return _mainGame.SaveRoot!.OurColor;
            }
            set
            {
                _mainGame.SaveRoot!.OurColor = value;
            }
        }
        public void LoadSavedGame() //done.
        {
            foreach (var thisSpace in _spaceList!.Values)
            {
                thisSpace.Player = 0;
            }
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.PieceList.ForEach(thisPiece =>
                {
                    var thisSpace = _spaceList.Values.Single(items => items.Index == thisPiece);
                    thisSpace.Player = thisPlayer.Id;
                });
            });
        }
        public void LoadBoard() //done.
        {
            _thisGlobal.Animates.LongestTravelTime = 75;
            PopulateSpaces();
        }
        public void Repaint()
        {
            _thisE.RepaintBoard();
        }
        private void PopulateSpaces() //done.
        {
            if (_spaceList != null)
                return;
            _spaceList = new System.Collections.Generic.Dictionary<int, SpaceInfo>();
            int x;
            int y;
            int q = 0;
            int z = 1;
            SpaceInfo thisSpace;
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 4; y++)
                {
                    q += 1;
                    thisSpace = new SpaceInfo();
                    thisSpace.ColorOwner = (EnumColorChoice)z;
                    thisSpace.Index = q;
                    thisSpace.WhatBoard = EnumBoardStatus.IsStart;
                    thisSpace.SpaceNumber = y;
                    _spaceList.Add(q, thisSpace);
                }
                z += 1;
                if (z > 4)
                    z = 1;
            }
            y = 0;
            for (x = 17; x <= 44; x++)
            {
                y += 1;
                thisSpace = new SpaceInfo();
                thisSpace.Index = x;
                if (x == 17)
                    thisSpace.ColorOwner = EnumColorChoice.Green;
                else if (x == 24)
                    thisSpace.ColorOwner = EnumColorChoice.Red;
                else if (x == 31)
                    thisSpace.ColorOwner = EnumColorChoice.Yellow;
                else if (x == 38)
                    thisSpace.ColorOwner = EnumColorChoice.Blue;
                else
                    thisSpace.ColorOwner = EnumColorChoice.None;
                thisSpace.WhatBoard = EnumBoardStatus.OnBoard;
                thisSpace.SpaceNumber = y;
                _spaceList.Add(x, thisSpace);
            }
            q = 44;
            z = 2;
            for (x = 1; x <= 4; x++)
            {
                for (y = 1; y <= 4; y++)
                {
                    q += 1;
                    thisSpace = new SpaceInfo();
                    thisSpace.ColorOwner = (EnumColorChoice)z;
                    thisSpace.SpaceNumber = y;
                    thisSpace.Index = q;
                    thisSpace.WhatBoard = EnumBoardStatus.IsHome;
                    _spaceList.Add(q, thisSpace);
                }
                z += 1;
                if (z > 4)
                    z = 1;
            }
        }
        public void ClearBoard() //done i think.
        {
            _mainGame.PlayerList!.ForEach(thisPlayer => thisPlayer.PieceList.Clear());
            foreach (var thisSpace in _spaceList!.Values)
            {
                if (thisSpace.ColorOwner != EnumColorChoice.None && thisSpace.WhatBoard == EnumBoardStatus.IsStart)
                {
                    thisSpace.Player = FindPlayer(thisSpace.ColorOwner);
                    if (thisSpace.Player > 0)
                    {
                        var tempPlayer = _mainGame.PlayerList[thisSpace.Player];
                        tempPlayer.PieceList.Add(thisSpace.Index);
                    }
                }
                else
                {
                    thisSpace.Player = 0;
                }
            }
            _thisE.RepaintBoard();
        }
        private int FindPlayer(EnumColorChoice thisColor) //done.
        {
            var thisPlayer = _mainGame.PlayerList.SingleOrDefault(items => items.Color == thisColor);
            if (thisPlayer == null)
                return 0;
            return thisPlayer.Id;
        }
        public void StartTurn() //done.
        {
            _mainGame.SaveRoot!.DiceNumber = 0;
            OurColor = _mainGame.SingleInfo!.Color;
            _thisGlobal.MovePlayer = 0;
            _mainGame.ThisMod!.ThisCup!.CanShowDice = false;
            _thisE.RepaintBoard();
        }
        public GameBoardProcesses(TroubleMainGameClass mainGame, GameBoardGraphicsCP graphicsBoard, GlobalVariableClass thisGlobal, EventAggregator thisE) //done.
        {
            _mainGame = mainGame;
            _graphicsBoard = graphicsBoard;
            _thisGlobal = thisGlobal;
            _thisE = thisE;
        }
        public bool IsValidMove(int index) //done now.
        {
            return MoveList.Any(items => items.SpaceFrom == index);
        }
        private async Task NoMovesEndTurnAsync() //done.
        {
            _mainGame.SaveRoot!.Instructions = "No moves.  End turn.";
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            await _mainGame.EndTurnAsync();
        }
        private async Task DoAnotherTurnAsync() //done.
        {
            _mainGame.SaveRoot!.Instructions = "Will get another turn for rolling a 6";
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            await _mainGame.StartNewTurnAsync();
        }
        private async Task DoEndTurnAsync() //done
        {
            _mainGame.SaveRoot!.Instructions = "Move was made successfully";
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            await _mainGame.EndTurnAsync();
        }
        private int WhenToGoHome //done.
        {
            get
            {
                int nums;
                if (OurColor == EnumColorChoice.Blue)
                    nums = 37;
                else if (OurColor == EnumColorChoice.Green)
                    nums = 44;
                else if (OurColor == EnumColorChoice.Red)
                    nums = 23;
                else if (OurColor == EnumColorChoice.Yellow)
                    nums = 30;
                else
                    throw new BasicBlankException("Need a color for the player");
                return _spaceList!.Values.Single(items => items.Index == nums).SpaceNumber;
            }
        }
        private int FindStartSpace(int whatPlayer) //done i think.
        {
            var tempPlayer = _mainGame.PlayerList![whatPlayer];
            var thisCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.IsStart && items.ColorOwner == tempPlayer.Color && items.Player == 0).ToCustomBasicList();
            if (thisCol.Count == 0)
                throw new BasicBlankException("Could not find the start space");
            return thisCol.First().Index;
        }
        private bool IsGameOver => _spaceList!.Values.Count(items => items.Player == _mainGame.WhoTurn && items.WhatBoard == EnumBoardStatus.IsHome) == 4; //done i think.
        private async Task BackToStartAsync(int oldSpace, int oldPlayer) //done i think.
        {
            int thisStart = FindStartSpace(oldPlayer);
            _thisGlobal.PlayerGoingBack = oldPlayer;
            _thisGlobal.Animates.LocationFrom = _graphicsBoard.LocationOfSpace(oldSpace);
            _thisGlobal.Animates.LocationTo = _graphicsBoard.LocationOfSpace(thisStart);
            var tempPlayer = _mainGame.PlayerList![oldPlayer];
            tempPlayer.PieceList.RemoveSpecificItem(oldSpace);
            await _thisGlobal.Animates.DoAnimateAsync();
            _thisGlobal.PlayerGoingBack = 0;
            var thisSpace = _spaceList![thisStart];
            thisSpace.Player = oldPlayer;
            tempPlayer.PieceList.Add(thisStart); //you are now at start.
            if (tempPlayer.PieceList.Count != 4)
                throw new BasicBlankException("After going back to start, needs 4 pieces");
            _thisE.RepaintBoard();
        }
        private int CalculateSafety(SpaceInfo thisSpace, int lefts, bool willStart) //done i think.
        {
            int newNum;
            if (willStart == false)
            {
                if (thisSpace.WhatBoard != EnumBoardStatus.IsHome)
                    newNum = lefts + 1;
                else
                    newNum = thisSpace.SpaceNumber + lefts;
            }
            else
            {
                newNum = lefts;
            }
            if (newNum == 0)
                throw new BasicBlankException("Cannot be 0");
            if (newNum > 4)
                return 0;
            var newCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.IsHome && items.ColorOwner == OurColor).ToCustomBasicList();
            if (newCol.Count != 4)
                throw new BasicBlankException("Need to have 4 home spaces");
            var thisCol = newCol.Where(items => items.SpaceNumber < newNum).ToCustomBasicList();
            if (thisSpace.WhatBoard == EnumBoardStatus.IsHome)
                thisCol.KeepConditionalItems(items => items.SpaceNumber > thisSpace.SpaceNumber);
            return newCol.First(items => items.SpaceNumber == newNum).Index;
        }
        private int CalculateNewSpace(SpaceInfo thisSpace, int numberToUse) //the rest did copy/paste so should be okay.
        {
            CustomBasicList<SpaceInfo> newCol;
            if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
            {
                newCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard && items.ColorOwner != EnumColorChoice.None).ToCustomBasicList();
                if (newCol.Count != 4)
                    throw new BasicBlankException("Need to have 4 start spaces");
                return newCol.Single(items => items.ColorOwner == OurColor).Index;
            }
            if (thisSpace.WhatBoard == EnumBoardStatus.IsHome)
                return CalculateSafety(thisSpace, numberToUse, false);
            SpaceInfo nextSpace;
            int toHome = WhenToGoHome;
            if (toHome == thisSpace.SpaceNumber)
                return CalculateSafety(thisSpace, numberToUse, true);
            int newNum = thisSpace.SpaceNumber;
            var thisCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard).ToCustomBasicList();
            for (int x = 1; x <= numberToUse; x++)
            {
                newNum++;
                if (newNum == 29)
                    newNum = 1;
                if (newNum == toHome + 1 || newNum == 1 & toHome == 28)
                    return CalculateSafety(thisSpace, numberToUse - x, false);
            }
            nextSpace = thisCol.Single(items => items.SpaceNumber == newNum);
            return nextSpace.Index;
        }
        public async Task GetValidMovesAsync()
        {
            if (_mainGame.SaveRoot!.DiceNumber == 0)
                return; //ignore because did not even roll.
            MoveList = new CustomBasicList<MoveInfo>();
            CustomBasicList<int> firstCol = _mainGame.SingleInfo!.PieceList.ToCustomBasicList();
            if (firstCol.Count != 4)
                throw new BasicBlankException("The player must have 4 pieces");
            if (_mainGame.SaveRoot.DiceNumber < 0 || _mainGame.SaveRoot.DiceNumber > 6)
                throw new BasicBlankException("The dice has to be from 1 to 6");
            CustomBasicList<int> thisCol = new CustomBasicList<int>();
            if (_mainGame.SaveRoot.DiceNumber < 6)
            {
                firstCol.ForEach(thisInt =>
                {
                    if (!_spaceList!.Values.Any(items => items.Index == thisInt && items.WhatBoard == EnumBoardStatus.IsStart))
                        thisCol.Add(thisInt);
                });
            }
            else
            {
                thisCol = firstCol;
            }
            int newPlayer;
            MoveInfo thisMove;
            thisCol.ForEach(thisInt =>
            {
                var thisSpace = _spaceList![thisInt];
                var nextSpace = CalculateNewSpace(thisSpace, _mainGame.SaveRoot.DiceNumber);
                if (nextSpace > 0)
                {
                    var newSpace = _spaceList[nextSpace];
                    if (newSpace.ColorOwner != EnumColorChoice.None)
                        newPlayer = FindPlayer(newSpace.ColorOwner);
                    else
                        newPlayer = 0;
                    if (newSpace.Player != thisSpace.Player || newSpace.Player == 0) //you can jump your own on this one.
                    {
                        thisMove = new MoveInfo();
                        thisMove.SpaceFrom = thisSpace.Index;
                        thisMove.SpaceTo = newSpace.Index;
                        MoveList.Add(thisMove);
                    }

                }
            });
            if (MoveList.Count == 0)
            {
                if (_mainGame.SaveRoot.DiceNumber == 6)
                {
                    _mainGame.SaveRoot.Instructions = "No moves.  Will get another turn.";
                    if (_mainGame.ThisTest!.NoAnimations == false)
                        await _mainGame.Delay!.DelaySeconds(.5);
                    await _mainGame.StartNewTurnAsync(); //i think
                    return;
                }
                await NoMovesEndTurnAsync();
                return;
            }
            int ourID = 0;
            if (_mainGame.ThisData!.MultiPlayer)
                ourID = _mainGame.PlayerList!.GetSelf().Id;
            if (_mainGame.ThisData.MultiPlayer == false || _mainGame.WhoTurn != ourID)
                _mainGame.SaveRoot.Instructions = $"Waiting for {_mainGame.SingleInfo.NickName} to make a move";
            else
                _mainGame.SaveRoot.Instructions = "Choose a piece to move";
            await _mainGame.ContinueTurnAsync();
        }
        public async Task MakeMoveAsync(int space)
        {
            if (!MoveList.Any(items => items.SpaceFrom == space) && space == 0)
                throw new BasicBlankException($"Space {space} was not found on the movelist");
            var thisSpace = _spaceList![space];
            _thisGlobal.MovePlayer = 0;
            _thisE.RepaintBoard();
            MoveInfo thisMove;
            thisMove = MoveList.Single(items => items.SpaceFrom == space);
            var newSpace = _spaceList[thisMove.SpaceTo];
            thisSpace.Player = 0;
            int oldPlayer = newSpace.Player;
            _mainGame.SingleInfo!.PieceList.RemoveSpecificItem(thisMove.SpaceFrom);
            if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
            {
                _mainGame.SingleInfo.PieceList.Add(thisMove.SpaceTo);
                if (newSpace.Player > 0)
                {
                    var otherPlayer = _mainGame.PlayerList![oldPlayer];
                    newSpace.Player = _mainGame.WhoTurn;
                    await BackToStartAsync(thisMove.SpaceTo, oldPlayer);
                }
                else
                {
                    newSpace.Player = _mainGame.WhoTurn;
                    _thisE.RepaintBoard();
                }
                if (_mainGame.SaveRoot!.DiceNumber < 6)
                    await DoEndTurnAsync();
                else
                    await DoAnotherTurnAsync();
                return;
            }
            _thisE.RepaintBoard();
            int nextMove;
            for (int x = 1; x <= _mainGame.SaveRoot!.DiceNumber; x++)
            {
                nextMove = CalculateNewSpace(thisSpace, x);

                _thisGlobal.MovePlayer = nextMove;
                _thisE.RepaintBoard();
                if (_mainGame.ThisTest!.NoAnimations == false)
                    await _mainGame.Delay!.DelaySeconds(.1);
            }
            _thisGlobal.MovePlayer = 0;
            _mainGame.SingleInfo.PieceList.Add(newSpace.Index);
            _thisE.RepaintBoard();
            if (newSpace.Player != 0)
            {
                newSpace.Player = _mainGame.WhoTurn;
                await BackToStartAsync(newSpace.Index, oldPlayer);
            }
            else
            {
                newSpace.Player = _mainGame.WhoTurn;
            }
            if (IsGameOver)
            {
                await _mainGame.ShowWinAsync();
                return;
            }
            if (_mainGame.SaveRoot.DiceNumber == 6)
            {
                await DoAnotherTurnAsync();
                return;
            }
            await DoEndTurnAsync();
        }
    }
}