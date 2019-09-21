using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ColorCards;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace RookCP
{
    [SingletonGame]
    public class RookSaveInfo : BasicSavedTrickGamesClass<EnumColorTypes, RookCardInformation, RookPlayerItem>
    {
        //anything needed for saveinfo is here.
        public int WonSoFar { get; set; }
        public bool DummyPlay { get; set; }
        public int HighestBidder { get; set; }
        public DeckRegularDict<RookCardInformation> NestList { get; set; } = new DeckRegularDict<RookCardInformation>();
        public DeckObservableDict<RookCardInformation> DummyList { get; set; } = new DeckObservableDict<RookCardInformation>();
        public DeckRegularDict<RookCardInformation> CardList { get; set; } = new DeckRegularDict<RookCardInformation>();
        private EnumStatusList _GameStatus;

        public EnumStatusList GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod == null)
                        return;
                    _thisMod.ShowVisibleChange();
                }
            }
        }
        private RookViewModel? _thisMod;

        public void LoadMod(RookViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.ShowVisibleChange();
        }
    }
}