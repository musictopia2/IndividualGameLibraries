using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace GolfCardGameCP.Data
{
    public class GolfCardGamePlayerItem : PlayerSingleHand<RegularSimpleCard>
    { //anything needed is here
        private bool _knocked;

        public bool Knocked
        {
            get { return _knocked; }
            set
            {
                if (SetProperty(ref _knocked, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private bool _firstChanged;

        public bool FirstChanged
        {
            get { return _firstChanged; }
            set
            {
                if (SetProperty(ref _firstChanged, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private bool _secondChanged;

        public bool SecondChanged
        {
            get { return _secondChanged; }
            set
            {
                if (SetProperty(ref _secondChanged, value))
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

        private bool _finishedChoosing;

        public bool FinishedChoosing
        {
            get { return _finishedChoosing; }
            set
            {
                if (SetProperty(ref _finishedChoosing, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public DeckObservableDict<RegularSimpleCard> TempSets { get; set; } = new DeckObservableDict<RegularSimpleCard>();
    }
}