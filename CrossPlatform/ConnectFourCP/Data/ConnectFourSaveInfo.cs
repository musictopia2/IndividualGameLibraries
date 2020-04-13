using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace ConnectFourCP.Data
{
    [SingletonGame]
    public class ConnectFourSaveInfo : BasicSavedGameClass<ConnectFourPlayerItem>
    { //anything needed for autoresume is here.
        public ConnectFourCollection GameBoard { get; set; } = new ConnectFourCollection();
    }
}