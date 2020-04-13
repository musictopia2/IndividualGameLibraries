using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace CousinRummyCP.Data
{
    public class CousinRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
    { //anything needed is here
        private int _tokensLeft;

        public int TokensLeft
        {
            get { return _tokensLeft; }
            set
            {
                if (SetProperty(ref _tokensLeft, value))
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
        public bool LaidDown { get; set; }

    }
}
