using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace MillebournesCP
{
    public class MillebournesPlayerItem : PlayerSingleHand<MillebournesCardInformation>
    {
        private int _Team;
        public int Team
        {
            get { return _Team; }
            set
            {
                if (SetProperty(ref _Team, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _OtherTurn;
        public bool OtherTurn
        {
            get { return _OtherTurn; }
            set
            {
                if (SetProperty(ref _OtherTurn, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _Miles;
        public int Miles
        {
            get { return _Miles; }
            set
            {
                if (SetProperty(ref _Miles, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _OtherPoints;
        public int OtherPoints
        {
            get { return _OtherPoints; }
            set
            {
                if (SetProperty(ref _OtherPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TotalPoints;
        public int TotalPoints
        {
            get { return _TotalPoints; }
            set
            {
                if (SetProperty(ref _TotalPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Number200s;
        public int Number200s
        {
            get { return _Number200s; }
            set
            {
                if (SetProperty(ref _Number200s, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}