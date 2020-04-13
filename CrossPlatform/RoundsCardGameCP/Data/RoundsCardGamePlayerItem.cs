using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using RoundsCardGameCP.Cards;
namespace RoundsCardGameCP.Data
{
    public class RoundsCardGamePlayerItem : PlayerTrick<EnumSuitList, RoundsCardGameCardInformation>
    { //anything needed is here
        private int _roundsWon;

        public int RoundsWon
        {
            get { return _roundsWon; }
            set
            {
                if (SetProperty(ref _roundsWon, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _currentPoints;

        public int CurrentPoints
        {
            get { return _currentPoints; }
            set
            {
                if (SetProperty(ref _currentPoints, value))
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
