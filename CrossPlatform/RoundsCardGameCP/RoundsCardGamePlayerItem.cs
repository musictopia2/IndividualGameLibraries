using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace RoundsCardGameCP
{
    public class RoundsCardGamePlayerItem : PlayerTrick<EnumSuitList, RoundsCardGameCardInformation>
    { //anything needed is here
        private int _RoundsWon;

        public int RoundsWon
        {
            get { return _RoundsWon; }
            set
            {
                if (SetProperty(ref _RoundsWon, value))
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