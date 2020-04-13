using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace TileRummyCP.Data
{
    [SingletonGame]
    public class TileRummySaveInfo : BasicSavedGameClass<TileRummyPlayerItem>
    { //anything needed for autoresume is here.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public CustomBasicList<SavedSet> BeginningList { get; set; } = new CustomBasicList<SavedSet>(); //this is at the beginning.
        public SavedScatteringPieces<TileInfo>? PoolData { get; set; }
        public int FirstPlayedLast { get; set; }
        public CustomBasicList<int> TilesFromField { get; set; } = new CustomBasicList<int>();
        public CustomBasicList<int> YourTiles { get; set; } = new CustomBasicList<int>();
        public bool DidExpand { get; set; }
    }
}