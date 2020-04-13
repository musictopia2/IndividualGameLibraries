using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using MillebournesCP.Cards;
namespace MillebournesCP.Data
{
    public class MillebournesPlayerItem : PlayerSingleHand<MillebournesCardInformation>
    { //anything needed is here
        private int _team;
        public int Team
        {
            get { return _team; }
            set
            {
                if (SetProperty(ref _team, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _otherTurn;
        public bool OtherTurn
        {
            get { return _otherTurn; }
            set
            {
                if (SetProperty(ref _otherTurn, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _miles;
        public int Miles
        {
            get { return _miles; }
            set
            {
                if (SetProperty(ref _miles, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _otherPoints;
        public int OtherPoints
        {
            get { return _otherPoints; }
            set
            {
                if (SetProperty(ref _otherPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _totalPoints;
        public int TotalPoints
        {
            get { return _totalPoints; }
            set
            {
                if (SetProperty(ref _totalPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _number200s;
        public int Number200s
        {
            get { return _number200s; }
            set
            {
                if (SetProperty(ref _number200s, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
