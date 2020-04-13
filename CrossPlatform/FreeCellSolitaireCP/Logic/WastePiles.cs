using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;

namespace FreeCellSolitaireCP.Logic
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
        }
        public int FreePiles => Piles.PileList.Count(items => items.CardList.Count == 0);
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int y = 0;
            8.Times(x =>
            {
                foreach (var thisPile in Piles.PileList)
                {
                    if (y == thisCol.Count)
                        break;
                    thisPile.CardList.Add(thisCol[y]);
                    y++;
                }
            });
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false)
                return true;
            var oldCard = Piles.GetLastCard(whichOne);
            return oldCard.Value - 1 == thisCard.Value && oldCard.Color != thisCard.Color;
        }

        public bool CanMoveCards(int whichOne, out int lastOne, int room)
        {
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            lastOne = -1; //until i figure out what else to do.
            var givList = Piles.ListGivenCards(PreviousSelected);

            TempList = givList.ListValidCardsAlternateColors();
            var thisPile = Piles.PileList[whichOne];
            SolitaireCard oldCard;
            if (thisPile.CardList.Count == 0)
            {
                if (room > TempList.Count)
                    lastOne = TempList.Count - 1;
                else
                    lastOne = room - 1;
                return true;
            }
            oldCard = Piles.GetLastCard(whichOne);
            if (oldCard.Value == EnumCardValueList.LowAce)
                return false;

            //can't quite do the extension because of the room part.

            for (int x = TempList.Count; x >= 1; x += -1)
            {
                var newCard = TempList[x - 1];
                if (newCard.Value + 1 == oldCard.Value && newCard.Color != oldCard.Color && x <= room)
                {
                    lastOne = x - 1;
                    return true;
                }
            }
            return false;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1;
            return false;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
    }
}
