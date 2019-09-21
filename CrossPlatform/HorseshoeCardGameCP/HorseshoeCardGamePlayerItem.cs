using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using Newtonsoft.Json;
namespace HorseshoeCardGameCP
{
    public class HorseshoeCardGamePlayerItem : PlayerTrick<EnumSuitList, HorseshoeCardGameCardInformation>
    { //anything needed is here
        private int _PreviousScore;

        public int PreviousScore
        {
            get { return _PreviousScore; }
            set
            {
                if (SetProperty(ref _PreviousScore, value))
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
        public PlayerBoardViewModel<HorseshoeCardGameCardInformation>? TempHand;

        public DeckRegularDict<HorseshoeCardGameCardInformation> SavedTemp = new DeckRegularDict<HorseshoeCardGameCardInformation>(); //this will hold the saved data
    }
}