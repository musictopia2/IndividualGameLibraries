using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
namespace BisleySolitaireCP.Logic
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int x = 0;
            Discards!.PileList!.ForEach(thisPile =>
            {
                thisPile.ObjectList.Add(thisCol[x]);
                x++;
            });
            CardList.ReplaceRange(thisCol); // i think.
        }
        protected override void AfterFirstLoad()
        {
            Discards!.RemoveFirstDiscardPiles(4);
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            return false;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            return false;
        }
        public override bool CanSelectUnselectPile(int whichOne)
        {
            if (whichOne >= 35)
                return true;
            var newPile = whichOne + 13;
            return Discards!.HasCard(newPile) == false;
        }
        public override bool CanMoveToAnotherPile(int whichOne)
        {
            if (PreviousSelected == -1)
                throw new BasicBlankException("There is no pile even selected");
            var oldCard = Discards!.GetLastCard(whichOne);
            var newCard = Discards!.GetLastCard(PreviousSelected);
            if (newCard.Suit != oldCard.Suit)
                return false; //can't fill it.
            if (oldCard.Value - 1 == newCard.Value)
                return true;
            return oldCard.Value == newCard.Value - 1;
        }
    }
}
