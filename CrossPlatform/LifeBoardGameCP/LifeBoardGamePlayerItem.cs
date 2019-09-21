using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace LifeBoardGameCP
{
    public class LifeBoardGamePlayerItem : PlayerBoardGame<EnumColorChoice>
    {
        public override bool DidChooseColor => Color != EnumColorChoice.None;
        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
        public DeckObservableDict<LifeBaseCard> Hand { get; set; } = new DeckObservableDict<LifeBaseCard>(); //this time, can use the standard hand.
        public EnumStart OptionChosen { get; set; }
        public int Position { get; set; } // where you are at on the board.
        public EnumFinal LastMove { get; set; }
        public int Distance { get; set; }
        public EnumGender Gender { get; set; }
        public CustomBasicList<EnumGender> ChildrenList { get; set; } = new CustomBasicList<EnumGender>();
        private decimal _Loans;
        public decimal Loans
        {
            get
            {
                return _Loans;
            }
            set
            {
                if (SetProperty(ref _Loans, value) == true)
                {
                }
            }
        }
        private decimal _Salary;
        public decimal Salary
        {
            get
            {
                return _Salary;
            }

            set
            {
                if (SetProperty(ref _Salary, value) == true)
                {
                }
            }
        }
        private decimal _MoneyEarned;
        public decimal MoneyEarned
        {
            get
            {
                return _MoneyEarned;
            }

            set
            {
                if (SetProperty(ref _MoneyEarned, value) == true)
                {
                }
            }
        }
        private int _FirstStock;
        public int FirstStock
        {
            get
            {
                return _FirstStock;
            }
            set
            {
                if (SetProperty(ref _FirstStock, value) == true)
                {
                }
            }
        }
        private int _SecondStock;
        public int SecondStock
        {
            get
            {
                return _SecondStock;
            }
            set
            {
                if (SetProperty(ref _SecondStock, value) == true)
                {
                }
            }
        }
        private bool _CarIsInsured;
        public bool CarIsInsured
        {
            get
            {
                return _CarIsInsured;
            }
            set
            {
                if (SetProperty(ref _CarIsInsured, value) == true)
                {
                }
            }
        }
        private bool _HouseIsInsured;
        public bool HouseIsInsured
        {
            get
            {
                return _HouseIsInsured;
            }
            set
            {
                if (SetProperty(ref _HouseIsInsured, value) == true)
                {
                }
            }
        }
        public bool DegreeObtained { get; set; }
        private int _TilesCollected;
        public int TilesCollected
        {
            get
            {
                return _TilesCollected;
            }
            set
            {
                if (SetProperty(ref _TilesCollected, value) == true)
                {
                }
            }
        }
        private string _HouseName = ""; // has to be this way because its data binding instead of using the old grid.
        public string HouseName
        {
            get
            {
                return _HouseName;
            }
            set
            {
                if (SetProperty(ref _HouseName, value) == true)
                {
                }
            }
        }
        private string _Career1 = "";
        public string Career1
        {
            get
            {
                return _Career1;
            }
            set
            {
                if (SetProperty(ref _Career1, value) == true)
                {
                }
            }
        }
        private string _Career2 = "";
        public string Career2
        {
            get
            {
                return _Career2;
            }
            set
            {
                if (SetProperty(ref _Career2, value) == true)
                {
                }
            }
        }
        public CustomBasicList<TileInfo> TileList { get; set; } = new CustomBasicList<TileInfo>();
        public EnumTurnInfo WhatTurn { get; set; }
        public bool Married { get; set; }
    }
}