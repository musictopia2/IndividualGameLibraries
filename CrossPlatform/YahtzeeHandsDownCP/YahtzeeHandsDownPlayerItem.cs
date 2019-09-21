using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace YahtzeeHandsDownCP
{
    public class YahtzeeHandsDownPlayerItem : PlayerSingleHand<YahtzeeHandsDownCardInformation>
    {
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
        private int _ScoreRound;
        public int ScoreRound
        {
            get { return _ScoreRound; }
            set
            {
                if (SetProperty(ref _ScoreRound, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _WonLastRound = "";
        public string WonLastRound
        {
            get { return _WonLastRound; }
            set
            {
                if (SetProperty(ref _WonLastRound, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public YahtzeeResults Results { get; set; } = new YahtzeeResults(); //for comparing.
    }
}