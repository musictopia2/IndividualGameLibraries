using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using Phase10CP.Cards;
using Phase10CP.SetClasses;
namespace Phase10CP.Data
{
    [SingletonGame]
    public class Phase10SaveInfo : BasicSavedCardClass<Phase10PlayerItem, Phase10CardInformation>
    { //anything needed for autoresume is here.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>(); //decided to not be string so its easier to change for testing purposes.
        public bool CompletedPhase { get; set; }
        public bool Skips { get; set; }
        public bool IsTie { get; set; }
    }
}