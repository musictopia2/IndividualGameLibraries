using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace DeadDie96CP
{
    [SingletonGame]
    public class DeadDie96SaveInfo : BasicSavedDiceClass<TenSidedDice, DeadDie96PlayerItem> { }
}