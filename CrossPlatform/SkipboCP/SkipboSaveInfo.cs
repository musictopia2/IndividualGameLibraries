using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SkipboCP
{
    [SingletonGame]
    public class SkipboSaveInfo : BasicSavedCardClass<SkipboPlayerItem, SkipboCardInformation>
    { //anything needed for autoresume is here.
        public CustomBasicList<BasicPileInfo<SkipboCardInformation>> PublicPileList { get; set; } = new CustomBasicList<BasicPileInfo<SkipboCardInformation>>();

        private int _CardsToShuffle;
        public int CardsToShuffle
        {
            get { return _CardsToShuffle; }
            set
            {
                if (SetProperty(ref _CardsToShuffle, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.CardsToShuffle = value;
                }

            }
        }
        public void LoadMod(SkipboViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.CardsToShuffle = CardsToShuffle;
        }

        private SkipboViewModel? _thisMod;
    }
}