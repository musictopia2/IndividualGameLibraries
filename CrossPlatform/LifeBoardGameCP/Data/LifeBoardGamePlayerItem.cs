using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using LifeBoardGameCP.Cards;
using Newtonsoft.Json;

namespace LifeBoardGameCP.Data
{
    public class LifeBoardGamePlayerItem : PlayerBoardGame<EnumColorChoice>
    { //anything needed is here
        [JsonIgnore]
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
        private decimal _loans;
        public decimal Loans
        {
            get
            {
                return _loans;
            }
            set
            {
                if (SetProperty(ref _loans, value) == true)
                {
                }
            }
        }
        private decimal _salary;
        public decimal Salary
        {
            get
            {
                return _salary;
            }

            set
            {
                if (SetProperty(ref _salary, value) == true)
                {
                }
            }
        }
        private decimal _moneyEarned;
        public decimal MoneyEarned
        {
            get
            {
                return _moneyEarned;
            }

            set
            {
                if (SetProperty(ref _moneyEarned, value) == true)
                {
                }
            }
        }
        private int _firstStock;
        public int FirstStock
        {
            get
            {
                return _firstStock;
            }
            set
            {
                if (SetProperty(ref _firstStock, value) == true)
                {
                }
            }
        }
        private int _secondStock;
        public int SecondStock
        {
            get
            {
                return _secondStock;
            }
            set
            {
                if (SetProperty(ref _secondStock, value) == true)
                {
                }
            }
        }
        private bool _carIsInsured;
        public bool CarIsInsured
        {
            get
            {
                return _carIsInsured;
            }
            set
            {
                if (SetProperty(ref _carIsInsured, value) == true)
                {
                }
            }
        }
        private bool _houseIsInsured;
        public bool HouseIsInsured
        {
            get
            {
                return _houseIsInsured;
            }
            set
            {
                if (SetProperty(ref _houseIsInsured, value) == true)
                {
                }
            }
        }
        public bool DegreeObtained { get; set; }
        private int _tilesCollected;
        public int TilesCollected
        {
            get
            {
                return _tilesCollected;
            }
            set
            {
                if (SetProperty(ref _tilesCollected, value) == true)
                {
                }
            }
        }
        private string _houseName = ""; // has to be this way because its data binding instead of using the old grid.
        public string HouseName
        {
            get
            {
                return _houseName;
            }
            set
            {
                if (SetProperty(ref _houseName, value) == true)
                {
                }
            }
        }
        private string _career1 = "";
        public string Career1
        {
            get
            {
                return _career1;
            }
            set
            {
                if (SetProperty(ref _career1, value) == true)
                {
                }
            }
        }
        private string _career2 = "";
        public string Career2
        {
            get
            {
                return _career2;
            }
            set
            {
                if (SetProperty(ref _career2, value) == true)
                {
                }
            }
        }
        public CustomBasicList<TileInfo> TileList { get; set; } = new CustomBasicList<TileInfo>();
        public EnumTurnInfo WhatTurn { get; set; }
        public bool Married { get; set; }
    }
}