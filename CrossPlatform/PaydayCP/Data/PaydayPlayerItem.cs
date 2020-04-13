using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using Newtonsoft.Json;
using PaydayCP.Cards;

namespace PaydayCP.Data
{
    public class PaydayPlayerItem : PlayerBoardGame<EnumColorChoice>
    { //anything needed is here
        [JsonIgnore]
        public override bool DidChooseColor => Color != EnumColorChoice.None;

        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
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
        private decimal _moneyHas;
        public decimal MoneyHas
        {
            get
            {
                return _moneyHas;
            }

            set
            {
                if (SetProperty(ref _moneyHas, value) == true)
                {
                }
            }
        }
        public decimal NetIncome()
        {
            return MoneyHas - Loans;
        }
        private int _currentMonth;
        public int CurrentMonth
        {
            get
            {
                return _currentMonth;
            }

            set
            {
                if (SetProperty(ref _currentMonth, value) == true)
                {
                }
            }
        }
        private int _dayNumber;
        public int DayNumber
        {
            get
            {
                return _dayNumber;
            }

            set
            {
                if (SetProperty(ref _dayNumber, value) == true)
                {
                }
            }
        }
        private int _choseNumber; //-1 means will never play again.
        public int ChoseNumber
        {
            get
            {
                return _choseNumber;
            }

            set
            {
                if (SetProperty(ref _choseNumber, value) == true)
                {
                }
            }
        }
        public DeckObservableDict<CardInformation> Hand { get; set; } = new DeckObservableDict<CardInformation>();
    }
}