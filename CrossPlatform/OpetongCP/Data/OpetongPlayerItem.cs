using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Messenging;
namespace OpetongCP.Data
{
    public class OpetongPlayerItem : PlayerRummyHand<RegularRummyCard>
    { //anything needed is here
        private int _setsPlayed;

        public int SetsPlayed
        {
            get { return _setsPlayed; }
            set
            {
                if (SetProperty(ref _setsPlayed, value))
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