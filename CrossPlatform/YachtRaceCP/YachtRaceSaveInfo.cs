using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace YachtRaceCP
{
    [SingletonGame]
    public class YachtRaceSaveInfo : BasicSavedDiceClass<SimpleDice, YachtRacePlayerItem> { }
}