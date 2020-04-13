using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using Spades2PlayerCP.Cards;
namespace Spades2PlayerCP.Data
{
    [SingletonGame]
    public class Spades2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem>,
        ITrickStatusSavedClass
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
                    if (_model != null)
                        _model.GameStatus = value;
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
                    if (_model != null)
                        _model.RoundNumber = value;
                }

            }
        }
        private Spades2PlayerVMData? _model;
        public void LoadMod(Spades2PlayerVMData model)
        {
            _model = model;
            _model.RoundNumber = RoundNumber;
            _model.GameStatus = GameStatus; //i think here too.
        }
        public bool NeedsToDraw { get; set; } //this is used so it knows to draw to begin with.
        public EnumTrickStatus TrickStatus { get; set; }
    }
}