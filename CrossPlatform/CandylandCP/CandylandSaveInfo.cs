using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace CandylandCP
{
    [SingletonGame]
    public class CandylandSaveInfo : BasicSavedGameClass<CandylandPlayerItem>, ISavedCardList<CandylandCardData>
    {
        public bool DidDraw { get; set; } //i think needed
        public DeckRegularDict<CandylandCardData>? CardList { get; set; }
        public CandylandCardData? CurrentCard { get; set; }
    }
}