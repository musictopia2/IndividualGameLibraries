using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace RackoCP
{
    [SingletonGame]
    public class RackoSaveInfo : BasicSavedCardClass<RackoPlayerItem, RackoCardInformation> { }
}