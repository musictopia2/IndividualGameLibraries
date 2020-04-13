using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace LottoDominosCP.Data
{
    public class LottoDominosPlayerItem : SimplePlayer
    { //anything needed is here
        private int _numberChosen = -1;
        public int NumberChosen
        {
            get { return _numberChosen; }
            set
            {
                if (SetProperty(ref _numberChosen, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _numberWon;
        public int NumberWon
        {
            get { return _numberWon; }
            set
            {
                if (SetProperty(ref _numberWon, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}
