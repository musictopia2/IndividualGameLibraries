using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using YahtzeeHandsDownCP.Cards;
namespace YahtzeeHandsDownCP.Data
{
    [SingletonGame]
    public class YahtzeeHandsDownSaveInfo : BasicSavedCardClass<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownCardInformation>
    { //anything needed for autoresume is here.
        public int ExtraTurns { get; set; }
        public CustomBasicList<int> Combos { get; set; } = new CustomBasicList<int>();
        public CustomBasicList<int> ChanceList { get; set; } = new CustomBasicList<int>();
        public int FirstPlayerWentOut { get; set; }
    }
}