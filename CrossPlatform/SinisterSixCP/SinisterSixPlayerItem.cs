using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace SinisterSixCP
{
    public class SinisterSixPlayerItem : SimplePlayer
    { //anything needed is here
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
    }
}