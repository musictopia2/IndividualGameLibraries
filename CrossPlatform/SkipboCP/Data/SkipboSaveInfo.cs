using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.MultiplePilesObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using SkipboCP.Cards;
namespace SkipboCP.Data
{
    [SingletonGame]
    public class SkipboSaveInfo : BasicSavedCardClass<SkipboPlayerItem, SkipboCardInformation>
    { //anything needed for autoresume is here.

        public CustomBasicList<BasicPileInfo<SkipboCardInformation>> PublicPileList { get; set; } = new CustomBasicList<BasicPileInfo<SkipboCardInformation>>();

        private int _cardsToShuffle;
        public int CardsToShuffle
        {
            get { return _cardsToShuffle; }
            set
            {
                if (SetProperty(ref _cardsToShuffle, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.CardsToShuffle = value;
                }

            }
        }
        public void LoadMod(SkipboVMData model)
        {
            _model = model;
            _model.CardsToShuffle = CardsToShuffle;
        }

        private SkipboVMData? _model;

    }
}