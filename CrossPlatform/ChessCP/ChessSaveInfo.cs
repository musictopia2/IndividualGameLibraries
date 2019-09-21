using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace ChessCP
{
    [SingletonGame]
    public class ChessSaveInfo : BasicSavedPlainBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem>
    { //anything needed for autoresume is here.
        public int SpaceHighlighted
        {
            get
            {
                return GameBoardGraphicsCP.SpaceSelected;
            }
            set
            {
                GameBoardGraphicsCP.SpaceSelected = value;
            }
        }
        public EnumGameStatus GameStatus { get; set; }
        public PreviousMove? PossibleMove { get; set; }
        public PreviousMove PreviousMove { get; set; } = new PreviousMove();
    }
}