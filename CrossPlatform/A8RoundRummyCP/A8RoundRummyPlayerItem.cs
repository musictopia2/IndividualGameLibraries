using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace A8RoundRummyCP
{
    public class A8RoundRummyPlayerItem : PlayerSingleHand<A8RoundRummyCardInformation>
    { //anything needed is here
        private int _TotalScore;

        public int TotalScore
        {
            get { return _TotalScore; }
            set
            {
                if (SetProperty(ref _TotalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}