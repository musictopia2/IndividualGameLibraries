using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace Rummy500CP
{
    public class DiscardListCP : HandViewModel<RegularRummyCard>
    {
        public DiscardListCP(IBasicGameVM thisMod) : base(thisMod)
        {
            AutoSelect = EnumAutoType.None;
            Text = "Discard";
        }
        public void AddToDiscard(RegularRummyCard thisCard)
        {
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            HandList.Add(thisCard);
        }
        public void ClearDiscardList()
        {
            HandList.Clear();
        }
        public int LastCardDiscarded => HandList.Last().Deck;
        public DeckRegularDict<RegularRummyCard> DiscardListSelected(int deck)
        {
            if (deck == 0)
                throw new BasicBlankException("Deck cannot be 0 for the discard selected.  Rethink");
            DeckRegularDict<RegularRummyCard> output = new DeckRegularDict<RegularRummyCard>();
            bool didStart = false;
            HandList.ForEach(thisCard =>
            {
                if (thisCard.Deck == deck)
                    didStart = true;
                if (didStart == true)
                    output.Add(thisCard);
            });
            if (output.Count == 0)
                throw new BasicBlankException("No cards for discard list.  Rethink");
            return output;
        }
        public void RemoveFromPoint(int deck)
        {
            DeckRegularDict<RegularRummyCard> output = new DeckRegularDict<RegularRummyCard>();
            foreach (var thisCard in HandList)
            {
                if (thisCard.Deck == deck)
                    break;
                output.Add(thisCard);
            }
            if (output.Count == HandList.Count)
                throw new BasicBlankException("No starting point for removing cards from point");
            HandList.ReplaceRange(output);
        }
    }
}