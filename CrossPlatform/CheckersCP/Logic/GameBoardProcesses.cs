using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.BasicGameBoards;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CheckersCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace CheckersCP.Logic
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardProcesses
    {
        private readonly GameBoardGraphicsCP _graphicsBoard;
        private readonly CheckersGameContainer _gameContainer;

        public GameBoardProcesses(GameBoardGraphicsCP graphicsBoard, CheckersGameContainer gameContainer)
        {
            _graphicsBoard = graphicsBoard;
            _gameContainer = gameContainer;
        }

        public SpaceCP? GetSpace(int row, int column) => GameBoardGraphicsCP.GetSpace(row, column);
        private async Task<CustomBasicList<PlayerSpace>> GetNewTurnDataAsync(CustomBasicList<PlayerSpace> oldData)
        {
            var thisText = await js.SerializeObjectAsync(oldData);
            return await js.DeserializeObjectAsync<CustomBasicList<PlayerSpace>>(thisText);
        }
        public void ClearBoard()
        {
            _graphicsBoard.ClearBoard(); //i think.
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentPieceList.Clear();
                var firstList = GameBoardGraphicsCP.GetBlackStartingSpaces();
                firstList.ForEach(thisIndex =>
                {
                    var thisSpace = new PlayerSpace();
                    thisSpace.Index = thisIndex;
                    thisSpace.IsCrowned = false;
                    thisPlayer.CurrentPieceList.Add(thisSpace);
                });
            });
        }
        private bool NeedsReversed(int playerConsidered)
        {
            if (_gameContainer.BasicData!.MultiPlayer)
            {
                var tempPlayer = _gameContainer.PlayerList![playerConsidered];
                return tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman;
            }
            return _gameContainer.WhoTurn != playerConsidered; //try this
        }
        private void PopulateSpaces()
        {
            _graphicsBoard.ClearBoard(); //before populating again.
            int realIndex;
            bool wasReversed;
            _gameContainer.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentPieceList.ForEach(thisTemp =>
                {
                    wasReversed = NeedsReversed(thisPlayer.Id);
                    realIndex = GameBoardGraphicsCP.GetRealIndex(thisTemp.Index, wasReversed);
                    var thisSpace = _gameContainer.SpaceList.Single(items => items.MainIndex == realIndex);
                    thisSpace.PlayerOwns = thisPlayer.Id;
                    thisSpace.WasReversed = wasReversed;
                    thisSpace.IsCrowned = thisTemp.IsCrowned;
                    thisSpace.PlayerColor = thisPlayer.Color.ToColor();
                });
            });
        }
        //no need for loadboard now.

        public async Task StartNewTurnAsync()
        {
            await _gameContainer.PlayerList!.ForEachAsync(async thisPlayer =>
            {
                thisPlayer.StartPieceList = await GetNewTurnDataAsync(thisPlayer.CurrentPieceList);
            });
            _gameContainer.SaveRoot!.ForcedToMove = false;
            _gameContainer.SaveRoot.SpaceHighlighted = 0;
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.None;
            PopulateSpaces();
            PopulateMoves();
            _gameContainer.Aggregator.RepaintBoard();
            await _gameContainer.ContinueTurnAsync!.Invoke();
        }
        private void PopulateMoves()
        {
            _gameContainer.CurrentMoveList = new CustomBasicList<MoveInfo>();
            _gameContainer.CompleteMoveList = new CustomBasicList<MoveInfo>();
            GetActualMoves();
        }
        #region "Move Processes"
        private bool _currentReversed;

        private MoveInfo PopulateMove(SpaceCP oldSpace, SpaceCP newSpace)
        {
            MoveInfo output = new MoveInfo();
            output.SpaceFrom = oldSpace.MainIndex;
            output.SpaceTo = newSpace.MainIndex;
            return output;
        }
        private CustomBasicList<MoveInfo> GetMovesWithPiece(SpaceCP oldSpace)
        {
            int row, column, checkRow, checkColumn;
            row = oldSpace.Row;
            column = oldSpace.Column;

            void UpdateChecker()
            {
                checkRow = row;
                checkColumn = column;
            }
            UpdateChecker();
            CustomBasicList<MoveInfo> thisList = new CustomBasicList<MoveInfo>();
            SpaceCP? newSpace;
            int playerIndex;
            MoveInfo thisMove;

            void CheckMoves(int incRow, int incColumn)
            {
                checkRow += incRow;
                checkColumn += incColumn;
                newSpace = GetSpace(checkRow, checkColumn);
                if (newSpace != null)
                {
                    if (newSpace.PlayerOwns == 0)
                    {
                        thisMove = PopulateMove(oldSpace, newSpace);
                        thisList.Add(thisMove);
                    }
                    else if (newSpace.PlayerOwns != oldSpace.PlayerOwns)
                    {
                        playerIndex = newSpace.MainIndex;
                        checkRow += incRow;
                        checkColumn += incColumn;
                        newSpace = GetSpace(checkRow, checkColumn);
                        if (newSpace != null)
                        {
                            if (newSpace.PlayerOwns == 0)
                            {
                                thisMove = PopulateMove(oldSpace, newSpace);
                                thisMove.PlayerCaptured = playerIndex;
                                thisList.Add(thisMove);
                            }
                        }
                    }
                }
            }

            if (_currentReversed == false || oldSpace.IsCrowned)
            {
                CheckMoves(-1, 1);
                UpdateChecker();
                CheckMoves(-1, -1);
            }
            if (oldSpace.IsCrowned == false && _currentReversed == false)
                return thisList;
            UpdateChecker();
            CheckMoves(1, 1);
            UpdateChecker();
            CheckMoves(1, -1);
            return thisList;
        }
        private void GetActualMoves()
        {
            _currentReversed = NeedsReversed(_gameContainer.WhoTurn);
            var tempList = _gameContainer.SpaceList.Where(items => items.PlayerOwns == _gameContainer.WhoTurn).ToCustomBasicList();
            if (_gameContainer.SaveRoot!.SpaceHighlighted > 0)
                tempList.KeepConditionalItems(items => items.MainIndex == _gameContainer.SaveRoot.SpaceHighlighted);
            tempList.ForEach(oldSpace =>
            {
                _gameContainer.CompleteMoveList.AddRange(GetMovesWithPiece(oldSpace));
            });
            if (_gameContainer.CompleteMoveList.Any(items => items.PlayerCaptured > 0))
                _gameContainer.SaveRoot.ForcedToMove = true;
            if (_gameContainer.SaveRoot.ForcedToMove)
                _gameContainer.CompleteMoveList.KeepConditionalItems(items => items.PlayerCaptured > 0);
        }

        public bool IsValidMove(int space)
        {
            if (space == _gameContainer.SaveRoot!.SpaceHighlighted)
                return true;
            if (_gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == space))
                return true;
            if (_gameContainer.SaveRoot.SpaceHighlighted == 0)
                return false;
            return _gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
        }
        private async Task RemovePieceAsync(int player, int oldPosition)
        {
            int tempTurn = _gameContainer.WhoTurn;
            if (_gameContainer.WhoTurn == 1)
                _gameContainer.WhoTurn = 2;
            else
                _gameContainer.WhoTurn = 1;
            _gameContainer.Animates!.LocationTo = _graphicsBoard.SuggestedOffLocation(_currentReversed);
            await _gameContainer.Animates.DoAnimateAsync();
            _gameContainer.WhoTurn = tempTurn;
            var thisPlayer = _gameContainer.PlayerList![player];
            var thisSpace = thisPlayer.CurrentPieceList.Single(items => items.Index == oldPosition);
            thisPlayer.CurrentPieceList.RemoveSpecificItem(thisSpace);
        }
        private void PopulateCurrent() //for autoresume, this may be needed
        {
            if (_gameContainer.SaveRoot!.SpaceHighlighted == 0)
                throw new BasicBlankException("No current move to populate with");
            _gameContainer.CurrentMoveList = _gameContainer.CompleteMoveList.Where(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted).ToCustomBasicList();
        }
        public async Task MakeMoveAsync(int space)
        {
            var hIndex = GameBoardGraphicsCP.GetRealIndex(_gameContainer.SaveRoot!.SpaceHighlighted, _currentReversed);
            if (_gameContainer.SaveRoot.SpaceHighlighted == space)
            {
                _gameContainer.SaveRoot.SpaceHighlighted = 0;
                _gameContainer.CurrentMoveList.Clear();
                _gameContainer.Aggregator.RepaintBoard();
                await _gameContainer.ContinueTurnAsync!();
                return;
            }
            if (_gameContainer.SaveRoot.SpaceHighlighted == 0)
            {
                _gameContainer.SaveRoot.SpaceHighlighted = space;
                PopulateCurrent();
                _gameContainer.Aggregator.RepaintBoard();
                await _gameContainer.ContinueTurnAsync!();
                return;
            }
            if (_gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == space))
            {
                _gameContainer.SaveRoot.SpaceHighlighted = space;
                PopulateCurrent();
                _gameContainer.Aggregator.RepaintBoard();
                await _gameContainer.ContinueTurnAsync!();
                return;
            }
            var thisMove = _gameContainer.CurrentMoveList.Single(items => items.SpaceTo == space);
            var thisSpace = GameBoardGraphicsCP.GetSpace(_gameContainer.SaveRoot.SpaceHighlighted, false);
            var newSpace = GameBoardGraphicsCP.GetSpace(space, false);
            _gameContainer.Animates!.LocationFrom = thisSpace.GetLocation();
            _gameContainer.Animates.LocationTo = thisSpace.GetLocation();
            var myCurrent = _gameContainer.SingleInfo!.CurrentPieceList.Single(items => items.Index == hIndex);
            _gameContainer.CurrentCrowned = thisSpace.IsCrowned;
            await _gameContainer.Animates.DoAnimateAsync();
            if (_gameContainer.BasicData!.MultiPlayer == false || _gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                myCurrent.Index = space;
            else
                myCurrent.Index = GameBoardGraphicsCP.GetIndexByPoint(9 - newSpace.Row, 9 - newSpace.Column);
            thisSpace.ClearSpace();
            _gameContainer.CurrentCrowned = newSpace.IsCrowned;
            newSpace.PlayerColor = _gameContainer.SingleInfo.Color.ToColor();
            newSpace.IsCrowned = myCurrent.IsCrowned;
            newSpace.PlayerOwns = _gameContainer.WhoTurn;
            bool isGameOver = false;
            if (thisMove.PlayerCaptured > 0)
            {
                var tempSpace = _gameContainer.SpaceList.Single(items => items.MainIndex == thisMove.PlayerCaptured);
                var newPlayer = tempSpace.PlayerOwns;
                var tempReverse = NeedsReversed(newPlayer);
                var oldPosition = GameBoardGraphicsCP.GetRealIndex(tempSpace.MainIndex, tempReverse);
                _gameContainer.Animates.LocationFrom = tempSpace.GetLocation();
                await RemovePieceAsync(newPlayer, oldPosition);
                tempSpace.PlayerOwns = 0;
                var tempPlayer = _gameContainer.PlayerList![newPlayer];
                if (tempPlayer.CurrentPieceList.Count == 0)
                    isGameOver = true;
            }
            _gameContainer.CurrentMoveList.Clear();
            _gameContainer.Aggregator.RepaintBoard();
            if (isGameOver || _gameContainer.Test.ImmediatelyEndGame)
            {
                await _gameContainer.ShowWinAsync!(); //in this case, no problem sending that class to do the code for showwinasync.
                return;
            }
            if (newSpace.IsCrowned == false)
            {
                if (newSpace.Row == 1 || newSpace.Row == 8)
                {
                    newSpace.IsCrowned = true;
                    myCurrent.IsCrowned = true;
                    _gameContainer.Aggregator.RepaintBoard();
                }
            }
            if (_gameContainer.SaveRoot.ForcedToMove)
            {
                _gameContainer.SaveRoot.SpaceHighlighted = space;
                _gameContainer.Aggregator.RepaintBoard();
                _gameContainer.CompleteMoveList.Clear();
                _gameContainer.CurrentMoveList.Clear();
                GetActualMoves();
                _gameContainer.CurrentMoveList = _gameContainer.CompleteMoveList.ToCustomBasicList(); //needs to copy over.
                if (_gameContainer.CompleteMoveList.Count > 0)
                {
                    await _gameContainer.ContinueTurnAsync!();
                    return;
                }
            }
            await _gameContainer.EndTurnAsync!();
        }
        #endregion
        private bool _finished;
        private Task AfterRepaintingAsync()
        {
            _gameContainer.SaveRoot!.SpaceHighlighted = 0; //i think if there was a space highlighted, it will return to 0.  so you have to choose the piece again if autoresume.
            PopulateSpaces();
            PopulateMoves();
            _graphicsBoard.Saved = false;
            _finished = true;
            _gameContainer.Aggregator.RepaintBoard();
            return Task.CompletedTask;
        }

        public async Task ReloadSavedGameAsync()
        {
            //await UIPlatform.ShowMessageAsync("trying to reload gameboard");
            _finished = false;
            _graphicsBoard.Saved = true; //could be iffy
            _gameContainer.Aggregator.RepaintBoard();
            BasicGameBoardDelegates.AfterPaintAsync = AfterRepaintingAsync;
            do
            {
                if (_finished)
                {
                    return;
                }
                await Task.Delay(10);
            } while (true);
        }
    }
}