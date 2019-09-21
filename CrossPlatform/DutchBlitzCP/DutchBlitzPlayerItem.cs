using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace DutchBlitzCP
{
    public class DutchBlitzPlayerItem : PlayerSingleHand<DutchBlitzCardInformation>
    { //anything needed is here
        private int _StockLeft;

        public int StockLeft
        {
            get { return _StockLeft; }
            set
            {
                if (SetProperty(ref _StockLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PointsRound;

        public int PointsRound
        {
            get { return _PointsRound; }
            set
            {
                if (SetProperty(ref _PointsRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PointsGame;

        public int PointsGame
        {
            get { return _PointsGame; }
            set
            {
                if (SetProperty(ref _PointsGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}