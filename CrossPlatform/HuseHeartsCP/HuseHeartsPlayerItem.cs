using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace HuseHeartsCP
{
    public class HuseHeartsPlayerItem : PlayerTrick<EnumSuitList, HuseHeartsCardInformation>
    { //anything needed is here
        private int _CurrentScore;

        public int CurrentScore
        {
            get { return _CurrentScore; }
            set
            {
                if (SetProperty(ref _CurrentScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PreviousScore;

        public int PreviousScore
        {
            get { return _PreviousScore; }
            set
            {
                if (SetProperty(ref _PreviousScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
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
        private bool _HadPoints;

        public bool HadPoints
        {
            get { return _HadPoints; }
            set
            {
                if (SetProperty(ref _HadPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public CustomBasicList<int> CardsPassed { get; set; } = new CustomBasicList<int>();
    }
}