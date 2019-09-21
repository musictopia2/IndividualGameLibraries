using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.ViewModelInterfaces;
namespace EightOffSolitaireCP
{
    public class ReservePiles : HandViewModel<SolitaireCard>
    {
        public int HowManyCards => HandList.Count;
        public void AddCard(SolitaireCard thisCard) => HandList.Add(thisCard);
        public void ClearBoard() => HandList.Clear();
        public void RemoveCard(SolitaireCard thisCard) => HandList.RemoveObjectByDeck(thisCard.Deck);
        public SolitaireCard GetCardSelected()
        {
            int nums = ObjectSelected();
            if (nums == 0)
                return new SolitaireCard(); //hopefully still okay.
            return HandList.GetSpecificItem(nums);
        }
        public ReservePiles(IBasicGameVM ThisMod) : base(ThisMod) { }
    }
}