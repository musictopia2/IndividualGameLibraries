using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.Messenging;
namespace TileRummyCP
{
    public class TileRummyPlayerItem : PlayerSingleHand<TileInfo>, IHandle<UpdateCountEventModel>
    { //anything needed is here
        private DeckRegularDict<TileInfo> _AdditionalTileList = new DeckRegularDict<TileInfo>();
        public DeckRegularDict<TileInfo> AdditionalTileList
        {
            get { return _AdditionalTileList; }
            set
            {
                if (SetProperty(ref _AdditionalTileList, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private bool _InitCompleted;
        public bool InitCompleted
        {
            get { return _InitCompleted; }
            set
            {
                if (SetProperty(ref _InitCompleted, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Score;
        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
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