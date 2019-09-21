using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace RageCardGameCP
{
    public class RageCardGamePlayerItem : PlayerTrick<EnumColor, RageCardGameCardInformation>
    { //anything needed is here
        private int _BidAmount;

        public int BidAmount
        {
            get { return _BidAmount; }
            set
            {
                if (SetProperty(ref _BidAmount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _RevealBid;

        public bool RevealBid
        {
            get { return _RevealBid; }
            set
            {
                if (SetProperty(ref _RevealBid, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _CorrectlyBidded;

        public int CorrectlyBidded
        {
            get { return _CorrectlyBidded; }
            set
            {
                if (SetProperty(ref _CorrectlyBidded, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _ScoreRound;

        public int ScoreRound
        {
            get { return _ScoreRound; }
            set
            {
                if (SetProperty(ref _ScoreRound, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _ScoreGame;

        public int ScoreGame
        {
            get { return _ScoreGame; }
            set
            {
                if (SetProperty(ref _ScoreGame, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}