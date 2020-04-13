using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace YachtRaceCP.Data
{
    public class YachtRacePlayerItem : SimplePlayer
    { //anything needed is here
        private float _time;

        public float Time
        {
            get { return _time; }
            set
            {
                if (SetProperty(ref _time, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
