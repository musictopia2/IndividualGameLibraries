using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SolitaireBoardGameCP
{
    [SingletonGame]
    public class SolitaireBoardGameMainGameClass
    {
        public GameSpace PreviousSpace = new GameSpace();
        private readonly SolitaireBoardGameViewModel _thisMod; //you do need a viewmodel this time.
        private SolitaireBoardGameSaveInfo _games;
        private ISolitaireBoardEvents? _solitaireE;
        private readonly EventAggregator _thisE;
        private readonly ISaveSinglePlayerClass _thisSave;
        private bool _didLoad;
        public IGamePackageResolver MainContainer { get; set; }
        public Vector PreviousPiece
        {
            get
            {
                return _games.PreviousPiece;
            }
            set
            {
                _games.PreviousPiece = value;
            }
        }
        public bool IsEnabled
        {
            get
            {
                return _thisMod.BoardEnabled;
            }
            set
            {
                _thisMod.BoardEnabled = value;
            }
        }

        internal void FinishLoading()
        {
            _solitaireE = MainContainer.Resolve<ISolitaireBoardEvents>();
        }

        public SolitaireBoardGameMainGameClass(IGamePackageResolver container, SolitaireBoardGameViewModel thisMod)
        {

            MainContainer = container;
            _games = MainContainer.Resolve<SolitaireBoardGameSaveInfo>();
            _thisE = MainContainer.Resolve<EventAggregator>();
            _thisSave = MainContainer.Resolve<ISaveSinglePlayerClass>();
            _thisMod = thisMod;
            LoadBoard();
        }

        private void LoadBoard()
        {
            int x;
            int y;
            GameSpace ThisSpace;
            for (x = 1; x <= 7; x++)
            {
                for (y = 1; y <= 7; y++)
                {
                    if (x < 3 & y < 3 | x > 5 & y < 3 | x < 3 & y > 5 | y > 5 & x > 5)
                    {
                    }
                    else
                    {
                        ThisSpace = new GameSpace();
                        ThisSpace.Vector = new Vector(x, y);
                        _games.SpaceList.Add(ThisSpace);
                    }
                }
            }
            if (_games.SpaceList.Count() == 0)
                throw new BasicBlankException("Can't have 0 items in the gameboard collection");
        }
        public async Task NewGameAsync()
        {
            PreviousPiece = new Vector();
            await ClearBoardAsync();
            IsEnabled = true;
        }
        public async Task ClearBoardAsync()
        {

            if (_didLoad == true || await _thisSave.CanOpenSavedSinglePlayerGameAsync() == false)
            {
                foreach (var ThisSpace in _games.SpaceList)
                {
                    if (ThisSpace.Vector.Row == 4 && ThisSpace.Vector.Column == 4)
                        ThisSpace.HasImage = false;
                    else
                        ThisSpace.HasImage = true;
                    ThisSpace.ClearSpace();
                }
                PreviousSpace = new GameSpace();
            }
            else
            {
                _games = await _thisSave.RetrieveSinglePlayerGameAsync<SolitaireBoardGameSaveInfo>();
                MainContainer.ReplaceObject(_games);
                //hopefully no need to replacesavedgame (?)
                //ThisE.ReplaceSavedGame();
            }
            if (_didLoad == false)
            {
                await _thisE.SendLoadAsync();
                _didLoad = true;
            }
        }
        public void SelectUnSelectSpace(GameSpace thisSpace)
        {
            foreach (GameSpace tempSpace in _games.SpaceList)
            {
                tempSpace.ClearSpace();
            }
            if (thisSpace == PreviousSpace)
                PreviousSpace = new GameSpace();
            else
            {
                PreviousSpace = thisSpace;
                PreviousSpace.Color = cs.Yellow;
            }
        }
        public async Task ProcessCommandAsync(GameSpace thisSpace)
        {
            if (thisSpace.HasImage == false)
            {
                if (PreviousPiece.Column == 0 || PreviousPiece.Row == 0)
                {
                    return; // because nothing selected
                }
                await _solitaireE!.PiecePlacedAsync(thisSpace);
                return;
            }
            await _solitaireE!.PieceSelectedAsync(thisSpace);
        }
        public async Task HightlightSpaceAsync(GameSpace thisSpace)
        {
            SelectUnSelectSpace(thisSpace);
            PreviousPiece = thisSpace.Vector;
            await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
        }
        private IEnumerable<GameSpace> MoveList(GameSpace thisSpace) // i think its of the type of gamespace well see
        {
            GameSpace previousSpace;
            previousSpace = _games.SpaceList[PreviousPiece];
            return MoveList(thisSpace, previousSpace);
        }
        private IEnumerable<GameSpace> MoveList(GameSpace thisSpace, GameSpace previousSpace)
        {
            IEnumerable<GameSpace> output;
            if (thisSpace.Vector.Column == previousSpace.Vector.Column)
            {
                if (thisSpace.Vector.Row > previousSpace.Vector.Row)
                {
                    output = from Spaces in _games.SpaceList
                             where Spaces.Vector.Column == thisSpace.Vector.Column
                             & Spaces.Vector.Row > previousSpace.Vector.Row & Spaces.Vector.Row < thisSpace.Vector.Row
                             select Spaces;
                    if (output.Count() > 0)
                        output = from Moves in output
                                 orderby Moves.Vector.Row
                                 select Moves;
                }
                else
                {
                    output = from Spaces in _games.SpaceList
                             where Spaces.Vector.Column == thisSpace.Vector.Column &
                             Spaces.Vector.Row < previousSpace.Vector.Row & Spaces.Vector.Row > thisSpace.Vector.Row
                             select Spaces;
                    if (output.Count() > 0)
                        output = from Moves in output
                                 orderby Moves.Vector.Row descending
                                 select Moves;
                }
                return output;
            }
            if (thisSpace.Vector.Column > previousSpace.Vector.Column)
            {
                output = from Spaces in _games.SpaceList
                         where Spaces.Vector.Row == thisSpace.Vector.Row &
                         Spaces.Vector.Column > previousSpace.Vector.Column & Spaces.Vector.Column < thisSpace.Vector.Column
                         select Spaces;
                if (output.Count() > 0)
                    output = from Moves in output
                             orderby Moves.Vector.Row
                             select Moves;
                return output;
            }
            else
            {
                output = from Spaces in _games.SpaceList
                         where Spaces.Vector.Row == thisSpace.Vector.Row &
                         Spaces.Vector.Column < previousSpace.Vector.Column & Spaces.Vector.Column > thisSpace.Vector.Column
                         select Spaces;
                if (output.Count() > 0)
                    output = from Moves in output
                             orderby Moves.Vector.Row descending
                             select Moves;
                return output;
            }
        }
        public bool IsValidMove(GameSpace thisSpace)
        {
            GameSpace previousSpace;
            previousSpace = _games.SpaceList[PreviousPiece];
            if (thisSpace.Vector.Column != previousSpace.Vector.Column &&
                thisSpace.Vector.Row != previousSpace.Vector.Row)
                return false;
            IEnumerable<GameSpace> thisCol;
            thisCol = MoveList(thisSpace, previousSpace);
            if (thisCol.Count() == 0)
                return false;
            return !thisCol.Any(Items => Items.HasImage == false);
        }
        public async Task UnselectPieceAsync(GameSpace thisSpace)
        {
            if (PreviousPiece.Equals(thisSpace))
                PreviousPiece = new Vector();
            SelectUnSelectSpace(thisSpace);
            await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
        }
        private int PiecesRemaining()
        {
            return _games.SpaceList.Count(Items => Items.HasImage == true);
        }
        public async Task MakeMoveAsync(GameSpace thisSpace)
        {
            var thisList = MoveList(thisSpace);
            foreach (var item in thisList)
            {
                item.HasImage = false; //for now.
            }
            var nextSpace = _games.SpaceList[PreviousPiece];
            nextSpace.HasImage = false;
            GameSpace tempPiece = new GameSpace();
            tempPiece.Color = cs.Blue;
            tempPiece.HasImage = true;
            await _thisE.AnimateMovePiecesAsync(PreviousPiece, thisSpace.Vector, tempPiece);
            thisSpace.HasImage = true;
            PreviousPiece = new Vector(); //i think
            int Manys = PiecesRemaining();
            if (Manys == 1)
            {
                await _thisSave.DeleteSinglePlayerGameAsync();
                await _solitaireE!.WonAsync();
                return;
            }
            await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
            _solitaireE!.MoveCompleted();
        }
    }
}