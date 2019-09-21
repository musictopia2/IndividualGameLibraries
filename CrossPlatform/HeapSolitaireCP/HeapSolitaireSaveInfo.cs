using BasicGameFramework.Attributes;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace HeapSolitaireCP
{
    [SingletonGame]
    public class HeapSolitaireSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        //anything else needed to save a game will be here.
        public int PreviousSelected { get; set; }
        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.Score = value;
                }

            }
        }
        public CustomBasicList<BasicPileInfo<HeapSolitaireCardInfo>>? WasteData { get; set; }
        public CustomBasicList<BasicPileInfo<HeapSolitaireCardInfo>>? MainPiles { get; set; }
        private HeapSolitaireViewModel? _thisMod;
        public void LoadMod(HeapSolitaireViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.Score = Score;
        }
    }
}
