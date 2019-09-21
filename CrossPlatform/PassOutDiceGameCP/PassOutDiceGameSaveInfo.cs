using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace PassOutDiceGameCP
{
    [SingletonGame]
    public class PassOutDiceGameSaveInfo : BasicSavedBoardDiceGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, PassOutDiceGamePlayerItem>
    { //anything needed for autoresume is here.
        public int PreviousSpace { get; set; }
        public CustomBasicList<int> SpacePlayers { get; set; } = new CustomBasicList<int>();
        public bool DidRoll { get; set; }
    }
}