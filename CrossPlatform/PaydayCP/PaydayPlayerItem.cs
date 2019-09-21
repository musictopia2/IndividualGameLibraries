using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace PaydayCP
{
    public class PaydayPlayerItem : PlayerBoardGame<EnumColorChoice>
    {
        public override bool DidChooseColor => Color != EnumColorChoice.None;
        public override void Clear()
        {
            Color = EnumColorChoice.None;
        }
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
        private decimal _MoneyHas;
        public decimal MoneyHas
        {
            get
            {
                return _MoneyHas;
            }

            set
            {
                if (SetProperty(ref _MoneyHas, value) == true)
                {
                }
            }
        }
        public decimal NetIncome()
        {
            return MoneyHas - Loans;
        }
        private int _CurrentMonth;
        public int CurrentMonth
        {
            get
            {
                return _CurrentMonth;
            }

            set
            {
                if (SetProperty(ref _CurrentMonth, value) == true)
                {
                }
            }
        }
        private int _DayNumber;
        public int DayNumber
        {
            get
            {
                return _DayNumber;
            }

            set
            {
                if (SetProperty(ref _DayNumber, value) == true)
                {
                }
            }
        }
        private int _ChoseNumber;
        public int ChoseNumber
        {
            get
            {
                return _ChoseNumber;
            }

            set
            {
                if (SetProperty(ref _ChoseNumber, value) == true)
                {
                }
            }
        }
        public DeckObservableDict<CardInformation> Hand { get; set; } = new DeckObservableDict<CardInformation>(); //this time, this has a hand.
    }
}