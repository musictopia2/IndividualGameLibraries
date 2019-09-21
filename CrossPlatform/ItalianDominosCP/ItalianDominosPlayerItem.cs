using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace ItalianDominosCP
{
    public class ItalianDominosPlayerItem : PlayerSingleHand<SimpleDominoInfo>
    {

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
        private bool _DrewYet;

        public bool DrewYet
        {
            get { return _DrewYet; }
            set
            {
                if (SetProperty(ref _DrewYet, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}