using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using Newtonsoft.Json;
namespace SkuckCardGameCP
{
    public class SkuckCardGamePlayerItem : PlayerTrick<EnumSuitList, SkuckCardGameCardInformation>
    { //anything needed is here
        private int _StrengthHand;

        public int StrengthHand
        {
            get { return _StrengthHand; }
            set
            {
                if (SetProperty(ref _StrengthHand, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _TieBreaker = "";

        public string TieBreaker
        {
            get { return _TieBreaker; }
            set
            {
                if (SetProperty(ref _TieBreaker, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
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
        private bool _BidVisible;

        public bool BidVisible
        {
            get { return _BidVisible; }
            set
            {
                if (SetProperty(ref _BidVisible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _PerfectRounds;

        public int PerfectRounds
        {
            get { return _PerfectRounds; }
            set
            {
                if (SetProperty(ref _PerfectRounds, value))
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
        [JsonIgnore]
        public PlayerBoardViewModel<SkuckCardGameCardInformation>? TempHand;

        public DeckRegularDict<SkuckCardGameCardInformation> SavedTemp = new DeckRegularDict<SkuckCardGameCardInformation>(); //this will hold the saved data
    }
}