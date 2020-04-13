using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using Pinochle2PlayerCP.Cards;
namespace Pinochle2PlayerCP.Data
{
    [SingletonGame]
    public class Pinochle2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<Pinochle2PlayerCardInformation> CardList { get; set; } = new DeckRegularDict<Pinochle2PlayerCardInformation>();
        public int GameOverAt { get; set; } = 1000;
        public bool ChooseToMeld { get; set; }
        public CustomBasicList<MeldClass> MeldList { get; set; } = new CustomBasicList<MeldClass>();
        public string StartMessage { get; set; } = "";
    }
}