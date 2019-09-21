using BasicGameFramework.BasicDrawables.Dictionary;
namespace TileRummyCP
{
    public class SavedSet
    {
        public DeckRegularDict<TileInfo> TileList { get; set; } = new DeckRegularDict<TileInfo>();
        public EnumWhatSets SetType { get; set; }
        public bool IsNew { get; set; }
    }
}