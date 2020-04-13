using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
namespace EightOffSolitaireCP.Data
{
    [SingletonGame]
    public class EightOffSolitaireSaveInfo : SolitaireSavedClass, IMappable
    {
        public DeckRegularDict<SolitaireCard> ReserveList { get; set; } = new DeckRegularDict<SolitaireCard>();
    }
}