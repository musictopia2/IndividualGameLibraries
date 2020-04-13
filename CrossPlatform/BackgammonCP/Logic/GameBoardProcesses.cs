using BackgammonCP.Data;
using BackgammonCP.Graphics;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace BackgammonCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {

        private readonly BackgammonGameContainer _gameContainer;
        private readonly GameBoardGraphicsCP _graphicsBoard;
        private readonly BackgammonVMData _model;

        public GameBoardProcesses(BackgammonGameContainer gameContainer, GameBoardGraphicsCP graphicsBoard, BackgammonVMData model)
        {
            _gameContainer = gameContainer;
            _graphicsBoard = graphicsBoard;
            _model = model;
        }
        #region "Reverse Processes"
        internal int GetReversedID(int oldID)
        {
            if (oldID == 0)
                return 27;
            if (oldID == 25)
                return 26;
            if (oldID == 26)
                return 25;
            if (oldID == 27)
                return 0;
            return 25 - oldID;
        }
        private bool NeedsReversed(int playerConsidered)
        {
            var tempPlayer = _gameContainer.PlayerList![playerConsidered];
            if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
                return true;
            if (_gameContainer.BasicData!.MultiPlayer)
                return tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman;
            if (_gameContainer.PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer))
                return false;
            return _gameContainer.WhoTurn != playerConsidered;
        }
        private TriangleClass GetTriangle(int player, int yourID, bool autoPopulate = true)
        {
            bool rets = NeedsReversed(player);
            int newID;
            if (rets == true)
                newID = GetReversedID(yourID);
            else
                newID = yourID;
            var thisTriangle = _gameContainer.TriangleList[newID];
            if (autoPopulate)
                thisTriangle.PlayerOwns = player;
            return thisTriangle;
        }
        #endregion
        private async Task<BackgammonPlayerDetails> GetNewTurnDataAsync(BackgammonPlayerDetails oldData)
        {
            string thisText = await js.SerializeObjectAsync(oldData);
            return await js.DeserializeObjectAsync<BackgammonPlayerDetails>(thisText);
        }
        private void PopulateDiceValues()
        {
            var thisList = _model.Cup!.DiceList;
            if (thisList.Count != 2)
                throw new BasicBlankException("There should be just 2 dice values");
            _gameContainer.FirstDiceValue = thisList.First().Value;
            _gameContainer.SecondDiceValue = thisList.Last().Value;
        }
        private void ClearTriangles()
        {
            foreach (var thisTriangle in _gameContainer.TriangleList.Values)
            {
                thisTriangle.NumberOfTiles = 0;
                thisTriangle.PlayerOwns = 0;
                thisTriangle.Locations.Clear();
            }
        }
        private void PopulateTriangles()
        {
            TriangleClass thisTriangle;
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                if (thisPlayer.CurrentTurnData!.PiecesAtHome > 0)
                {
                    thisTriangle = GetTriangle(thisPlayer.Id, 25);
                    thisTriangle.NumberOfTiles = thisPlayer.CurrentTurnData.PiecesAtHome;
                }
                if (thisPlayer.CurrentTurnData.PiecesAtStart > 0)
                {
                    thisTriangle = GetTriangle(thisPlayer.Id, 0);
                    thisTriangle.NumberOfTiles = thisPlayer.CurrentTurnData.PiecesAtStart;
                }
                var thisList = thisPlayer.CurrentTurnData.PiecesOnBoard.GroupBy(items => items).ToCustomBasicList();
                thisList.ForEach(thisIndex =>
                {
                    var ourIndex = thisIndex.Key;
                    var count = thisIndex.Count();
                    thisTriangle = GetTriangle(thisPlayer.Id, ourIndex);
                    thisTriangle.NumberOfTiles = count;
                });
            });
        }
        private int SpaceNumberForRoll(CustomBasicList<int> thisCol, int whatNum, bool willReverse)
        {
            int thisNum;
            if (willReverse)
                thisNum = GetReversedID(whatNum);
            else
                thisNum = whatNum;
            if (thisCol.Any(items => items == thisNum))
                return thisNum;
            return 0;
        }
        private int SpaceAboveRoll(CustomBasicList<int> thisCol, int whatNum, bool willReverse)
        {
            int thisNum;
            if (willReverse)
                thisNum = GetReversedID(whatNum);
            else
                thisNum = whatNum;
            int finalValue = 0;
            int diffs = 50;
            int oldDiffs = 0;
            thisCol.ForEach(thisIndex =>
            {
                if (thisIndex > thisNum && willReverse == false)
                    oldDiffs = thisIndex - thisNum;
                else if (thisIndex < thisNum && willReverse)
                    oldDiffs = thisNum - thisIndex;
                if (oldDiffs < diffs)
                {
                    diffs = oldDiffs;
                    finalValue = thisIndex;
                }
            });
            return finalValue;
        }
        private int OneToStack(int whatNum, CustomBasicList<int> thisCol, bool willReverse)
        {
            var thisNum = SpaceNumberForRoll(thisCol, whatNum, willReverse);
            if (thisNum > 0)
                return thisNum;
            return SpaceAboveRoll(thisCol, whatNum, willReverse);
        }
        private EnumStatusType CalculateStatus(int index)
        {
            var thisSpace = _gameContainer.TriangleList[index];
            if (thisSpace.PlayerOwns == 0)
                return EnumStatusType.CompletelyOpen;
            if (thisSpace.NumberOfTiles == 1)
            {
                if (thisSpace.PlayerOwns == _gameContainer.WhoTurn)
                    return EnumStatusType.PlayerHasOne;
                else
                    return EnumStatusType.KnockOtherPlayer;
            }
            if (thisSpace.PlayerOwns == _gameContainer.WhoTurn)
                return EnumStatusType.PlayerOwns;
            return EnumStatusType.Closed;
        }
        private int NumberToMove(int whatNum, bool isReversed)
        {
            if (isReversed == false)
                return whatNum;
            return whatNum * -1;
        }
        private CustomBasicList<int> ListSpacesFrom() => _gameContainer.TriangleList.Where(items => items.Value.PlayerOwns == _gameContainer.WhoTurn && items.Key > 0 && items.Key < 25).Select(items => items.Key).OrderBy(items => items).ToCustomBasicList();
        private bool CanStackUp(bool isReversed)
        {
            var thisList = ListSpacesFrom();
            if (isReversed == false)
                return thisList.First() >= 19;
            return thisList.Last() <= 6;
        }
        private SKPoint CalculateDiffs(int spaceNumber)
        {
            if (spaceNumber == 0)
                return _graphicsBoard.GetActualPoint(30, 0);
            if (spaceNumber == 27)
                return _graphicsBoard.GetActualPoint(-30, 0);
            if (spaceNumber == 25 || spaceNumber == 26)
                return _graphicsBoard.GetActualPoint(0, -11);
            var thisTriangle = _gameContainer.TriangleList[spaceNumber];
            int values;
            if (thisTriangle.NumberOfTiles > 5)
                values = 15;
            else
                values = 28;
            if (spaceNumber > 0 & spaceNumber < 13)
                return _graphicsBoard.GetActualPoint(0, values);
            else
                return _graphicsBoard.GetActualPoint(0, values * -1);
        }
        private SKPoint CalculateFirstLocation(int spaceNumber, SKRect tempRect)
        {
            var diffRight = _graphicsBoard.GetActualPoint(30, 0);
            var diffBottom = _graphicsBoard.GetActualPoint(0, 30);
            if (spaceNumber == 25 || spaceNumber == 26)
                return new SKPoint(tempRect.Location.X, tempRect.Bottom - diffBottom.Y);
            if (spaceNumber == 0)
                return new SKPoint(tempRect.Location.X + 3, tempRect.Location.Y);
            if (spaceNumber == 27)
                return new SKPoint(tempRect.Right - diffRight.X, tempRect.Location.Y);
            if (spaceNumber > 0 && spaceNumber < 13)
                return new SKPoint(tempRect.Location.X + 3, tempRect.Location.Y);
            else if (spaceNumber > 12 && spaceNumber < 25)
                return new SKPoint(tempRect.Location.X + 3, tempRect.Bottom - diffBottom.Y);
            else
                throw new BasicBlankException("Cannot find the space");
        }
        private SKPoint CalculateNewLocation(int spaceNumber)
        {
            var tempPoint = CalculateDiffs(spaceNumber);
            var diffx = tempPoint.X;
            var diffy = tempPoint.Y;
            var thisTriangle = _gameContainer.TriangleList[spaceNumber];
            if (thisTriangle.NumberOfTiles == 0)
            {
                var tempRect = _graphicsBoard.GetRectangleSpace(spaceNumber);
                return CalculateFirstLocation(spaceNumber, tempRect);
            }
            var thisLocation = thisTriangle.Locations.Last();
            tempPoint = thisLocation;
            tempPoint.X += diffx;
            tempPoint.Y += diffy;
            return tempPoint;
        }
        private CustomBasicList<MoveInfo> MovesWithSpecificNumber(int value)
        {
            bool willReverse = _gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self;
            bool stacks;
            bool fromStart;
            if (_gameContainer.SingleInfo.CurrentTurnData!.PiecesAtStart > 0)
            {
                stacks = false;
                fromStart = true;
            }
            else
            {
                fromStart = false;
                stacks = CanStackUp(willReverse); //i think
            }
            if (fromStart == true && stacks == true)
                throw new BasicBlankException("Can't stack if from start");
            EnumStatusType thisResult;
            MoveInfo thisMove;
            CustomBasicList<MoveInfo> thisList = new CustomBasicList<MoveInfo>();
            if (fromStart)
            {
                if (willReverse == false)
                    thisResult = CalculateStatus(value);
                else
                    thisResult = CalculateStatus(GetReversedID(value));
                if (thisResult == EnumStatusType.Closed)
                    return new CustomBasicList<MoveInfo>(); //because no moves can be made
                thisMove = new MoveInfo();
                thisMove.DiceNumber = value;
                thisMove.Results = thisResult;
                if (willReverse == false)
                {
                    thisMove.SpaceFrom = 0;
                    thisMove.SpaceTo = value;
                }
                else
                {
                    thisMove.SpaceFrom = 27;
                    thisMove.SpaceTo = GetReversedID(value);
                }
                return new CustomBasicList<MoveInfo> { thisMove };
            }
            var fromList = ListSpacesFrom();
            if (fromList.Count == 0)
                return new CustomBasicList<MoveInfo>(); //because there was none.
            int nextStart = 0;
            if (stacks)
                nextStart = OneToStack(value, fromList, willReverse);
            var newNum = NumberToMove(value, willReverse);
            int endMove;
            fromList.ForEach(thisIndex =>
            {
                endMove = thisIndex + newNum; //maybe this simple (?)
                if (endMove <= 0)
                    endMove = 26;
                if (endMove > 24)
                {
                    if (stacks && nextStart == thisIndex)
                    {
                        thisMove = new MoveInfo();
                        thisMove.Results = EnumStatusType.Stackup;
                        thisMove.SpaceFrom = thisIndex;
                        thisMove.DiceNumber = value;
                        if (willReverse)
                            thisMove.SpaceTo = 26;
                        else
                            thisMove.SpaceTo = 25;
                        thisList.Add(thisMove);
                    }
                }
                else
                {
                    thisResult = CalculateStatus(endMove);
                    if (thisResult != EnumStatusType.Closed)
                    {
                        thisMove = new MoveInfo();
                        thisMove.Results = thisResult;
                        thisMove.SpaceFrom = thisIndex;
                        thisMove.DiceNumber = value;
                        thisMove.SpaceTo = endMove;
                        thisList.Add(thisMove);
                    }
                }
            });
            return thisList;
        }
        private void RepositionPieces()
        {
            var offsetSize = _graphicsBoard.GetActualPoint(3, 3);
            foreach (var thisTriangle in _gameContainer.TriangleList.Values)
            {
                thisTriangle.Locations.Clear();
                if (thisTriangle.NumberOfTiles > 0)
                {
                    int index = _gameContainer.TriangleList.GetKey(thisTriangle);
                    var thisRect = _graphicsBoard.GetRectangleSpace(index);
                    var tempPoint = CalculateDiffs(index);
                    var thisLocation = CalculateFirstLocation(index, thisRect);
                    var diffx = tempPoint.X;
                    var diffy = tempPoint.Y;
                    if (index > 0 && index < 25)
                        thisLocation.X += offsetSize.X;
                    thisTriangle.NumberOfTiles.Times(x =>
                    {
                        thisTriangle.Locations.Add(thisLocation);
                        thisLocation = new SKPoint(thisLocation.X + diffx, thisLocation.Y + diffy);
                    });
                }
            }
        }
        private async Task BackToStartAsync(int player, int index, int oldPosition)
        {
            var thisPlayer = _gameContainer.PlayerList![player];
            thisPlayer.CurrentTurnData!.PiecesOnBoard!.RemoveSpecificItem(oldPosition);
            thisPlayer.CurrentTurnData.PiecesAtStart++;
            _gameContainer.Animates!.LocationTo = CalculateNewLocation(index);
            RepositionPieces();
            int tempTurn = _gameContainer.WhoTurn;
            _gameContainer.WhoTurn = tempTurn;
            await _gameContainer.Animates.DoAnimateAsync();
            _gameContainer.WhoTurn = tempTurn;
            var thisTriangle = _gameContainer.TriangleList[index];
            thisTriangle.PlayerOwns = player;
            thisTriangle.NumberOfTiles += thisTriangle.NumberOfTiles; //not sure;
            PopulateTriangles();
        }
        private void PopulateMoves()
        {
            _gameContainer.MoveList = new CustomBasicList<MoveInfo>();
            ClearTriangles();
            PopulateTriangles();
            bool rets = _gameContainer.HadDoubles();
            CustomBasicList<int> thisList = new CustomBasicList<int>();
            if (rets == true)
            {
                thisList.Add(_gameContainer.FirstDiceValue);
            }
            else
            {
                if (_gameContainer.SaveRoot!.NumberUsed != _gameContainer.FirstDiceValue)
                    thisList.Add(_gameContainer.FirstDiceValue);
                if (_gameContainer.SaveRoot.NumberUsed != _gameContainer.SecondDiceValue)
                    thisList.Add(_gameContainer.SecondDiceValue);
            }
            if (thisList.Count == 0)
                throw new BasicBlankException("Should not populate moves because all used up.  If I can ignore, take this out");
            thisList.ForEach(thisValue =>
            {
                var tempList = MovesWithSpecificNumber(thisValue);
                _gameContainer.MoveList.AddRange(tempList);
            });
            RepositionPieces();
        }
        private void LoadTriangles()
        {
            _gameContainer.TriangleList = new System.Collections.Generic.Dictionary<int, TriangleClass>();
            for (int x = 0; x <= 27; x++)
            {
                var thisTriangle = new TriangleClass();
                _gameContainer.TriangleList.Add(x, thisTriangle);
            }
        }
        public void ClearBoard()
        {
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentTurnData = new BackgammonPlayerDetails();
                thisPlayer.CurrentTurnData.PiecesOnBoard = new CustomBasicList<int>() { 1, 1, 12, 12, 12, 12, 12, 17, 17, 17, 19, 19, 19, 19, 19 };
            });
            LoadTriangles();
            _gameContainer.MoveList = new CustomBasicList<MoveInfo>();
            ClearTriangles(); //i think
            PopulateTriangles();
            _gameContainer.SaveRoot!.SpaceHighlighted = -1;
            _gameContainer.SaveRoot.NumberUsed = 0;
            RepositionPieces();
            Repaint();
        }
        private void Repaint()
        {
            _gameContainer.RepaintBoard();
        }
        private bool CanEndGame() => _gameContainer.SingleInfo!.CurrentTurnData!.PiecesAtHome == 15;
        public async Task UndoAllMovesAsync()
        {
            _gameContainer.SaveRoot!.NumberUsed = 0;
            _gameContainer.SaveRoot.SpaceHighlighted = -1;
            _gameContainer.MoveList.Clear();
            _gameContainer.SaveRoot.MovesMade = 0;
            _gameContainer.SaveRoot.MadeAtLeastOneMove = false;
            _model.LastStatus = "You decided to undo all moves";
            await _gameContainer.PlayerList!.ForEachAsync(async thisPlayer => thisPlayer.CurrentTurnData = await GetNewTurnDataAsync(thisPlayer.StartTurnData!));
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
            PopulateMoves();
            RepositionPieces();
            Repaint();
            if (_gameContainer.DiceVisibleProcesses == null)
            {
                throw new BasicBlankException("Nobody is handling dice visible.  Rethink");
            }
            _gameContainer.DiceVisibleProcesses.Invoke();
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        public Task ReloadSavedGameAsync()
        {
            throw new BasicBlankException("No autoresume for now because of issues with repainting.  If i decided to do it eventually, then will have to put into interfaces which will do the work required when the first paint works.");
            //LoadTriangles();
            //PopulateDiceValues();
            //PopulateMoves();
            //_mainGame.DiceVisibleProcesses();
            //_graphicsBoard.Saved = true; //could be iffy
            //_thisE.RepaintBoard();
            //await Task.Delay(1000); //problem is this time, needs the actual coordinates for positioning.
            //RepositionPieces();
            //_graphicsBoard.Saved = false;
            //_alreadyPainted = false;
        }
        //private bool _alreadyPainted;
        //public void StartPaint()
        //{
        //    if (_alreadyPainted)
        //        return;
        //    _thisE.RepaintBoard();
        //    _alreadyPainted = true;
        //}
        public async Task StartNewTurnAsync()
        {
            _gameContainer.SaveRoot!.NumberUsed = 0;
            _gameContainer.MoveList.Clear();
            _gameContainer.SaveRoot.MovesMade = 0;
            _gameContainer.SaveRoot.MadeAtLeastOneMove = false;
            await _gameContainer.PlayerList!.ForEachAsync(async thisPlayer => thisPlayer.StartTurnData = await GetNewTurnDataAsync(thisPlayer.CurrentTurnData!));
            PopulateDiceValues();
            PopulateTriangles();
            PopulateMoves();
            RepositionPieces();
            Repaint();
            if (_gameContainer.MoveList.Count == 0)
            {
                _model.LastStatus = "No Moves Available";
                if (_gameContainer.Test!.NoAnimations == false)
                    await _gameContainer.Delay!.DelaySeconds(.5);
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
            _gameContainer.SaveRoot.SpaceHighlighted = -1;
        }
        public bool IsValidMove(int space)
        {
            if (_gameContainer.SaveRoot!.SpaceHighlighted == space)
                return true; //can always highlight/unhighlight
            if (_gameContainer.SaveRoot.SpaceHighlighted == -1)
                return _gameContainer.MoveList.Any(items => items.SpaceFrom == space);
            return _gameContainer.MoveList.Any(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
        }
        public async Task MakeMoveAsync(int space)
        {
            if (_gameContainer.SaveRoot!.SpaceHighlighted == space)
            {
                _gameContainer.SaveRoot.SpaceHighlighted = -1;
                Repaint();
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (_gameContainer.SaveRoot.SpaceHighlighted == -1)
            {
                _gameContainer.SaveRoot.SpaceHighlighted = space;
                Repaint();
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _gameContainer.MoveInProgress = true;
            var thisMove = _gameContainer.MoveList.First(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
            var thisTriangle = _gameContainer.TriangleList[_gameContainer.SaveRoot.SpaceHighlighted];
            thisTriangle.NumberOfTiles--;
            var thisLocation = thisTriangle.Locations.Last();
            if (thisTriangle.NumberOfTiles == 0)
                thisTriangle.PlayerOwns = 0;
            _gameContainer.Animates!.LocationFrom = thisLocation;
            _gameContainer.Animates.LocationTo = CalculateNewLocation(space);
            RepositionPieces();
            await _gameContainer.Animates.DoAnimateAsync();
            var newTriangle = _gameContainer.TriangleList[space];
            if (newTriangle.NumberOfTiles == 1)
            {
                if (newTriangle.PlayerOwns != _gameContainer.WhoTurn)
                {
                    var newPlayer = newTriangle.PlayerOwns;
                    newTriangle.PlayerOwns = _gameContainer.WhoTurn;
                    _gameContainer.Animates.LocationFrom = newTriangle.Locations.Single();
                    if (NeedsReversed(newPlayer))
                        await BackToStartAsync(newPlayer, 27, GetReversedID(space));
                    else
                        await BackToStartAsync(newPlayer, 0, space);
                }
                else
                {
                    newTriangle.NumberOfTiles++;
                }
            }
            else
            {
                newTriangle.PlayerOwns = _gameContainer.WhoTurn;
                newTriangle.NumberOfTiles++;
            }
            bool privateReverse = NeedsReversed(_gameContainer.WhoTurn);
            int newSpace;
            if (privateReverse)
                newSpace = GetReversedID(space);
            else
                newSpace = space;
            if (_gameContainer.SaveRoot.SpaceHighlighted == 0 || _gameContainer.SaveRoot.SpaceHighlighted == 27)
            {
                _gameContainer.SingleInfo!.CurrentTurnData!.PiecesAtStart--;
            }
            else
            {
                if (privateReverse)
                    _gameContainer.SaveRoot.SpaceHighlighted = GetReversedID(_gameContainer.SaveRoot.SpaceHighlighted);
                _gameContainer.SingleInfo!.CurrentTurnData!.PiecesOnBoard!.RemoveSpecificItem(_gameContainer.SaveRoot.SpaceHighlighted);
            }
            if (space == 25 || space == 26)
                _gameContainer.SingleInfo.CurrentTurnData.PiecesAtHome++;
            else
                _gameContainer.SingleInfo.CurrentTurnData.PiecesOnBoard!.Add(newSpace);
            _gameContainer.MoveInProgress = false;
            RepositionPieces();
            _gameContainer.SaveRoot.SpaceHighlighted = -1;
            Repaint();
            if (CanEndGame())
            {
                await _gameContainer.ShowWinAsync!.Invoke();
                return; //this simple i think.
            }
            _gameContainer.SaveRoot.MovesMade++;
            _gameContainer.SaveRoot.MadeAtLeastOneMove = true;
            if (_gameContainer.HadDoubles() == false)
            {
                if (_gameContainer.SaveRoot.NumberUsed > 0)
                {
                    if (_gameContainer.DiceVisibleProcesses == null)
                    {
                        throw new BasicBlankException("Nobody is handling dice visible.  Rethink");
                    }
                    _gameContainer.DiceVisibleProcesses.Invoke();
                    _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    return;
                }
                _gameContainer.SaveRoot.NumberUsed = thisMove.DiceNumber;
                if (_gameContainer.DiceVisibleProcesses == null)
                {
                    throw new BasicBlankException("Nobody is handling dice visible.  Rethink");
                }
                _gameContainer.DiceVisibleProcesses.Invoke();
                PopulateMoves();
                if (_gameContainer.MoveList.Count == 0)
                {
                    _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                    await _gameContainer.ContinueTurnAsync!.Invoke();
                    return;
                }
                _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return; //i think old version could had this bug.
            }
            if (_gameContainer.DiceVisibleProcesses == null)
            {
                throw new BasicBlankException("Nobody is handling dice visible.  Rethink");
            }
            _gameContainer.DiceVisibleProcesses.Invoke();
            if (_gameContainer.SaveRoot.MovesMade == 4)
            {
                _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                await _gameContainer.ContinueTurnAsync!.Invoke(); //becase you made all 4 moves.
                return;
            }
            PopulateMoves();
            Repaint();
            if (_gameContainer.MoveList.Count == 0)
            {
                _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
    }
}