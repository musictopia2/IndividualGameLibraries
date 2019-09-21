using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace TicTacToeCP
{
    [SingletonGame]
    public class TicTacToeSaveInfo : BasicSavedGameClass<TicTacToePlayerItem>
    { //anything needed for autoresume is here.
        public TicTacToeCollection GameBoard { get; set; } = new TicTacToeCollection(); //i think
    }
}