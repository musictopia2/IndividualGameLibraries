using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace BattleshipCP
{
    [SingletonGame]
    public class BattleshipSaveInfo : BasicSavedGameClass<BattleshipPlayerItem> { }
}