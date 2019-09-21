using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace CheckersCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly GameBoardGraphicsCP _graphicsBoard;
        private readonly CheckersMainGameClass _mainGame;
        private readonly EventAggregator _thisE;
        public GameBoardProcesses(GameBoardGraphicsCP graphicsBoard, CheckersMainGameClass mainGame, EventAggregator thisE)
        {
            _graphicsBoard = graphicsBoard;
            _mainGame = mainGame;
            _thisE = thisE;
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
            _mainGame.PlayerList!.ForEach(thisPlayer =>
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
            if (_mainGame.ThisData!.MultiPlayer)
            {
                var tempPlayer = _mainGame.PlayerList![playerConsidered];
                return tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman;
            }
            return _mainGame.WhoTurn != playerConsidered; //try this
        }
        private void PopulateSpaces()
        {
            _graphicsBoard.ClearBoard(); //before populating again.
            int realIndex;
            bool wasReversed;
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentPieceList.ForEach(thisTemp =>
                {
                    wasReversed = NeedsReversed(thisPlayer.Id);
                    realIndex = GameBoardGraphicsCP.GetRealIndex(thisTemp.Index, wasReversed);
                    var thisSpace = _mainGame.ThisGlobal!.SpaceList.Single(items => items.MainIndex == realIndex);
                    thisSpace.PlayerOwns = thisPlayer.Id;
                    thisSpace.WasReversed = wasReversed;
                    thisSpace.IsCrowned = thisTemp.IsCrowned;
                    thisSpace.PlayerColor = thisPlayer.Color.ToColor();
                });
            });
        }
        public void LoadBoard()
        {
            _mainGame.ThisGlobal!.Animates = new BasicGameFramework.GameGraphicsCP.Animations.AnimateSkiaSharpGameBoard();
            _mainGame.ThisGlobal.Animates.LongestTravelTime = 100;
            _mainGame.ThisGlobal.CurrentCrowned = false;
        }
        public async Task StartNewTurnAsync()
        {
            await _mainGame.PlayerList!.ForEachAsync(async thisPlayer =>
            {
                thisPlayer.StartPieceList = await GetNewTurnDataAsync(thisPlayer.CurrentPieceList);
            });
            _mainGame.SaveRoot!.ForcedToMove = false;
            _mainGame.SaveRoot.SpaceHighlighted = 0;
            _mainGame.SaveRoot.GameStatus = EnumGameStatus.None;
            PopulateSpaces();
            PopulateMoves();
            _thisE.RepaintBoard();
            await _mainGame.ContinueTurnAsync();
        }
        private void PopulateMoves()
        {
            _mainGame.ThisGlobal!.CurrentMoveList = new CustomBasicList<MoveInfo>();
            _mainGame.ThisGlobal.CompleteMoveList = new CustomBasicList<MoveInfo>();
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
            _currentReversed = NeedsReversed(_mainGame.WhoTurn);
            var tempList = _mainGame.ThisGlobal!.SpaceList.Where(items => items.PlayerOwns == _mainGame.WhoTurn).ToCustomBasicList();
            if (_mainGame.SaveRoot!.SpaceHighlighted > 0)
                tempList.KeepConditionalItems(items => items.MainIndex == _mainGame.SaveRoot.SpaceHighlighted);
            tempList.ForEach(oldSpace =>
            {
                _mainGame.ThisGlobal.CompleteMoveList.AddRange(GetMovesWithPiece(oldSpace));
            });
            if (_mainGame.ThisGlobal.CompleteMoveList.Any(items => items.PlayerCaptured > 0))
                _mainGame.SaveRoot.ForcedToMove = true;
            if (_mainGame.SaveRoot.ForcedToMove)
                _mainGame.ThisGlobal.CompleteMoveList.KeepConditionalItems(items => items.PlayerCaptured > 0);
        }

        public bool IsValidMove(int space)
        {
            if (space == _mainGame.SaveRoot!.SpaceHighlighted)
                return true;
            if (_mainGame.ThisGlobal!.CompleteMoveList.Any(items => items.SpaceFrom == space))
                return true;
            if (_mainGame.SaveRoot.SpaceHighlighted == 0)
                return false;
            return _mainGame.ThisGlobal.CompleteMoveList.Any(items => items.SpaceFrom == _mainGame.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
        }
        private async Task RemovePieceAsync(int player, int oldPosition)
        {
            int tempTurn = _mainGame.WhoTurn;
            if (_mainGame.WhoTurn == 1)
                _mainGame.WhoTurn = 2;
            else
                _mainGame.WhoTurn = 1;
            _mainGame.ThisGlobal!.Animates!.LocationTo = _graphicsBoard.SuggestedOffLocation(_currentReversed);
            await _mainGame.ThisGlobal.Animates.DoAnimateAsync();
            _mainGame.WhoTurn = tempTurn;
            var thisPlayer = _mainGame.PlayerList![player];
            var thisSpace = thisPlayer.CurrentPieceList.Single(items => items.Index == oldPosition);
            thisPlayer.CurrentPieceList.RemoveSpecificItem(thisSpace);
        }
        private void PopulateCurrent() //for autoresume, this may be needed
        {
            if (_mainGame.SaveRoot!.SpaceHighlighted == 0)
                throw new BasicBlankException("No current move to populate with");
            _mainGame.ThisGlobal!.CurrentMoveList = _mainGame.ThisGlobal.CompleteMoveList.Where(items => items.SpaceFrom == _mainGame.SaveRoot.SpaceHighlighted).ToCustomBasicList();
        }
        public async Task MakeMoveAsync(int space)
        {
            var hIndex = GameBoardGraphicsCP.GetRealIndex(_mainGame.SaveRoot!.SpaceHighlighted, _currentReversed);
            if (_mainGame.SaveRoot.SpaceHighlighted == space)
            {
                _mainGame.SaveRoot.SpaceHighlighted = 0;
                _mainGame.ThisGlobal!.CurrentMoveList.Clear();
                _thisE.RepaintBoard();
                await _mainGame.ContinueTurnAsync();
                return;
            }
            if (_mainGame.SaveRoot.SpaceHighlighted == 0)
            {
                _mainGame.SaveRoot.SpaceHighlighted = space;
                PopulateCurrent();
                _thisE.RepaintBoard();
                await _mainGame.ContinueTurnAsync();
                return;
            }
            if (_mainGame.ThisGlobal!.CompleteMoveList.Any(items => items.SpaceFrom == space))
            {
                _mainGame.SaveRoot.SpaceHighlighted = space;
                PopulateCurrent();
                _thisE.RepaintBoard();
                await _mainGame.ContinueTurnAsync();
                return;
            }
            var thisMove = _mainGame.ThisGlobal.CurrentMoveList.Single(items => items.SpaceTo == space);
            var thisSpace = GameBoardGraphicsCP.GetSpace(_mainGame.SaveRoot.SpaceHighlighted, false);
            var newSpace = GameBoardGraphicsCP.GetSpace(space, false);
            _mainGame.ThisGlobal.Animates!.LocationFrom = thisSpace.GetLocation();
            _mainGame.ThisGlobal.Animates.LocationTo = thisSpace.GetLocation();
            var myCurrent = _mainGame.SingleInfo!.CurrentPieceList.Single(items => items.Index == hIndex);
            _mainGame.ThisGlobal.CurrentCrowned = thisSpace.IsCrowned;
            await _mainGame.ThisGlobal.Animates.DoAnimateAsync();
            if (_mainGame.ThisData!.MultiPlayer == false || _mainGame.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                myCurrent.Index = space;
            else
                myCurrent.Index = GameBoardGraphicsCP.GetIndexByPoint(9 - newSpace.Row, 9 - newSpace.Column);
            thisSpace.ClearSpace();
            _mainGame.ThisGlobal.CurrentCrowned = newSpace.IsCrowned;
            newSpace.PlayerColor = _mainGame.SingleInfo.Color.ToColor();
            newSpace.IsCrowned = myCurrent.IsCrowned;
            newSpace.PlayerOwns = _mainGame.WhoTurn;
            bool isGameOver = false;
            if (thisMove.PlayerCaptured > 0)
            {
                var tempSpace = _mainGame.ThisGlobal.SpaceList.Single(items => items.MainIndex == thisMove.PlayerCaptured);
                var newPlayer = tempSpace.PlayerOwns;
                var tempReverse = NeedsReversed(newPlayer);
                var oldPosition = GameBoardGraphicsCP.GetRealIndex(tempSpace.MainIndex, tempReverse);
                _mainGame.ThisGlobal.Animates.LocationFrom = tempSpace.GetLocation();
                await RemovePieceAsync(newPlayer, oldPosition);
                tempSpace.PlayerOwns = 0;
                var tempPlayer = _mainGame.PlayerList![newPlayer];
                if (tempPlayer.CurrentPieceList.Count == 0)
                    isGameOver = true;
            }
            _mainGame.ThisGlobal.CurrentMoveList.Clear();
            _thisE.RepaintBoard();
            if (isGameOver)
            {
                await _mainGame.ShowWinAsync();
                return;
            }
            if (newSpace.IsCrowned == false)
            {
                if (newSpace.Row == 1 || newSpace.Row == 8)
                {
                    newSpace.IsCrowned = true;
                    myCurrent.IsCrowned = true;
                    _thisE.RepaintBoard();
                }
            }
            if (_mainGame.SaveRoot.ForcedToMove)
            {
                _mainGame.SaveRoot.SpaceHighlighted = space;
                _thisE.RepaintBoard();
                _mainGame.ThisGlobal.CompleteMoveList.Clear();
                _mainGame.ThisGlobal.CurrentMoveList.Clear();
                GetActualMoves();
                _mainGame.ThisGlobal.CurrentMoveList = _mainGame.ThisGlobal.CompleteMoveList.ToCustomBasicList(); //needs to copy over.
                if (_mainGame.ThisGlobal.CompleteMoveList.Count > 0)
                {
                    await _mainGame.ContinueTurnAsync();
                    return;
                }
            }
            await _mainGame.EndTurnAsync();
        }
        #endregion
        public async Task ReloadSavedGameAsync()
        {
            _thisE.RepaintBoard();
            _graphicsBoard.Saved = true; //could be iffy
            await Task.Delay(200); //problem is this time, needs the actual coordinates for positioning.
            _mainGame.SaveRoot!.SpaceHighlighted = 0; //i think if there was a space highlighted, it will return to 0.  so you have to choose the piece again if autoresume.
            PopulateSpaces();
            PopulateMoves();
            _graphicsBoard.Saved = false;
            _thisE.RepaintBoard();
        }
    }
}