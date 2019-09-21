using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace SequenceDiceCP
{
    [SingletonGame]
    public class SequenceDiceSaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, SequenceDicePlayerItem>
    { //anything needed for autoresume is here.
        public EnumGameStatusList GameStatus { get; set; }//don't need previousspace because that is given from the spaceinfocp
        public SequenceBoardCollection GameBoard { get; set; } = new SequenceBoardCollection();
    }
}