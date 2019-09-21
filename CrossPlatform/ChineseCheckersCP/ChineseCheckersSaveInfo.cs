using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChineseCheckersCP
{
    [SingletonGame]
    public class ChineseCheckersSaveInfo : BasicSavedPlainBoardGameClass<EnumColorList, MarblePiecesCP<EnumColorList>, ChineseCheckersPlayerItem>
    {
        public CustomBasicList<MoveInfo> PreviousMoves { get; set; } = new CustomBasicList<MoveInfo>();
        public bool TurnContinued { get; set; }
        public int PreviousSpace { get; set; }
    }
}