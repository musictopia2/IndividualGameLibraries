using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace ThinkTwiceCP
{
    public class ThinkTwicePlayerItem : SimplePlayer
    { //anything needed is here
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
        private int _ScoreGame;

        public int ScoreGame
        {
            get { return _ScoreGame; }
            set
            {
                if (SetProperty(ref _ScoreGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}