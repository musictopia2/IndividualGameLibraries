using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace DiceDominosCP.Data
{
    [SingletonGame]
    public class DiceDominosSaveInfo : BasicSavedDiceClass<SimpleDice, DiceDominosPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<SimpleDominoInfo>? BoardDice { get; set; }
        public bool HasRolled { get; set; }
        public bool DidHold { get; set; }
    }
}