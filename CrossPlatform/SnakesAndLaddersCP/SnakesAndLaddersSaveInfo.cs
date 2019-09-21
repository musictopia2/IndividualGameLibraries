using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace SnakesAndLaddersCP
{
    [SingletonGame]
    public class SnakesAndLaddersSaveInfo : BasicSavedGameClass<SnakesAndLaddersPlayerItem>
    {
        public DiceList<SimpleDice> DiceList = new DiceList<SimpleDice>();
        public bool HasRolled { get; set; }
    }
}