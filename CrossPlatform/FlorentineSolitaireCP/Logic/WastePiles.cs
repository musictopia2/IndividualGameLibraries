using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
namespace FlorentineSolitaireCP.Logic
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            int x = 0;
            Discards!.PileList!.ForEach(thisPile =>
            {
                thisPile.ObjectList.Add(thisCol[x]);
                x++;
            });
        }
        protected override void AfterFirstLoad()
        {
            CustomBasicList<int> thisList = new CustomBasicList<int>() { 0, 2, 6, 8 };
            Discards!.RemoveSpecificDiscardPiles(thisList);
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Discards!.HasCard(whichOne) == false)
                return true;
            if (whichOne == 2)
                return false;
            var oldCard = Discards.GetLastCard(whichOne);

            if (oldCard.Value - 1 == thisCard.Value)
                return true;
            return oldCard.Value == EnumCardValueList.LowAce && thisCard.Value == EnumCardValueList.King; //because round corner.
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            return false;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            if (whichOne == 2)
                return false;
            if (Discards!.HasCard(whichOne) == false)
            {
                return PreviousSelected == 2;
            }
            if (PreviousSelected == 2)
                return false;
            var oldCard = Discards.GetLastCard(whichOne);
            var newCard = Discards.GetLastCard(PreviousSelected);
            if (oldCard.Value - 1 == newCard.Value)
                return true;
            return oldCard.Value == EnumCardValueList.LowAce && newCard.Value == EnumCardValueList.King; //because round corner.
        }
    }
}
