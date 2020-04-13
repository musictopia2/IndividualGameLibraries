using CommonBasicStandardLibraries.CollectionClasses;
namespace ThreeLetterFunCP.Data
{
    public class TempWord
    {
        public int CardUsed { get; set; }
        public CustomBasicList<TilePosition> TileList { get; set; } = new CustomBasicList<TilePosition>();
        public int Player { get; set; }
        public int TimeToGetWord { get; set; }
    }
}