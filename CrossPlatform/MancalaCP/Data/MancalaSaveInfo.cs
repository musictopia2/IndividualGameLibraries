using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace MancalaCP.Data
{
    [SingletonGame]
    public class MancalaSaveInfo : BasicSavedGameClass<MancalaPlayerItem>
    { //anything needed for autoresume is here.
        public bool IsStart { get; set; }
    }
}