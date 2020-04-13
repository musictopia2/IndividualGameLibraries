using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace SnakesAndLaddersCP.Data
{
    public class SnakesAndLaddersPlayerItem : SimplePlayer
    { //anything needed is here
        private int _spaceNumber;
        public int SpaceNumber
        {
            get { return _spaceNumber; }
            set
            {
                if (SetProperty(ref _spaceNumber, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}