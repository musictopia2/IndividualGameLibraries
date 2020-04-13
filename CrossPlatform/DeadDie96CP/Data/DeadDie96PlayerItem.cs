using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace DeadDie96CP.Data
{
    public class DeadDie96PlayerItem : SimplePlayer
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
