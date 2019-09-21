using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace LifeCardGameCP
{
    [SingletonGame]
    public class LifeCardGameSaveInfo : BasicSavedCardClass<LifeCardGamePlayerItem, LifeCardGameCardInformation>
    {
        public CustomBasicList<int> YearList { get; set; } = new CustomBasicList<int>();
        public int YearsPassed() => YearList.Count * 10;
    }
}