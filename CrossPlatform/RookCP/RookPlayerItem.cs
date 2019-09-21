using BasicGameFramework.ColorCards;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace RookCP
{
    public class RookPlayerItem : PlayerTrick<EnumColorTypes, RookCardInformation>
    { //anything needed is here
        private bool _IsDummy;

        public bool IsDummy
        {
            get { return _IsDummy; }
            set
            {
                if (SetProperty(ref _IsDummy, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _Pass;

        public bool Pass
        {
            get { return _Pass; }
            set
            {
                if (SetProperty(ref _Pass, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _BidAmount;

        public int BidAmount
        {
            get { return _BidAmount; }
            set
            {
                if (SetProperty(ref _BidAmount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _CurrentScore;

        public int CurrentScore
        {
            get { return _CurrentScore; }
            set
            {
                if (SetProperty(ref _CurrentScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TotalScore;

        public int TotalScore
        {
            get { return _TotalScore; }
            set
            {
                if (SetProperty(ref _TotalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}