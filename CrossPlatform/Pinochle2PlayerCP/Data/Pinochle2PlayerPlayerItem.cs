using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.Messenging;
using Pinochle2PlayerCP.Cards;
namespace Pinochle2PlayerCP.Data
{
    public class Pinochle2PlayerPlayerItem : PlayerTrick<EnumSuitList, Pinochle2PlayerCardInformation>, IHandle<UpdateCountEventModel>
    { //anything needed is here
        private int _currentScore;

        public int CurrentScore
        {
            get { return _currentScore; }
            set
            {
                if (SetProperty(ref _currentScore, value))
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

        private DeckRegularDict<Pinochle2PlayerCardInformation> _additionalCards = new DeckRegularDict<Pinochle2PlayerCardInformation>();

        public DeckRegularDict<Pinochle2PlayerCardInformation> AdditionalCards
        {
            get { return _additionalCards; }
            set
            {
                if (SetProperty(ref _additionalCards, value))
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