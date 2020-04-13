using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace ShipCaptainCrewCP.Data
{
    [SingletonGame]
    public class ShipCaptainCrewSaveInfo : BasicSavedDiceClass<SimpleDice, ShipCaptainCrewPlayerItem>
    { //anything needed for autoresume is here.

    }
}