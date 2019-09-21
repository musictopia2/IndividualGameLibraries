using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace Spades2PlayerCP
{
    public class Spades2PlayerPlayerItem : PlayerTrick<EnumSuitList, Spades2PlayerCardInformation>
    { //anything needed is here
        private int _HowManyBids;

        public int HowManyBids
        {
            get { return _HowManyBids; }
            set
            {
                if (SetProperty(ref _HowManyBids, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Bags;

        public int Bags
        {
            get { return _Bags; }
            set
            {
                if (SetProperty(ref _Bags, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _CurrentScore;

        public int CurrentScore
        {
            get { return _CurrentScore; }
            set
            {
                if (SetProperty(ref _CurrentScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
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
        private bool IsNil => HowManyBids == 0;
        public void CalculateScore()
        {
            bool nils = IsNil;
            int thisScore = 0;
            int bids = HowManyBids;
            if (TricksWon > 0 && IsNil == true)
            {
                thisScore = -100;
                thisScore += TricksWon;
                Bags += TricksWon;
            }
            else if (nils == true)
                thisScore += 100;
            else if (TricksWon < bids)
            {
                thisScore = bids * 10;
                thisScore *= -1;
            }
            else
            {
                thisScore = bids * 10;
                int Extras = TricksWon - bids;
                thisScore += Extras;
                Bags += Extras;
            }
            if (Bags >= 20)
            {
                thisScore -= 200;
                Bags -= 20;
            }
            else if (Bags >= 10)
            {
                thisScore -= 100;
                Bags -= 10;
            }
            CurrentScore = thisScore;
            TotalScore += CurrentScore;
        }
    }
}