using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Messenging;
namespace OpetongCP
{
    public class OpetongPlayerItem : PlayerSingleHand<RegularRummyCard>, IHandle<UpdateCountEventModel>
    { //anything needed is here
        private int _SetsPlayed;

        public int SetsPlayed
        {
            get { return _SetsPlayed; }
            set
            {
                if (SetProperty(ref _SetsPlayed, value))
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
        private int TempCards;

        protected override int GetTotalObjectCount => base.GetTotalObjectCount + TempCards;

        public void Handle(UpdateCountEventModel Message)
        {
            TempCards = Message.ObjectCount;
            ObjectCount = GetTotalObjectCount;
        }
    }
}