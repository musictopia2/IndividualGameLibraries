using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace Rummy500CP
{
    public class Rummy500PlayerItem : PlayerSingleHand<RegularRummyCard>
    { //anything needed is here
        private int _PointsPlayed;

        public int PointsPlayed
        {
            get { return _PointsPlayed; }
            set
            {
                if (SetProperty(ref _PointsPlayed, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _CardsPlayed;

        public int CardsPlayed
        {
            get { return _CardsPlayed; }
            set
            {
                if (SetProperty(ref _CardsPlayed, value))
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