using BasicGameFramework.Attributes;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
namespace Spades2PlayerCP
{
    [SingletonGame]
    public class Spades2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem>, ITrickStatusSavedClass
    { //anything needed for autoresume is here.
        public bool FirstCard { get; set; }
        private EnumGameStatus _GameStatus;

        public EnumGameStatus GameStatus
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
        public SavedDiscardPile<Spades2PlayerCardInformation>? OtherPile { get; set; }
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
        private Spades2PlayerViewModel? _thisMod;
        public void LoadMod(Spades2PlayerViewModel thisMod)
        {
            _thisMod = thisMod;
            thisMod.RoundNumber = RoundNumber;
            thisMod.ShowVisibleChange(); //i think here too.
        }
        public bool NeedsToDraw { get; set; } //this is used so it knows to draw to begin with.
        public EnumTrickStatus TrickStatus { get; set; }
    }
}