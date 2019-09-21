using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace XPuzzleCP
{
    public enum EnumMoveList
    {
        None, TurnOver, MadeMove, Won
    }
    [SingletonGame]
    public class XPuzzleGameBoardClass : IAdvancedDIContainer
    {
        private EnumMoveList _results = EnumMoveList.None;

        public IGamePackageResolver? MainContainer { get; set; }
        private readonly XPuzzleGlobalMod _thisGlobal;
        private XPuzzleSaveInfo _games;
        private readonly RandomGenerator _rs;
        private readonly ISaveSinglePlayerClass _thisSave;
        public XPuzzleGameBoardClass(IGamePackageResolver container)
        {
            MainContainer = container;
            _thisGlobal = MainContainer.Resolve<XPuzzleGlobalMod>();
            _games = MainContainer.Resolve<XPuzzleSaveInfo>();
            _rs = MainContainer.Resolve<RandomGenerator>();
            _games.SpaceList.SetContainer(container);
            _thisSave = MainContainer.Resolve<ISaveSinglePlayerClass>();
        }
        private Vector PreviousOpen
        {
            get
            {
                return _games.PreviousOpen;
            }
            set
            {
                _games.PreviousOpen = value;
            }
        }
        public EnumMoveList Results()
        {
            if (_results == EnumMoveList.None)
                throw new Exception("No move was made");
            return _results;
        }
        public async Task NewGameAsync()
        {
            if (_thisGlobal.GameLoaded == true || await _thisSave.CanOpenSavedSinglePlayerGameAsync() == false)
            {
                _games.SpaceList.ClearBoard();
                PreviousOpen = new Vector(3, 3);
                CustomBasicList<int> thisList = _rs.GenerateRandomList(8);
                _games.SpaceList.PopulateBoard(thisList);
            }
            else
            {
                //EventAggregator ThisE = MainContainer.Resolve<EventAggregator>();
                _games = await _thisSave.RetrieveSinglePlayerGameAsync<XPuzzleSaveInfo>();
                MainContainer!.ReplaceObject(_games);
                //ThisE.ReplaceSavedGame(); //if somebody has something they should retrieve it.

                //if we need replacesavedgame, will do.  hopefully can find another way to deal with this now (?)


            }
            if (_thisGlobal.GameLoaded == false)
            {
                EventAggregator thisE = MainContainer!.Resolve<EventAggregator>();
                await thisE.SendLoadAsync();
            }
            await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
        }

        public async Task MakeMoveAsync(int row, int column)
        {
            XPuzzleSpaceInfo thisSpace = _games.SpaceList[row, column];
            await MakeMoveAsync(thisSpace); //so i can do for testing.
        }

        public async Task MakeMoveAsync(XPuzzleSpaceInfo thisSpace)
        {
            _results = EnumMoveList.None;
            if (IsValidMove(thisSpace) == false)
            {
                _results = EnumMoveList.TurnOver;
                return;
            }
            XPuzzleSpaceInfo previousSpace;
            previousSpace = _games.SpaceList[PreviousOpen];
            previousSpace.Color = cs.Navy;
            thisSpace.Color = cs.Black;
            previousSpace.Text = thisSpace.Text;
            thisSpace.Text = "";
            PreviousOpen = thisSpace.Vector; //i think            
            if (HasWon() == true)
            {
                _results = EnumMoveList.Won; //no saving of game.
                await _thisSave.DeleteSinglePlayerGameAsync();
                return;
            }
            await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
            _results = EnumMoveList.MadeMove;
        }
        public bool IsValidMove(int row, int column)
        {
            XPuzzleSpaceInfo thisSpace = _games.SpaceList[row, column];
            return IsValidMove(thisSpace);
        }
        public bool IsValidMove(XPuzzleSpaceInfo thisSpace) //so it can be used for testing
        {
            if (thisSpace.Vector.Column == PreviousOpen.Column &&
                thisSpace.Vector.Row == PreviousOpen.Row)
                return false;
            XPuzzleSpaceInfo newSpace;
            newSpace = _games.SpaceList[PreviousOpen];
            if (thisSpace.Vector.Column + 1 == newSpace.Vector.Column)
            {
                if (thisSpace.Vector.Row == newSpace.Vector.Row)
                    return true;
            }
            if (thisSpace.Vector.Column - 1 == newSpace.Vector.Column)
            {
                if (thisSpace.Vector.Row == newSpace.Vector.Row)
                    return true;
            }
            if (thisSpace.Vector.Row + 1 == newSpace.Vector.Row)
            {
                if (thisSpace.Vector.Column == newSpace.Vector.Column)
                    return true;
            }
            if (thisSpace.Vector.Row - 1 == newSpace.Vector.Row)
            {
                if (thisSpace.Vector.Column == newSpace.Vector.Column)
                    return true;
            }
            return false;
        }
        public bool HasWon()
        {
            if (PreviousOpen.Column != 3 && PreviousOpen.Row != 3)
                return false;
            int x = 0;
            foreach (var thisSpace in _games.SpaceList)
            {
                x += 1;
                if (x < 9)
                {
                    if (thisSpace.Text == "")
                        return false;
                    if (int.Parse(thisSpace.Text) != x)
                        return false;
                }
            }
            return true;
        }
    }
}