using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace DeadDie96CP.Data
{
    [SingletonGame]
    public class DeadDie96SaveInfo : BasicSavedDiceClass<TenSidedDice, DeadDie96PlayerItem>
    { //anything needed for autoresume is here.

    }
}