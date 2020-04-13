using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SolitaireBoardGameCP.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace SolitaireBoardGameCP.Logic
{
    //if this is being passed around, then has to be singleton.  however, when the main view model is created, it will create a new game object.
    //if rounds, then rethink.  probably does not apply to single player games as much.
    //well see when we get to bunco.
    [SingletonGame]
    public class SolitaireBoardGameMainGameClass : IAggregatorContainer
    {
        //internal SolitaireBoardGameViewModel ThisMod;
        public GameSpace PreviousSpace = new GameSpace();
        private readonly ISaveSinglePlayerClass _thisState;
        private readonly ISolitaireBoardEvents _solitaireBoard; //well see.
        internal SolitaireBoardGameSaveInfo _saveRoot;
        public IGamePackageResolver MainContainer { get; set; }
        public SolitaireBoardGameMainGameClass(ISaveSinglePlayerClass thisState, IEventAggregator aggregator,
            ISolitaireBoardEvents solitaireBoard,
            IGamePackageResolver mainContainer
            )
        {
            _thisState = thisState;
            Aggregator = aggregator;
            MainContainer = mainContainer;
            _solitaireBoard = solitaireBoard;
            _saveRoot = mainContainer.ReplaceObject<SolitaireBoardGameSaveInfo>(); //i think should be this way for now just in case.
            LoadBoard();
        }

        private void LoadBoard()
        {
            int x;
            int y;
            GameSpace thisSpace;
            for (x = 1; x <= 7; x++)
            {
                for (y = 1; y <= 7; y++)
                {
                    if (x < 3 & y < 3 | x > 5 & y < 3 | x < 3 & y > 5 | y > 5 & x > 5)
                    {
                    }
                    else
                    {
                        thisSpace = new GameSpace();
                        thisSpace.Vector = new Vector(x, y);
                        _saveRoot.SpaceList.Add(thisSpace);
                    }
                }
            }
            if (_saveRoot.SpaceList.Count() == 0)
            {
                throw new BasicBlankException("Can't have 0 items in the gameboard collection");
            }
        }

        internal bool _gameGoing;


        public Vector PreviousPiece
        {
            get
            {
                return _saveRoot.PreviousPiece;
            }
            set
            {
                _saveRoot.PreviousPiece = value;
            }
        }

        public IEventAggregator Aggregator { get; }

        public async Task NewGameAsync()
        {



            PreviousPiece = new Vector();
            await ClearBoardAsync();



        }

        public async Task ClearBoardAsync()
        {

            if (await _thisState.CanOpenSavedSinglePlayerGameAsync() == false)
            {
                foreach (var ThisSpace in _saveRoot.SpaceList)
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
                _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<SolitaireBoardGameSaveInfo>();
                MainContainer.ReplaceObject(_saveRoot);
                //hopefully no need to replacesavedgame (?)
                //ThisE.ReplaceSavedGame();
            }
            await Aggregator.SendLoadAsync();
        }
        public void SelectUnSelectSpace(GameSpace thisSpace)
        {
            foreach (GameSpace tempSpace in _saveRoot.SpaceList)
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
                await _solitaireBoard.PiecePlacedAsync(thisSpace, this);
                return;
            }
            await _solitaireBoard.PieceSelectedAsync(thisSpace, this);
        }
        public async Task HightlightSpaceAsync(GameSpace thisSpace)
        {
            SelectUnSelectSpace(thisSpace);
            PreviousPiece = thisSpace.Vector;
            await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
        }
        private IEnumerable<GameSpace> MoveList(GameSpace thisSpace) // i think its of the type of gamespace well see
        {
            GameSpace previousSpace;
            previousSpace = _saveRoot.SpaceList[PreviousPiece];
            return MoveList(thisSpace, previousSpace);
        }
        private IEnumerable<GameSpace> MoveList(GameSpace thisSpace, GameSpace previousSpace)
        {
            IEnumerable<GameSpace> output;
            if (thisSpace.Vector.Column == previousSpace.Vector.Column)
            {
                if (thisSpace.Vector.Row > previousSpace.Vector.Row)
                {
                    output = from Spaces in _saveRoot.SpaceList
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
                    output = from Spaces in _saveRoot.SpaceList
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
                output = from Spaces in _saveRoot.SpaceList
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
                output = from Spaces in _saveRoot.SpaceList
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
            previousSpace = _saveRoot.SpaceList[PreviousPiece];
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
            await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
        }
        private int PiecesRemaining()
        {
            return _saveRoot.SpaceList.Count(Items => Items.HasImage == true);
        }
        public async Task MakeMoveAsync(GameSpace thisSpace)
        {
            var thisList = MoveList(thisSpace);
            foreach (var item in thisList)
            {
                item.HasImage = false; //for now.
            }
            var nextSpace = _saveRoot.SpaceList[PreviousPiece];
            nextSpace.HasImage = false;
            GameSpace tempPiece = new GameSpace();
            tempPiece.Color = cs.Blue;
            tempPiece.HasImage = true;
            await Aggregator.AnimateMovePiecesAsync(PreviousPiece, thisSpace.Vector, tempPiece);
            thisSpace.HasImage = true;
            PreviousPiece = new Vector(); //i think
            int Manys = PiecesRemaining();
            if (Manys == 1)
            {
                await _thisState.DeleteSinglePlayerGameAsync();
                await ShowWinAsync();
                return;
            }
            await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
        }

        //private async Task RestoreGameAsync()
        //{
        //    _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<SolitaireBoardGameSaveInfo>();

        //}
        public async Task ShowWinAsync()
        {
            _gameGoing = false;
            await UIPlatform.ShowMessageAsync("You Win");
            //ThisMod.NewGameVisible = true;
            await _thisState.DeleteSinglePlayerGameAsync();
            //send message to show win.
            await this.SendGameOverAsync();

        }
    }
}
