using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Attributes;
using RookCP.Cards;
namespace RookCP.Data
{
    [SingletonGame]
    public class RookSaveInfo : BasicSavedTrickGamesClass<EnumColorTypes, RookCardInformation, RookPlayerItem>
    { //anything needed for autoresume is here.
        public int WonSoFar { get; set; }
        public bool DummyPlay { get; set; }
        public int HighestBidder { get; set; }
        public DeckRegularDict<RookCardInformation> NestList { get; set; } = new DeckRegularDict<RookCardInformation>();
        public DeckObservableDict<RookCardInformation> DummyList { get; set; } = new DeckObservableDict<RookCardInformation>();
        public DeckRegularDict<RookCardInformation> CardList { get; set; } = new DeckRegularDict<RookCardInformation>();
        private EnumStatusList _gameStatus;

        public EnumStatusList GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (SetProperty(ref _gameStatus, value))
                {
                    //can decide what to do when property changes
                    if (_model == null)
                        return;
                    _model.GameStatus = value;
                }
            }
        }
        private RookVMData? _model;

        public void LoadMod(RookVMData model)
        {
            _model = model;
            _model.GameStatus = GameStatus;
        }
    }
}