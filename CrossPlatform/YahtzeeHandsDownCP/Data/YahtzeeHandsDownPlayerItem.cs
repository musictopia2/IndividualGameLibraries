using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using YahtzeeHandsDownCP.Cards;
namespace YahtzeeHandsDownCP.Data
{
    public class YahtzeeHandsDownPlayerItem : PlayerSingleHand<YahtzeeHandsDownCardInformation>
    { //anything needed is here
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
        private int _scoreRound;
        public int ScoreRound
        {
            get { return _scoreRound; }
            set
            {
                if (SetProperty(ref _scoreRound, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _wonLastRound = "";
        public string WonLastRound
        {
            get { return _wonLastRound; }
            set
            {
                if (SetProperty(ref _wonLastRound, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public YahtzeeResults Results { get; set; } = new YahtzeeResults(); //for comparing.
    }
}
