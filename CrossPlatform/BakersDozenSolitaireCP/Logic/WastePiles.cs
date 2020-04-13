using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
namespace BakersDozenSolitaireCP.Logic
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
            int x;
            int y = 0;
            for (x = 1; x <= 4; x++)
            {
                foreach (var thisPile in Piles.PileList)
                {
                    if ((x != 1) & (thisCol[y].Value == EnumCardValueList.King))
                        throw new BasicBlankException("A king must be put into the first row");
                    thisPile.CardList.Add(thisCol[y]);
                    y += 1;
                }
            }
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

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            SolitaireCard newCard;
            SolitaireCard oldCard;
            if (Piles.HasCardInColumn(whichOne) == false)
                return false;// cannot move because the empty piles are never filled
            oldCard = Piles.GetLastCard(whichOne);
            newCard = GetCard(); // i think
            return oldCard.Value - 1 == newCard.Value;
        }
    }
}
