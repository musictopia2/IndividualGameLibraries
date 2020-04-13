using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace ConnectTheDotsCP.Data
{
    [SingletonGame]
    public class ConnectTheDotsSaveInfo : BasicSavedGameClass<ConnectTheDotsPlayerItem>
    { //anything needed for autoresume is here.
        public SavedBoardData? BoardData { get; set; }
    }
}