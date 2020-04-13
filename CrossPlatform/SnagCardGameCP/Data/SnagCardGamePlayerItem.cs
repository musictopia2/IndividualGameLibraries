using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SnagCardGameCP.Cards;
namespace SnagCardGameCP.Data
{
    public class SnagCardGamePlayerItem : PlayerTrick<EnumSuitList, SnagCardGameCardInformation>
    { //anything needed is here
        private int _cardsWon;

        public int CardsWon
        {
            get { return _cardsWon; }
            set
            {
                if (SetProperty(ref _cardsWon, value))
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
        private int _totalPoints;

        public int TotalPoints
        {
            get { return _totalPoints; }
            set
            {
                if (SetProperty(ref _totalPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
