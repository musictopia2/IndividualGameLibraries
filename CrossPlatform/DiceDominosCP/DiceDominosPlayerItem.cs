using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace DiceDominosCP
{
    public class DiceDominosPlayerItem : SimplePlayer
    { //anything needed is here
        private int _DominosWon;

        public int DominosWon
        {
            get { return _DominosWon; }
            set
            {
                if (SetProperty(ref _DominosWon, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}