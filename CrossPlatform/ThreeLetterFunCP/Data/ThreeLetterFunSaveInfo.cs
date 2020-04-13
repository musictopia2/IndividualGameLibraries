using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ThreeLetterFunCP.Data
{
    [SingletonGame]
    public class ThreeLetterFunSaveInfo : BasicSavedGameClass<ThreeLetterFunPlayerItem>
    { //anything needed for autoresume is here.
        public EnumLevel Level { get; set; }
        public CustomBasicList<TileInformation> TileList { get; set; } = new CustomBasicList<TileInformation>();
        public DeckRegularDict<ThreeLetterFunCardData> SavedList { get; set; } = new DeckRegularDict<ThreeLetterFunCardData>();
        public bool ShortGame { get; set; }
        public int UpTo { get; set; }
        public int CardsToBeginWith { get; set; }
        public bool CanStart { get; set; } //defaults to false.
    }
}