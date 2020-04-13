using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace BattleshipCP.Data
{
    [SingletonGame]
    public class BattleshipSaveInfo : BasicSavedGameClass<BattleshipPlayerItem>
    { //anything needed for autoresume is here.

    }
}