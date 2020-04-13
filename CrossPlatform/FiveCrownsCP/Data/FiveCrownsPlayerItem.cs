using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using FiveCrownsCP.Cards;
namespace FiveCrownsCP.Data
{
    public class FiveCrownsPlayerItem : PlayerRummyHand<FiveCrownsCardInformation>
    { //anything needed is here
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
