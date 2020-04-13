using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CheckersCP.Logic;
//i think this is the most common things i like to do
namespace CheckersCP.Data
{
    [SingletonGame]
    public class CheckersSaveInfo : BasicSavedGameClass<CheckersPlayerItem>
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

        public bool ForcedToMove { get; set; }
    }
}