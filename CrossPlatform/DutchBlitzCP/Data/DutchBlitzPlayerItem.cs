using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using DutchBlitzCP.Cards;
namespace DutchBlitzCP.Data
{
    public class DutchBlitzPlayerItem : PlayerSingleHand<DutchBlitzCardInformation>
    { //anything needed is here
        private int _stockLeft;

        public int StockLeft
        {
            get { return _stockLeft; }
            set
            {
                if (SetProperty(ref _stockLeft, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _pointsRound;

        public int PointsRound
        {
            get { return _pointsRound; }
            set
            {
                if (SetProperty(ref _pointsRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _pointsGame;

        public int PointsGame
        {
            get { return _pointsGame; }
            set
            {
                if (SetProperty(ref _pointsGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}
