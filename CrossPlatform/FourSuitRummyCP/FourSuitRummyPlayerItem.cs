using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using Newtonsoft.Json;
namespace FourSuitRummyCP
{
    public class FourSuitRummyPlayerItem : PlayerSingleHand<RegularRummyCard>, IHandle<UpdateCountEventModel>
    {
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
        private DeckRegularDict<RegularRummyCard> _AdditionalCards = new DeckRegularDict<RegularRummyCard>();

        public DeckRegularDict<RegularRummyCard> AdditionalCards
        {
            get { return _AdditionalCards; }
            set
            {
                if (SetProperty(ref _AdditionalCards, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _tempCards;
        protected override int GetTotalObjectCount => base.GetTotalObjectCount + _tempCards;
        public void Handle(UpdateCountEventModel message)
        {
            _tempCards = message.ObjectCount;
            ObjectCount = GetTotalObjectCount;
        }
        [JsonIgnore]
        public MainSets? MainSets;
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();  //this is the saved data.
    }
}