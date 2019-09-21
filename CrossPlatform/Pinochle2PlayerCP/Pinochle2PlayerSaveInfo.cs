using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Pinochle2PlayerCP
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