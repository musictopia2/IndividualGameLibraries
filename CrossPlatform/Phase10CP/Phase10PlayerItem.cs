using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
namespace Phase10CP
{
    public class Phase10PlayerItem : PlayerSingleHand<Phase10CardInformation>, IHandle<UpdateCountEventModel>
    { //anything needed is here
        private int _Phase = 1;
        public int Phase
        {
            get { return _Phase; }
            set
            {
                if (SetProperty(ref _Phase, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _Completed;

        public bool Completed
        {
            get { return _Completed; }
            set
            {
                if (SetProperty(ref _Completed, value))
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

        protected override int GetTotalObjectCount => base.GetTotalObjectCount + _tempCards;

        private int _tempCards;

        private DeckRegularDict<Phase10CardInformation> _AdditionalCards = new DeckRegularDict<Phase10CardInformation>();

        public DeckRegularDict<Phase10CardInformation> AdditionalCards
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
        public void Handle(UpdateCountEventModel message)
        {
            _tempCards = message.ObjectCount;
            ObjectCount = GetTotalObjectCount; //this needs to be done too because something else changed.
            //you may have removed cards from your hand but the object count has to be the same.
        }
    }
}