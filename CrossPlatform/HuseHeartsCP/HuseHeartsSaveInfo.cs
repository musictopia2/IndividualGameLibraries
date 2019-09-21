using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
namespace HuseHeartsCP
{
    [SingletonGame]
    public class HuseHeartsSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, HuseHeartsCardInformation, HuseHeartsPlayerItem>, ITrickStatusSavedClass
    { //anything needed for autoresume is here.
        public DeckRegularDict<HuseHeartsCardInformation> BlindList { get; set; } = new DeckRegularDict<HuseHeartsCardInformation>();
        public DeckObservableDict<HuseHeartsCardInformation> DummyList { get; set; } = new DeckObservableDict<HuseHeartsCardInformation>();
        public EnumTrickStatus TrickStatus { get; set; }
        public int WhoLeadsTrick { get; set; }
        public int WhoWinsBlind { get; set; }

        private int _RoundNumber;

        public int RoundNumber
        {
            get { return _RoundNumber; }
            set
            {
                if (SetProperty(ref _RoundNumber, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.RoundNumber = value;
                }

            }
        }
        private EnumStatus _GameStatus;

        public EnumStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.ShowVisibleChange();
                }

            }
        }
        private HuseHeartsViewModel? _thisMod;
        public void LoadMod(HuseHeartsViewModel thisMod)
        {
            this._thisMod = thisMod;
            thisMod.RoundNumber = RoundNumber;
            thisMod.ShowVisibleChange(); //i think here too.
        }
    }
}