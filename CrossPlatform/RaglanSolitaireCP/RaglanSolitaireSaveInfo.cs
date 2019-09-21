using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
namespace RaglanSolitaireCP
{
    [SingletonGame]
    public class RaglanSolitaireSaveInfo : SolitaireSavedClass
    {
        public DeckRegularDict<SolitaireCard> StockCards { get; set; } = new DeckRegularDict<SolitaireCard>();
    }
}