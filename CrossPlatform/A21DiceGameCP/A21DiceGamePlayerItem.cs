using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace A21DiceGameCP
{
    public class A21DiceGamePlayerItem : SimplePlayer
    { //anything needed is here
        private bool _IsFaceOff;

        public bool IsFaceOff
        {
            get { return _IsFaceOff; }
            set
            {
                if (SetProperty(ref _IsFaceOff, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _NumberOfRolls;

        public int NumberOfRolls
        {
            get { return _NumberOfRolls; }
            set
            {
                if (SetProperty(ref _NumberOfRolls, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}