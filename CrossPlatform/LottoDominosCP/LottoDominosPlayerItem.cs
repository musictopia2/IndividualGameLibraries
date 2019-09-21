using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace LottoDominosCP
{
    public class LottoDominosPlayerItem : SimplePlayer
    {
        private int _NumberChosen = -1;
        public int NumberChosen
        {
            get { return _NumberChosen; }
            set
            {
                if (SetProperty(ref _NumberChosen, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _NumberWon;
        public int NumberWon
        {
            get { return _NumberWon; }
            set
            {
                if (SetProperty(ref _NumberWon, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}