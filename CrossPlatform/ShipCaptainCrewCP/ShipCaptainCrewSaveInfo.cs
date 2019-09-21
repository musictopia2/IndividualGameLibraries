using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace ShipCaptainCrewCP
{
    [SingletonGame]
    public class ShipCaptainCrewSaveInfo : BasicSavedDiceClass<SimpleDice, ShipCaptainCrewPlayerItem> { }
}