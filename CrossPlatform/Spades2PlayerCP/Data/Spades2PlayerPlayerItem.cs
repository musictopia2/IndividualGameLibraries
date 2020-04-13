using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using Spades2PlayerCP.Cards;
namespace Spades2PlayerCP.Data
{
    public class Spades2PlayerPlayerItem : PlayerTrick<EnumSuitList, Spades2PlayerCardInformation>
    { //anything needed is here
        private int _howManyBids;

        public int HowManyBids
        {
            get { return _howManyBids; }
            set
            {
                if (SetProperty(ref _howManyBids, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _bags;

        public int Bags
        {
            get { return _bags; }
            set
            {
                if (SetProperty(ref _bags, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _currentScore;

        public int CurrentScore
        {
            get { return _currentScore; }
            set
            {
                if (SetProperty(ref _currentScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
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