using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TroubleCP.Data;
using TroubleCP.Graphics;

namespace TroubleCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {

        private readonly TroubleGameContainer _gameContainer;
        private readonly TroubleVMData _model;
        private readonly GameBoardGraphicsCP _graphicsCP;

        public GameBoardProcesses(TroubleGameContainer gameContainer, TroubleVMData model, GameBoardGraphicsCP graphicsCP)
        {
            _gameContainer = gameContainer;
            _model = model;
            _graphicsCP = graphicsCP;
            LoadBoard();
            _gameContainer.IsValidMove = IsValidMove;
        }

        private Dictionary<int, SpaceInfo>? _spaceList;
        private CustomBasicList<MoveInfo> MoveList
        {
            get
            {
                return _gameContainer.SaveRoot!.MoveList;
            }
            set
            {
                _gameContainer.SaveRoot!.MoveList = value;
            }
        }
        private EnumColorChoice OurColor
        {
            get
            {
                return _gameContainer.SaveRoot!.OurColor;
            }
            set
            {
                _gameContainer.SaveRoot!.OurColor = value;
            }
        }
        public void LoadSavedGame() //done.
        {
            foreach (var thisSpace in _spaceList!.Values)
                thisSpace.Player = 0;
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
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
            _gameContainer.Animates.LongestTravelTime = 75;
            PopulateSpaces();
        }
        public void Repaint()
        {
            _gameContainer.Aggregator.RepaintBoard();
        }
        private void PopulateSpaces() //done.
        {
            if (_spaceList != null)
                return;
            _spaceList = new Dictionary<int, SpaceInfo>();
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
            _gameContainer.PlayerList!.ForEach(thisPlayer => thisPlayer.PieceList.Clear());
            foreach (var thisSpace in _spaceList!.Values)
                if (thisSpace.ColorOwner != EnumColorChoice.None && thisSpace.WhatBoard == EnumBoardStatus.IsStart)
                {
                    thisSpace.Player = FindPlayer(thisSpace.ColorOwner);
                    if (thisSpace.Player > 0)
                    {
                        var tempPlayer = _gameContainer.PlayerList[thisSpace.Player];
                        tempPlayer.PieceList.Add(thisSpace.Index);
                    }
                }
                else
                    thisSpace.Player = 0;
            Repaint();
        }
        private int FindPlayer(EnumColorChoice thisColor) //done.
        {
            var thisPlayer = _gameContainer.PlayerList.SingleOrDefault(items => items.Color == thisColor);
            if (thisPlayer == null)
                return 0;
            return thisPlayer.Id;
        }
        public void StartTurn() //done.
        {
            _gameContainer.SaveRoot!.DiceNumber = 0;
            OurColor = _gameContainer.SingleInfo!.Color;
            _gameContainer.MovePlayer = 0;
            _model.Cup!.CanShowDice = false;
            Repaint();
        }

