using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace SnagCardGameCP
{
    public class SnagCardGamePlayerItem : PlayerTrick<EnumSuitList, SnagCardGameCardInformation>
    { //anything needed is here
        private int _CardsWon;

        public int CardsWon
        {
            get { return _CardsWon; }
            set
            {
                if (SetProperty(ref _CardsWon, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _CurrentPoints;

        public int CurrentPoints
        {
            get { return _CurrentPoints; }
            set
            {
                if (SetProperty(ref _CurrentPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TotalPoints;

        public int TotalPoints
        {
            get { return _TotalPoints; }
            set
            {
                if (SetProperty(ref _TotalPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}