using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.TriangleClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.DrawableListsViewModels;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace TriangleSolitaireCP
{
    [SingletonGame]
    public class TriangleSolitaireSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        public SavedDiscardPile<SolitaireCard>? PileData { get; set; }
        public SavedTriangle? TriangleData { get; set; }
        public EnumIncreaseList Incs { get; set; }
    }
}
