using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Messenging;
namespace CousinRummyCP
{
    public class CousinRummyPlayerItem : PlayerSingleHand<RegularRummyCard>, IHandle<UpdateCountEventModel>
    { //anything needed is here
        private int _TokensLeft;

        public int TokensLeft
        {
            get { return _TokensLeft; }
            set
            {
                if (SetProperty(ref _TokensLeft, value))
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
        public bool LaidDown { get; set; }
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
    }
}