using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace UnoCP
{
    public class UnoPlayerItem : PlayerSingleHand<UnoCardInformation>
    { //anything needed is here
        private int _TotalPoints;
        public int TotalPoints
        {
            get
            {
                return _TotalPoints;
            }

            set
            {
                if (SetProperty(ref _TotalPoints, value) == true)
                {
                }
            }
        }

        private int _PreviousPoints;
        public int PreviousPoints
        {
            get
            {
                return _PreviousPoints;
            }

            set
            {
                if (SetProperty(ref _PreviousPoints, value) == true)
                {
                }
            }
        }
    }
}