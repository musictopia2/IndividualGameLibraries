using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SixtySix2PlayerCP
{
    [SingletonGame]
    public class SixtySix2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<SixtySix2PlayerCardInformation> CardList { get; set; } = new DeckRegularDict<SixtySix2PlayerCardInformation>();
        public CustomBasicList<int> CardsForMarriage { get; set; } = new CustomBasicList<int>();
        public int LastTrickWon { get; set; }
        private SixtySix2PlayerViewModel? _thisMod;
        public void LoadMod(SixtySix2PlayerViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisMod.BonusPoints = BonusPoints;
        }
        private int _BonusPoints;
        public int BonusPoints
        {
            get { return _BonusPoints; }
            set
            {
                if (SetProperty(ref _BonusPoints, value))
                {
                    if (_thisMod != null)
                        _thisMod.BonusPoints = value;
                }
            }
        }
    }
}