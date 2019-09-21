using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace A21DiceGameCP
{
    [SingletonGame]
    public class A21DiceGameSaveInfo : BasicSavedDiceClass<SimpleDice, A21DiceGamePlayerItem>
    { //anything needed for autoresume is here.
        public bool IsFaceOff { get; set; }
    }
}