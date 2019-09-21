using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace CribbageCP
{
    public class CribbagePlayerItem : PlayerSingleHand<CribbageCard>
    { //anything needed is here
        private int _ScoreRound;

        public int ScoreRound
        {
            get { return _ScoreRound; }
            set
            {
                if (SetProperty(ref _ScoreRound, value))
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

        private bool _IsSkunk = true; //has to be proven false.

        public bool IsSkunk
        {
            get { return _IsSkunk; }
            set
            {
                if (SetProperty(ref _IsSkunk, value))
                {
                    //can decide what to do when property changes
                }

            }
        }


        private bool _FinishedLooking;

        public bool FinishedLooking
        {
            get { return _FinishedLooking; }
            set
            {
                if (SetProperty(ref _FinishedLooking, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _FirstPosition;

        public int FirstPosition
        {
            get { return _FirstPosition; }
            set
            {
                if (SetProperty(ref _FirstPosition, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _SecondPosition;

        public int SecondPosition
        {
            get { return _SecondPosition; }
            set
            {
                if (SetProperty(ref _SecondPosition, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private bool _ChoseCrib;

        public bool ChoseCrib
        {
            get { return _ChoseCrib; }
            set
            {
                if (SetProperty(ref _ChoseCrib, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}