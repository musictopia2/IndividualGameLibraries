using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace YachtRaceCP
{
    public class YachtRacePlayerItem : SimplePlayer
    { //anything needed is here
        private float _Time;

        public float Time
        {
            get { return _Time; }
            set
            {
                if (SetProperty(ref _Time, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}