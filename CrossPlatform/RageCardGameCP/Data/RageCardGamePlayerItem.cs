using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using RageCardGameCP.Cards;
namespace RageCardGameCP.Data
{
    public class RageCardGamePlayerItem : PlayerTrick<EnumColor, RageCardGameCardInformation>
    { //anything needed is here
        private int _bidAmount;

        public int BidAmount
        {
            get { return _bidAmount; }
            set
            {
                if (SetProperty(ref _bidAmount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _revealBid;

        public bool RevealBid
        {
            get { return _revealBid; }
            set
            {
                if (SetProperty(ref _revealBid, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _correctlyBidded;

        public int CorrectlyBidded
        {
            get { return _correctlyBidded; }
            set
            {
                if (SetProperty(ref _correctlyBidded, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _scoreRound;

        public int ScoreRound
        {
            get { return _scoreRound; }
            set
            {
                if (SetProperty(ref _scoreRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _scoreGame;

        public int ScoreGame
        {
            get { return _scoreGame; }
            set
            {
                if (SetProperty(ref _scoreGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}