using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace LifeBoardGameCP
{
    [SingletonGame]
    public class LifeBoardGameSaveInfo : BasicSavedPlainBoardGameClass<EnumColorChoice, CarPieceCP, LifeBoardGamePlayerItem>
    {
        private EnumWhatStatus _GameStatus;
        public EnumWhatStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.GameStatus = value;
                }
            }
        }
        public CustomBasicList<TileInfo> TileList { get; set; } = new CustomBasicList<TileInfo>();
        public DeckRegularDict<HouseInfo> HouseList { get; set; } = new DeckRegularDict<HouseInfo>();
        public bool WasMarried { get; set; }
        public bool GameStarted { get; set; }
        public bool WasNight { get; set; }
        public int MaxChosen { get; set; }
        public int NewPosition { get; set; }
        public bool EndAfterSalary { get; set; }
        public bool EndAfterStock { get; set; }
        public int NumberRolled { get; set; } // i think this is needed as well
        public int SpinPosition { get; set; }
        public int ChangePosition { get; set; }
        public CustomBasicList<int> SpinList { get; set; } = new CustomBasicList<int>(); // needs this.  so based on career chosen, they can get the 100,000.
        public int TemporarySpaceSelected { get; set; }
        private LifeBoardGameViewModel? _thisMod;
        internal void LoadMod(LifeBoardGameViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.GameStatus = GameStatus;
        }
    }
}