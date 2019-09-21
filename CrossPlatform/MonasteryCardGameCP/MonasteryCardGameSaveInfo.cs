using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MonasteryCardGameCP
{
    [SingletonGame]
    public class MonasteryCardGameSaveInfo : BasicSavedCardClass<MonasteryCardGamePlayerItem, MonasteryCardInfo>
    { //anything needed for autoresume is here.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public int Mission { get; set; }
    }
}