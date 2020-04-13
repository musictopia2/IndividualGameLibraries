using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using HuseHeartsCP.Cards;
namespace HuseHeartsCP.Data
{
    public class HuseHeartsPlayerItem : PlayerTrick<EnumSuitList, HuseHeartsCardInformation>
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
        private int _previousScore;

        public int PreviousScore
        {
            get { return _previousScore; }
            set
            {
                if (SetProperty(ref _previousScore, value))
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
        private bool _hadPoints;

        public bool HadPoints
        {
            get { return _hadPoints; }
            set
            {
                if (SetProperty(ref _hadPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public CustomBasicList<int> CardsPassed { get; set; } = new CustomBasicList<int>();
    }
}
