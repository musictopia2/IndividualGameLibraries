using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using Phase10CP.Cards;
namespace Phase10CP.Data
{
    public class Phase10PlayerItem : PlayerRummyHand<Phase10CardInformation>
    { //anything needed is here
        private int _phase = 1;
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
        private bool _completed;

        public bool Completed
        {
            get { return _completed; }
            set
            {
                if (SetProperty(ref _completed, value))
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
