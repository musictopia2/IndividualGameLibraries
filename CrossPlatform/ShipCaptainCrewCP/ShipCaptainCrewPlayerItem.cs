using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace ShipCaptainCrewCP
{
    public class ShipCaptainCrewPlayerItem : SimplePlayer
    { //anything needed is here
        private bool _WentOut;
        public bool WentOut
        {
            get { return _WentOut; }
            set
            {
                if (SetProperty(ref _WentOut, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _Score;
        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _Wins;
        public int Wins
        {
            get { return _Wins; }
            set
            {
                if (SetProperty(ref _Wins, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}