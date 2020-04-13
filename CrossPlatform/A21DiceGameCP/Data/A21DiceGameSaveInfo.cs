using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace A21DiceGameCP.Data
{
    [SingletonGame]
    public class A21DiceGameSaveInfo : BasicSavedDiceClass<SimpleDice, A21DiceGamePlayerItem>
    { //anything needed for autoresume is here.
        public bool IsFaceOff { get; set; }
    }
}