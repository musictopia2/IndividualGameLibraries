using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace SnakesAndLaddersCP
{
    public class SnakesAndLaddersPlayerItem : SimplePlayer
    {
        private int _SpaceNumber;
        public int SpaceNumber
        {
            get { return _SpaceNumber; }
            set
            {
                if (SetProperty(ref _SpaceNumber, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}