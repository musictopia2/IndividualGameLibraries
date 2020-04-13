using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace GoFishCP.Data
{
    public class GoFishPlayerItem : PlayerSingleHand<RegularSimpleCard>
    { //anything needed is here
        private int _pairs;

        public int Pairs
        {
            get { return _pairs; }
            set
            {
                if (SetProperty(ref _pairs, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
