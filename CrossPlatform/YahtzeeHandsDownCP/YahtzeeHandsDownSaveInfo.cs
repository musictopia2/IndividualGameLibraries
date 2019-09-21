using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace YahtzeeHandsDownCP
{
    [SingletonGame]
    public class YahtzeeHandsDownSaveInfo : BasicSavedCardClass<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownCardInformation>
    {
        public int ExtraTurns { get; set; }
        public CustomBasicList<int> Combos { get; set; } = new CustomBasicList<int>();
        public CustomBasicList<int> ChanceList { get; set; } = new CustomBasicList<int>();
        public int FirstPlayerWentOut { get; set; }
    }
}
