using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace A21DiceGameCP.Data
{
    public class A21DiceGamePlayerItem : SimplePlayer
    { //anything needed is here
        private bool _isFaceOff;

        public bool IsFaceOff
        {
            get { return _isFaceOff; }
            set
            {
                if (SetProperty(ref _isFaceOff, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                if (SetProperty(ref _score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _numberOfRolls;

        public int NumberOfRolls
        {
            get { return _numberOfRolls; }
            set
            {
                if (SetProperty(ref _numberOfRolls, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
