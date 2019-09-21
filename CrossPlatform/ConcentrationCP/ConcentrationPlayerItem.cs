using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace ConcentrationCP
{
    public class ConcentrationPlayerItem : PlayerSingleHand<RegularSimpleCard>
    { //anything needed is here
        private int _Pairs;

        public int Pairs
        {
            get { return _Pairs; }
            set
            {
                if (SetProperty(ref _Pairs, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}