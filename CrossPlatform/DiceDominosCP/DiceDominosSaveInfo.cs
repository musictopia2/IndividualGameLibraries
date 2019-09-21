using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Dice;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace DiceDominosCP
{
    [SingletonGame]
    public class DiceDominosSaveInfo : BasicSavedDiceClass<SimpleDice, DiceDominosPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<SimpleDominoInfo>? BoardDice { get; set; }
        public bool HasRolled { get; set; }
        public bool DidHold { get; set; }
    }
}