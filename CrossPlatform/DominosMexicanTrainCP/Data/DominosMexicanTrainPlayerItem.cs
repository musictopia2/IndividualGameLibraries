using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
namespace DominosMexicanTrainCP.Data
{
    public class DominosMexicanTrainPlayerItem : PlayerSingleHand<MexicanDomino>, IHandle<UpdateCountEventModel>
    { //anything needed is here
      //if not needed, take out.
        private DeckObservableDict<MexicanDomino> _longestTrainList = new DeckObservableDict<MexicanDomino>();
        public DeckObservableDict<MexicanDomino> LongestTrainList
        {
            get { return _longestTrainList; }
            set
            {
                if (SetProperty(ref _longestTrainList, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
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
        private int _previousLeft;
        public int PreviousLeft
        {
            get { return _previousLeft; }
            set
            {
                if (SetProperty(ref _previousLeft, value))
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
