using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace MancalaCP
{
    [SingletonGame]
    public class MancalaSaveInfo : BasicSavedGameClass<MancalaPlayerItem>
    { //anything needed for autoresume is here.
        public bool IsStart { get; set; }
    }
}