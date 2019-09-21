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
namespace ChessCP
{
    [SingletonGame]
    public class GameBoardProcesses
    {
        private readonly GameBoardGraphicsCP _graphicsBoard;
        private readonly ChessMainGameClass _mainGame;
        private readonly EventAggregator _thisE;
        private readonly CustomBasicList<ChessMan> _chessList = new CustomBasicList<ChessMan>();
        public GameBoardProcesses(GameBoardGraphicsCP graphicsBoard, ChessMainGameClass mainGame, EventAggregator thisE)
        {
            _graphicsBoard = graphicsBoard;
            _mainGame = mainGame;
            _thisE = thisE;
        }

        public SpaceCP GetSpace(int row, int column) => GameBoardGraphicsCP.GetSpace(row, column)!;
        private async Task<CustomBasicList<PlayerSpace>> GetNewTurnDataAsync(CustomBasicList<PlayerSpace> oldData)
        {
            var thisText = await js.SerializeObjectAsync(oldData);
            return await js.DeserializeObjectAsync<CustomBasicList<PlayerSpace>>(thisText);
        }
        private int GetIndex(int row, int column) => GameBoardGraphicsCP.GetIndexByPoint(row, column);
        public void ClearBoard()
        {
            _graphicsBoard.ClearBoard(); //i think.
            _mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.CurrentPieceList.Clear();
                int x;
                int thisIndex;
                thisPlayer.CurrentPieceList = new CustomBasicList<PlayerSpace>();
                PlayerSpace thisPiece;
                for (x = 1; x <= 8; x++)
                {
                    thisIndex = GetIndex(7, x);
                    thisPiece = new PlayerSpace();
                    {
                        var withBlock = thisPiece;
                        withBlock.Index = thisIndex;
                        withBlock.Piece = EnumPieceType.PAWN;
                    }
                    thisPlayer.CurrentPieceList.Add(thisPiece);
                }
                thisIndex = GetIndex(8, 1);
                thisPiece = new PlayerSpace();
                thisPiece.Index = thisIndex;
                thisPiece.Piece = EnumPieceType.ROOK;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 8);
                thisPiece = new PlayerSpace();
                thisPiece.Piece = EnumPieceType.ROOK;
                thisPiece.Index = thisIndex;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 2);
                thisPiece = new PlayerSpace();
                thisPiece.Index = thisIndex;
                thisPiece.Piece = EnumPieceType.KNIGHT;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 7);
                thisPiece = new PlayerSpace();
                thisPiece.Index = thisIndex;
                thisPiece.Piece = EnumPieceType.KNIGHT;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 3);
                thisPiece = new PlayerSpace();
                thisPiece.Index = thisIndex;
                thisPiece.Piece = EnumPieceType.BISHOP;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 6);
                thisPiece = new PlayerSpace();
                thisPiece.Piece = EnumPieceType.BISHOP;
                thisPiece.Index = thisIndex;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 4);
                thisPiece = new PlayerSpace();
                thisPiece.Index = thisIndex;
                thisPiece.Piece = EnumPieceType.QUEEN;
                thisPlayer.CurrentPieceList.Add(thisPiece);
                thisIndex = GetIndex(8, 5);
                thisPiece = new PlayerSpace();
                thisPiece.Index = thisIndex;
                thisPiece.Piece = EnumPieceType.KING;
                thisPlayer.CurrentPieceList.Add(thisPiece);
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
            int realIndex;
            _graphicsBoard.ClearBoard(); // always needs to clear first before repopulating again
            bool wasReversed;
            foreach (var thisPlayer in _mainGame.PlayerList!)
            {
                foreach (var ThisTemp in thisPlayer.CurrentPieceList)
                {
                    if (NeedsReversed(thisPlayer.Id) == true)
                    {
                        realIndex = GameBoardGraphicsCP.GetRealIndex(ThisTemp.Index, true);
                        wasReversed = true;
                    }
                    else
                    {
                        realIndex = GameBoardGraphicsCP.GetRealIndex(ThisTemp.Index, false);
                        wasReversed = false;
                    }
                    var thisSpace = _mainGame.ThisGlobal!.SpaceList.Single(items => items.MainIndex == realIndex);
                    thisSpace.PlayerOwns = thisPlayer.Id;
                    thisSpace.WasReversed = wasReversed;
                    thisSpace.PlayerPiece = ThisTemp.Piece;
                    thisSpace.PlayerColor = thisPlayer.Color.ToColor();
                }
            }
            _chessList.Clear();
            var thisList = _mainGame.ThisGlobal!.SpaceList.Where(items => items.PlayerOwns != 0).ToCustomBasicList();
            foreach (var thisSpace in thisList)
            {
                ChessMan ThisMan;
                if (thisSpace.PlayerPiece == EnumPieceType.BISHOP)
                    ThisMan = new BishopClass();
                else if (thisSpace.PlayerPiece == EnumPieceType.KING)
                    ThisMan = new KingClass();
                else if (thisSpace.PlayerPiece == EnumPieceType.KNIGHT)
                    ThisMan = new KnightClass();
                else if (thisSpace.PlayerPiece == EnumPieceType.PAWN)
                    ThisMan = new PawnClass();
                else if (thisSpace.PlayerPiece == EnumPieceType.QUEEN)
                    ThisMan = new QueenClass();
                else if (thisSpace.PlayerPiece == EnumPieceType.ROOK)
                    ThisMan = new RookClass();
                else
                    throw new BasicBlankException("Nothing found");
                ThisMan.PieceCategory = thisSpace.PlayerPiece;
                ThisMan.Player = thisSpace.PlayerOwns;
                ThisMan.IsReversed = thisSpace.WasReversed;
                ThisMan.Row = thisSpace.Row;
                ThisMan.Column = thisSpace.Column;
                _chessList.Add(ThisMan);
            }
        }
        public async Task UndoAllMovesAsync()
        {
            _mainGame.SaveRoot!.SpaceHighlighted = 0;
            _mainGame.SaveRoot.GameStatus = EnumGameStatus.None;
            await _mainGame.PlayerList!.ForEachAsync(async thisPlayer => thisPlayer.CurrentPieceList = await GetNewTurnDataAsync(thisPlayer.StartPieceList));
            PopulateSpaces();
            PopulateMoves(); // you do have to populate the moves again
            _mainGame.SaveRoot.PossibleMove = new PreviousMove();
            _thisE.RepaintBoard();
            await _mainGame.ContinueTurnAsync();
        }
        private void PopulateMoves()
        {
            _mainGame.ThisGlobal!.CurrentMoveList = new CustomBasicList<MoveInfo>();
            _mainGame.ThisGlobal.CompleteMoveList = new CustomBasicList<MoveInfo>();
            GetActualMoves();
        }
        public async Task StartNewTurnAsync()
        {
            foreach (var thisPlayer in _mainGame.PlayerList!)
                // has to do all players in case one player gets knocked back to start
                thisPlayer.StartPieceList = await GetNewTurnDataAsync(thisPlayer.CurrentPieceList);
            _mainGame.SaveRoot!.PossibleMove = new PreviousMove();
            _mainGame.SaveRoot.SpaceHighlighted = 0;
            _mainGame.SaveRoot.GameStatus = EnumGameStatus.None; // start with none.
            PopulateSpaces();
            PopulateMoves();
            _thisE.RepaintBoard();
            await _mainGame.ContinueTurnAsync();
        }
        public void LoadBoard()
        {
            _mainGame.ThisGlobal!.Animates = new BasicGameFramework.GameGraphicsCP.Animations.AnimateSkiaSharpGameBoard();
            if (_mainGame.ThisData!.IsXamarinForms == false)
                _mainGame.ThisGlobal.Animates.LongestTravelTime = 200;
            else
                _mainGame.ThisGlobal.Animates.LongestTravelTime = 75;
            _mainGame.ThisGlobal.CurrentCrowned = false;
            _mainGame.ThisGlobal.CurrentPiece = EnumPieceType.None;
        }
        private bool _currentReversed;
        private void GetActualMoves() // version 1 will just show all possible moves here.
        {
            SpaceCP oldSpace;
            _currentReversed = NeedsReversed(_mainGame.WhoTurn);
            var tempList = _chessList.Where(items => items.Player == _mainGame.WhoTurn).ToCustomBasicList();
            foreach (ChessMan thisMan in tempList)
            {
                oldSpace = GetSpace(thisMan.Row, thisMan.Column);
                var thisList = thisMan.GetValidMoves();
                foreach (var thisV in thisList)
                {
                    var thisSpace = GetSpace(thisV.Row, thisV.Column);
                    MoveInfo thisMove = new MoveInfo();
                    thisMove.SpaceFrom = oldSpace.MainIndex;
                    thisMove.SpaceTo = thisSpace.MainIndex;
                    thisMove.Piece = thisMove.Piece;
                    if (thisSpace.PlayerOwns == 0)
                        thisMove.Results = EnumStatusType.CompletelyOpen;
                    else
                        thisMove.Results = EnumStatusType.PlayerOwns;// 
                    _mainGame.ThisGlobal!.CompleteMoveList.Add(thisMove);
                }
            }
        }
        public bool IsValidMove(int space)
        {
            var hIndex = GameBoardGraphicsCP.GetRealIndex(_mainGame.SaveRoot!.SpaceHighlighted, _currentReversed);
            if (hIndex == space)
                return true;
            int index = GameBoardGraphicsCP.GetRealIndex(space, _currentReversed);

            if (_mainGame.ThisGlobal!.CompleteMoveList.Any(items => items.SpaceFrom == index))
                return true;
            if (_mainGame.SaveRoot.SpaceHighlighted == 0)
                return false;
            return _mainGame.ThisGlobal.CompleteMoveList.Any(items => items.SpaceFrom == hIndex && items.SpaceTo == index);
        }
        private void PopulateCurrent() //for autoresume, this may be needed
        {
            if (_mainGame.SaveRoot!.SpaceHighlighted == 0)
                throw new BasicBlankException("No current move to populate with");
            _mainGame.ThisGlobal!.CurrentMoveList = _mainGame.ThisGlobal.CompleteMoveList.Where(items => items.SpaceFrom == _mainGame.SaveRoot.SpaceHighlighted).ToCustomBasicList();
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
            _mainGame.ThisGlobal.Animates.LocationTo = newSpace.GetLocation();
            var myCurrent = _mainGame.SingleInfo!.CurrentPieceList.Single(items => items.Index == hIndex);
            _mainGame.ThisGlobal.CurrentPiece = thisSpace.PlayerPiece;
            await _mainGame.ThisGlobal.Animates.DoAnimateAsync();
            if (_mainGame.ThisData!.MultiPlayer == false || _mainGame.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                myCurrent.Index = space;
            else
                myCurrent.Index = GameBoardGraphicsCP.GetIndexByPoint(9 - newSpace.Row, 9 - newSpace.Column);
            var newPlayer = newSpace.PlayerOwns;
            bool wasPawn = thisSpace.PlayerPiece == EnumPieceType.PAWN;
            thisSpace.ClearSpace();
            _mainGame.ThisGlobal.CurrentPiece = newSpace.PlayerPiece;
            newSpace.PlayerPiece = myCurrent.Piece; //try this.
            newSpace.PlayerColor = _mainGame.SingleInfo.Color.ToColor();
            newSpace.PlayerOwns = _mainGame.WhoTurn;
            bool wasKing = false;
            if (thisMove.Results == EnumStatusType.PlayerOwns)
            {
                if (_mainGame.ThisGlobal.CurrentPiece == EnumPieceType.KING)
                    wasKing = true;
                var tempReverse = NeedsReversed(newPlayer);
                var oldPosition = GameBoardGraphicsCP.GetRealIndex(space, tempReverse);
                _mainGame.ThisGlobal.Animates.LocationFrom = newSpace.GetLocation();
                await RemovePieceAsync(newPlayer, oldPosition);
            }
            _mainGame.SaveRoot.PossibleMove = new PreviousMove();
            _mainGame.SaveRoot.PossibleMove.PlayerColor = _mainGame.SingleInfo.Color.ToColor();
            _mainGame.SaveRoot.PossibleMove.SpaceFrom = _mainGame.SaveRoot.SpaceHighlighted;
            _mainGame.SaveRoot.PossibleMove.SpaceTo = space;
            _mainGame.SaveRoot.SpaceHighlighted = 0;
            _mainGame.ThisGlobal.CurrentMoveList.Clear();
            _thisE.RepaintBoard();
            if (wasKing)
            {
                await _mainGame.ShowWinAsync();
                return;
            }
            if (wasPawn)
            {
                if (newSpace.Row == 1 || newSpace.Row == 8)
                {
                    myCurrent.Piece = EnumPieceType.QUEEN;
                    newSpace.PlayerPiece = EnumPieceType.QUEEN;
                    _thisE.RepaintBoard();
                }
            }
            _mainGame.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            await _mainGame.ContinueTurnAsync();
        }
        public async Task ReloadSavedGameAsync()
        {
            _graphicsBoard.Saved = true; //could be iffy
            await Task.Delay(200); //problem is this time, needs the actual coordinates for positioning.
            _mainGame.SaveRoot!.SpaceHighlighted = 0; //i think if there was a space highlighted, it will return to 0.  so you have to choose the piece again if autoresume.
            PopulateSpaces();
            PopulateMoves();
            if (_mainGame.ThisData!.MultiPlayer && _mainGame.ThisData.Client && _mainGame.SaveRoot.PreviousMove.SpaceFrom > 0)
            {
                _mainGame.SaveRoot.PreviousMove.SpaceFrom = GameBoardGraphicsCP.GetRealIndex(_mainGame.SaveRoot.PreviousMove.SpaceFrom, true);
                _mainGame.SaveRoot.PreviousMove.SpaceTo = GameBoardGraphicsCP.GetRealIndex(_mainGame.SaveRoot.PreviousMove.SpaceTo, true);
            }
            _graphicsBoard.Saved = false;
            _thisE.RepaintBoard();
        }
    }
}