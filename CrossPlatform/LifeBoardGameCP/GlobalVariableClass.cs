using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GameboardPositionHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
namespace LifeBoardGameCP
{
    [SingletonGame]
    public class GlobalVariableClass
    {
        internal GameSpace? CountrySpace;
        internal GameSpace? MillionSpace;
        internal GameSpace? ExtraSpace;
        internal PositionPieces? Pos;
        public EnumViewCategory CurrentView { get; set; } = EnumViewCategory.StartGame;
        public CustomBasicList<SpaceInfo>? SpaceList { get; set; }
        private readonly LifeBoardGameMainGameClass _mainGame;
        public ISpinnerCanvas? SpinnerCanvas;
        public GlobalVariableClass(LifeBoardGameMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public int CurrentSelected
        {
            get
            {
                return _mainGame.ThisMod!.CurrentSelected;
            }
            set
            {
                _mainGame.ThisMod!.CurrentSelected = value;
            }
        }
        public EnumWhatStatus GameStatus
        {
            get
            {
                return _mainGame.SaveRoot!.GameStatus;
            }
            set
            {
                _mainGame.SaveRoot!.GameStatus = value;
            }
        }
        public CustomBasicList<int> SpinList
        {
            get
            {
                return _mainGame.SaveRoot!.SpinList;
            }
            set
            {
                _mainGame.SaveRoot!.SpinList = value;
            }
        }
        public CustomBasicList<TileInfo> TileList
        {
            get
            {
                return _mainGame.SaveRoot!.TileList;
            }
            set
            {
                _mainGame.SaveRoot!.TileList = value;
            }
        }
        public bool WasMarried
        {
            get
            {
                return _mainGame.SaveRoot!.WasMarried;
            }
            set
            {
                _mainGame.SaveRoot!.WasMarried = value;
            }
        }
        public bool WasNight
        {
            get
            {
                return _mainGame.SaveRoot!.WasNight;
            }
            set
            {
                _mainGame.SaveRoot!.WasNight = value;
            }
        }
        public int MaxChosen
        {
            get
            {
                return _mainGame.SaveRoot!.MaxChosen;
            }
            set
            {
                _mainGame.SaveRoot!.MaxChosen = value;
            }
        }
        public bool EndAfterSalary
        {
            get
            {
                return _mainGame.SaveRoot!.EndAfterSalary;
            }
            set
            {
                _mainGame.SaveRoot!.EndAfterSalary = value;
            }
        }
        public bool EndAfterStock
        {
            get
            {
                return _mainGame.SaveRoot!.EndAfterStock;
            }
            set
            {
                _mainGame.SaveRoot!.EndAfterStock = value;
            }
        }
        public int GetNumberSpun(int position)
        {
            if (position >= 3 && position <= 33)
                return 1;
            if (position >= 40 && position <= 69)
                return 2;
            if (position >= 76 && position <= 105)
                return 3;
            if (position >= 112 && position <= 141)
                return 4;
            if (position >= 148 && position <= 177)
                return 5;
            if (position >= 184 && position <= 213)
                return 6;
            if (position >= 220 && position <= 249)
                return 7;
            if (position >= 256 && position <= 285)
                return 8;
            if (position >= 292 && position <= 321)
                return 9;
            if (position >= 328 && position <= 357)
                return 10;
            return 0;
        }
    }
}