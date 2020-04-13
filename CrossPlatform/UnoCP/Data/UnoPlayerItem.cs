using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using UnoCP.Cards;
namespace UnoCP.Data
{
    public class UnoPlayerItem : PlayerSingleHand<UnoCardInformation>
    { //anything needed is here
        private int _totalPoints;
        public int TotalPoints
        {
            get
            {
                return _totalPoints;
            }

            set
            {
                if (SetProperty(ref _totalPoints, value) == true)
                {
                }
            }
        }

        private int _previousPoints;
        public int PreviousPoints
        {
            get
            {
                return _previousPoints;
            }

            set
            {
                if (SetProperty(ref _previousPoints, value) == true)
                {
                }
            }
        }
    }
}
