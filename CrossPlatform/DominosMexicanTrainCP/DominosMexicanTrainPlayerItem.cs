using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
namespace DominosMexicanTrainCP
{
    public class DominosMexicanTrainPlayerItem : PlayerSingleHand<MexicanDomino>, IHandle<UpdateCountEventModel>
    {
        private DeckObservableDict<MexicanDomino> _LongestTrainList = new DeckObservableDict<MexicanDomino>();
        public DeckObservableDict<MexicanDomino> LongestTrainList
        {
            get { return _LongestTrainList; }
            set
            {
                if (SetProperty(ref _LongestTrainList, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
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
        private int _PreviousLeft;
        public int PreviousLeft
        {
            get { return _PreviousLeft; }
            set
            {
                if (SetProperty(ref _PreviousLeft, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int TempCards;
        protected override int GetTotalObjectCount => base.GetTotalObjectCount + TempCards;
        public void Handle(UpdateCountEventModel message)
        {
            TempCards = message.ObjectCount;
            ObjectCount = GetTotalObjectCount;
        }
    }
}