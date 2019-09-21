using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace GolfCardGameCP
{
    public class GolfCardGamePlayerItem : PlayerSingleHand<RegularSimpleCard>
    { //anything needed is here
        private bool _Knocked;

        public bool Knocked
        {
            get { return _Knocked; }
            set
            {
                if (SetProperty(ref _Knocked, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private bool _FirstChanged;

        public bool FirstChanged
        {
            get { return _FirstChanged; }
            set
            {
                if (SetProperty(ref _FirstChanged, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private bool _SecondChanged;

        public bool SecondChanged
        {
            get { return _SecondChanged; }
            set
            {
                if (SetProperty(ref _SecondChanged, value))
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

        private bool _FinishedChoosing;

        public bool FinishedChoosing
        {
            get { return _FinishedChoosing; }
            set
            {
                if (SetProperty(ref _FinishedChoosing, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public DeckObservableDict<RegularSimpleCard> TempSets { get; set; } = new DeckObservableDict<RegularSimpleCard>();
    }
}