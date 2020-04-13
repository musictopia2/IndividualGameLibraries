using A8RoundRummyCP.Cards;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
namespace A8RoundRummyCP.Data
{
    public class A8RoundRummyPlayerItem : PlayerSingleHand<A8RoundRummyCardInformation>
    { //anything needed is here
        private int _totalScore;

        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                if (SetProperty(ref _totalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
