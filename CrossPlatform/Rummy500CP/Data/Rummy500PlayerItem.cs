using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace Rummy500CP.Data
{
    public class Rummy500PlayerItem : PlayerSingleHand<RegularRummyCard>
    { //anything needed is here
        private int _pointsPlayed;

        public int PointsPlayed
        {
            get { return _pointsPlayed; }
            set
            {
                if (SetProperty(ref _pointsPlayed, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _cardsPlayed;

        public int CardsPlayed
        {
            get { return _cardsPlayed; }
            set
            {
                if (SetProperty(ref _cardsPlayed, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _currentScore;

        public int CurrentScore
        {
            get { return _currentScore; }
            set
            {
                if (SetProperty(ref _currentScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _totalScore;

        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                if (SetProperty(ref _totalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
