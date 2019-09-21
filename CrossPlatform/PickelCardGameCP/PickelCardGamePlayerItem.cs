using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace PickelCardGameCP
{
    public class PickelCardGamePlayerItem : PlayerTrick<EnumSuitList, PickelCardGameCardInformation>
    { //anything needed is here
        private EnumSuitList _SuitDesired;

        public EnumSuitList SuitDesired
        {
            get { return _SuitDesired; }
            set
            {
                if (SetProperty(ref _SuitDesired, value))
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