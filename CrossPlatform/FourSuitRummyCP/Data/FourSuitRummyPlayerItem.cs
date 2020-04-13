using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using FourSuitRummyCP.Logic;
using Newtonsoft.Json;

namespace FourSuitRummyCP.Data
{
    public class FourSuitRummyPlayerItem : PlayerRummyHand<RegularRummyCard>
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
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();  //this is the saved data.
        [JsonIgnore]
        public MainSets? MainSets; //maybe we need this after all. even though the view model will show separate (?)
    }
}
