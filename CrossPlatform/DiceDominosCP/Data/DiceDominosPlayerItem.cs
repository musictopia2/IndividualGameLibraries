using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace DiceDominosCP.Data
{
    public class DiceDominosPlayerItem : SimplePlayer
    { //anything needed is here
        private int _dominosWon;

        public int DominosWon
        {
            get { return _dominosWon; }
            set
            {
                if (SetProperty(ref _dominosWon, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}