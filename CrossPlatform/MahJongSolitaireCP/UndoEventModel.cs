using BaseMahjongTilesCP;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MahJongSolitaireCP
{
    /// <summary>
    /// This will make the gameboard not visible.
    /// </summary>
    public class StartNewGameEventModel { }

    public class TileChosenEventModel
    {
        public int Deck { get; set; } //if deck is 0, then its invisible.
        //otherwise, will be whatever it is.

    }

    public class UndoEventModel
    {
        public CustomBasicList<MahjongSolitaireTileInfo>? PreviousList { get; set; }
    }
}