        public bool IsValidMove(int index) //done now.
        {
            return MoveList.Any(items => items.SpaceFrom == index);
        }
        private async Task NoMovesEndTurnAsync() //done.
        {
            _gameContainer.SaveRoot!.Instructions = "No moves.  End turn.";
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _gameContainer.EndTurnAsync!.Invoke();
        }
        private async Task DoAnotherTurnAsync() //done.
        {
            _gameContainer.SaveRoot!.Instructions = "Will get another turn for rolling a 6";
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _gameContainer.StartNewTurnAsync!.Invoke();
        }
        private async Task DoEndTurnAsync() //done
        {
            _gameContainer.SaveRoot!.Instructions = "Move was made successfully";
            if (_gameContainer.Test!.NoAnimations == false)
                await _gameContainer.Delay!.DelaySeconds(.5);
            await _gameContainer.EndTurnAsync!.Invoke();
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
            var tempPlayer = _gameContainer.PlayerList![whatPlayer];
            var thisCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.IsStart && items.ColorOwner == tempPlayer.Color && items.Player == 0).ToCustomBasicList();
            if (thisCol.Count == 0)
                throw new BasicBlankException("Could not find the start space");
            return thisCol.First().Index;
        }
        private bool IsGameOver => _spaceList!.Values.Count(items => items.Player == _gameContainer.WhoTurn && items.WhatBoard == EnumBoardStatus.IsHome) == 4; //done i think.
        private async Task BackToStartAsync(int oldSpace, int oldPlayer) //done i think.
        {
            int thisStart = FindStartSpace(oldPlayer);
            _gameContainer.PlayerGoingBack = oldPlayer;
            _gameContainer.Animates.LocationFrom = _graphicsCP.LocationOfSpace(oldSpace);
            _gameContainer.Animates.LocationTo = _graphicsCP.LocationOfSpace(thisStart);
            var tempPlayer = _gameContainer.PlayerList![oldPlayer];
            tempPlayer.PieceList.RemoveSpecificItem(oldSpace);
            await _gameContainer.Animates.DoAnimateAsync();
            _gameContainer.PlayerGoingBack = 0;
            var thisSpace = _spaceList![thisStart];
            thisSpace.Player = oldPlayer;
            tempPlayer.PieceList.Add(thisStart); //you are now at start.
            if (tempPlayer.PieceList.Count != 4)
                throw new BasicBlankException("After going back to start, needs 4 pieces");
            Repaint();
        }
        private int CalculateSafety(SpaceInfo thisSpace, int lefts, bool willStart) //done i think.
        {
            int newNum;
            if (willStart == false)
                if (thisSpace.WhatBoard != EnumBoardStatus.IsHome)
                    newNum = lefts + 1;
                else
                    newNum = thisSpace.SpaceNumber + lefts;
            else
                newNum = lefts;
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
            if (_gameContainer.SaveRoot!.DiceNumber == 0)
                return; //ignore because did not even roll.
            MoveList = new CustomBasicList<MoveInfo>();
            CustomBasicList<int> firstCol = _gameContainer.SingleInfo!.PieceList.ToCustomBasicList();
            if (firstCol.Count != 4)
                throw new BasicBlankException("The player must have 4 pieces");
            if (_gameContainer.SaveRoot.DiceNumber < 0 || _gameContainer.SaveRoot.DiceNumber > 6)
                throw new BasicBlankException("The dice has to be from 1 to 6");
            CustomBasicList<int> thisCol = new CustomBasicList<int>();
            if (_gameContainer.SaveRoot.DiceNumber < 6)
                firstCol.ForEach(thisInt =>
                {
                    if (!_spaceList!.Values.Any(items => items.Index == thisInt && items.WhatBoard == EnumBoardStatus.IsStart))
                        thisCol.Add(thisInt);
                });
            else
                thisCol = firstCol;
            int newPlayer;
            MoveInfo thisMove;
            thisCol.ForEach(thisInt =>
            {
                var thisSpace = _spaceList![thisInt];
                var nextSpace = CalculateNewSpace(thisSpace, _gameContainer.SaveRoot.DiceNumber);
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
                if (_gameContainer.SaveRoot.DiceNumber == 6)
                {
                    _gameContainer.SaveRoot.Instructions = "No moves.  Will get another turn.";
                    if (_gameContainer.Test!.NoAnimations == false)
                        await _gameContainer.Delay!.DelaySeconds(.5);
                    await _gameContainer.StartNewTurnAsync!.Invoke(); //i think
                    return;
                }
                await NoMovesEndTurnAsync();
                return;
            }
            int ourID = 0;
            if (_gameContainer.BasicData!.MultiPlayer)
                ourID = _gameContainer.PlayerList!.GetSelf().Id;
            if (_gameContainer.BasicData.MultiPlayer == false || _gameContainer.WhoTurn != ourID)
                _gameContainer.SaveRoot.Instructions = $"Waiting for {_gameContainer.SingleInfo.NickName} to make a move";
            else
                _gameContainer.SaveRoot.Instructions = "Choose a piece to move";
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        public async Task MakeMoveAsync(int space)
        {
            if (!MoveList.Any(items => items.SpaceFrom == space) && space == 0)
                throw new BasicBlankException($"Space {space} was not found on the movelist");
            var thisSpace = _spaceList![space];
            _gameContainer.MovePlayer = 0;
            Repaint();
            MoveInfo thisMove;
            thisMove = MoveList.Single(items => items.SpaceFrom == space);
            var newSpace = _spaceList[thisMove.SpaceTo];
            thisSpace.Player = 0;
            int oldPlayer = newSpace.Player;
            _gameContainer.SingleInfo!.PieceList.RemoveSpecificItem(thisMove.SpaceFrom);
            if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
            {
                _gameContainer.SingleInfo.PieceList.Add(thisMove.SpaceTo);
                if (newSpace.Player > 0)
                {
                    var otherPlayer = _gameContainer.PlayerList![oldPlayer];
                    newSpace.Player = _gameContainer.WhoTurn;
                    await BackToStartAsync(thisMove.SpaceTo, oldPlayer);
                }
                else
                {
                    newSpace.Player = _gameContainer.WhoTurn;
                    Repaint();
                }
                if (_gameContainer.SaveRoot!.DiceNumber < 6)
                    await DoEndTurnAsync();
                else
                    await DoAnotherTurnAsync();
                return;
            }
            Repaint();
            int nextMove;
            for (int x = 1; x <= _gameContainer.SaveRoot!.DiceNumber; x++)
            {
                nextMove = CalculateNewSpace(thisSpace, x);

                _gameContainer.MovePlayer = nextMove;
                Repaint();
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.1);
            }
            _gameContainer.MovePlayer = 0;
            _gameContainer.SingleInfo.PieceList.Add(newSpace.Index);
            Repaint();
            if (newSpace.Player != 0)
            {
                newSpace.Player = _gameContainer.WhoTurn;
                await BackToStartAsync(newSpace.Index, oldPlayer);
            }
            else
                newSpace.Player = _gameContainer.WhoTurn;
            if (IsGameOver)
            {
                await _gameContainer.ShowWinAsync!.Invoke();
                return;
            }
            if (_gameContainer.SaveRoot.DiceNumber == 6)
            {
                await DoAnotherTurnAsync();
                return;
            }
            await DoEndTurnAsync();
        }
    }
}