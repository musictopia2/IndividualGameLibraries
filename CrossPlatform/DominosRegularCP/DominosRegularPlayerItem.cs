using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace DominosRegularCP
{
    public class DominosRegularPlayerItem : PlayerSingleHand<SimpleDominoInfo>
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
        private bool _NoPlay;
        public bool NoPlay
        {
            get { return _NoPlay; }
            set
            {
                if (SetProperty(ref _NoPlay, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}