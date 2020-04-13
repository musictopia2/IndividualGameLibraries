using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
namespace CandylandCP.Data
{
    [SingletonGame]
    public class CandylandSaveInfo : BasicSavedGameClass<CandylandPlayerItem>, ISavedCardList<CandylandCardData>
    { //anything needed for autoresume is here.
        public bool DidDraw { get; set; } //i think needed
        public DeckRegularDict<CandylandCardData>? CardList { get; set; }
        public CandylandCardData? CurrentCard { get; set; }
    }
}