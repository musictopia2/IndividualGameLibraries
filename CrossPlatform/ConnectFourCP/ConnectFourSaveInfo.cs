using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace ConnectFourCP
{
    [SingletonGame]
    public class ConnectFourSaveInfo : BasicSavedPlainBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ConnectFourPlayerItem>
    { //anything needed for autoresume is here.
        public ConnectFourCollection GameBoard { get; set; } = new ConnectFourCollection();
    }
}