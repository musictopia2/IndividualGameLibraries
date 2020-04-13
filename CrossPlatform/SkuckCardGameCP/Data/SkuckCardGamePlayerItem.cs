using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using Newtonsoft.Json;
using SkuckCardGameCP.Cards;
namespace SkuckCardGameCP.Data
{
    public class SkuckCardGamePlayerItem : PlayerTrick<EnumSuitList, SkuckCardGameCardInformation>
    { //anything needed is here
        private int _strengthHand;

        public int StrengthHand
        {
            get { return _strengthHand; }
            set
            {
                if (SetProperty(ref _strengthHand, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _tieBreaker = "";

        public string TieBreaker
        {
            get { return _tieBreaker; }
            set
            {
                if (SetProperty(ref _tieBreaker, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
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
        private bool _bidVisible;

        public bool BidVisible
        {
            get { return _bidVisible; }
            set
            {
                if (SetProperty(ref _bidVisible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _perfectRounds;

        public int PerfectRounds
        {
            get { return _perfectRounds; }
            set
            {
                if (SetProperty(ref _perfectRounds, value))
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
        [JsonIgnore]
        public PlayerBoardObservable<SkuckCardGameCardInformation>? TempHand;

        public DeckRegularDict<SkuckCardGameCardInformation> SavedTemp = new DeckRegularDict<SkuckCardGameCardInformation>();
    }
}
