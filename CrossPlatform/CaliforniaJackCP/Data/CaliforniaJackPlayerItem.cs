using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CaliforniaJackCP.Cards;
namespace CaliforniaJackCP.Data
{
    public class CaliforniaJackPlayerItem : PlayerTrick<EnumSuitList, CaliforniaJackCardInformation>
    { //anything needed is here
        private int _points;

        public int Points
        {
            get { return _points; }
            set
            {
                if (SetProperty(ref _points, value))
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
