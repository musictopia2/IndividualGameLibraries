using BaseMahjongTilesCP;
using CommonBasicStandardLibraries.CollectionClasses;

namespace MahJongSolitaireCP.EventModels
{
    public class UndoEventModel
    {
        public CustomBasicList<MahjongSolitaireTileInfo>? PreviousList { get; set; }
    }
}