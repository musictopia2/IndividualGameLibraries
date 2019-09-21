using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
namespace FlinchCP
{
    [SingletonGame]
    public class FlinchSaveInfo : BasicSavedCardClass<FlinchPlayerItem, FlinchCardInformation>
    { //anything needed for autoresume is here.
        public int PlayerFound { get; set; }
        public EnumStatusList GameStatus { get; set; }
        public CustomBasicList<BasicPileInfo<FlinchCardInformation>> PublicPileList { get; set; } = new CustomBasicList<BasicPileInfo<FlinchCardInformation>>();
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
        public void LoadMod(FlinchViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.CardsToShuffle = CardsToShuffle;
        }

        private FlinchViewModel? _thisMod;
    }
}