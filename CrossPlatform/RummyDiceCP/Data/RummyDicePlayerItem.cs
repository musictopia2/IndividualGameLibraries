using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace RummyDiceCP.Data
{
    public class RummyDicePlayerItem : SimplePlayer
    { //anything needed is here
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
        private int _scoreGame;

        public int ScoreGame
        {
            get { return _scoreGame; }
            set
            {
                if (SetProperty(ref _scoreGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _phase;

        public int Phase
        {
            get { return _phase; }
            set
            {
                if (SetProperty(ref _phase, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public int HowManyRepeats { get; set; }
    }
}
