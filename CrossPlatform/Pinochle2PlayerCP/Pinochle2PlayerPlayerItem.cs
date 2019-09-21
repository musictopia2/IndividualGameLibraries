using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.Messenging;
namespace Pinochle2PlayerCP
{
    public class Pinochle2PlayerPlayerItem : PlayerTrick<EnumSuitList, Pinochle2PlayerCardInformation>, IHandle<UpdateCountEventModel>
    { //anything needed is here
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

        private DeckRegularDict<Pinochle2PlayerCardInformation> _AdditionalCards = new DeckRegularDict<Pinochle2PlayerCardInformation>();

        public DeckRegularDict<Pinochle2PlayerCardInformation> AdditionalCards
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