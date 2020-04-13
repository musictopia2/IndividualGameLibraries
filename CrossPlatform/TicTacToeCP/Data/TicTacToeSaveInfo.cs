using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace TicTacToeCP.Data
{
    [SingletonGame]
    public class TicTacToeSaveInfo : BasicSavedGameClass<TicTacToePlayerItem>
    { //anything needed for autoresume is here.
        public TicTacToeCollection GameBoard { get; set; } = new TicTacToeCollection(); //i think
    }
}