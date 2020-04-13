using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using HorseshoeCardGameCP.Cards;
using Newtonsoft.Json;
namespace HorseshoeCardGameCP.Data
{
    public class HorseshoeCardGamePlayerItem : PlayerTrick<EnumSuitList, HorseshoeCardGameCardInformation>
    { //anything needed is here
        private int _previousScore;

        public int PreviousScore
        {
            get { return _previousScore; }
            set
            {
                if (SetProperty(ref _previousScore, value))
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
        public PlayerBoardObservable<HorseshoeCardGameCardInformation>? TempHand;

        public DeckRegularDict<HorseshoeCardGameCardInformation> SavedTemp = new DeckRegularDict<HorseshoeCardGameCardInformation>(); //this will hold the saved data
    }
}
