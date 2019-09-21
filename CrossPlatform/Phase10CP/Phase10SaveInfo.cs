using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Phase10CP
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