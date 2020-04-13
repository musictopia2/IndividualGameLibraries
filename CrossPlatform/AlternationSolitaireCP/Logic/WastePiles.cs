using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
namespace AlternationSolitaireCP.Logic
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
            int y = 0;
            7.Times(x =>
            {
                Piles.PileList.ForEach(thisPile =>
                {
                    var thisCard = thisCol[y];
                    thisCard.IsUnknown = x == 2 || x == 4 || x == 6;
                    thisPile.CardList.Add(thisCard);
                    y++;
                });
            });
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false)
                return true;
            var prevCard = Piles.GetLastCard(whichOne);
            return prevCard.Value - 1 == thisCard.Value && prevCard.Color != thisCard.Color;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            var firstList = Piles.ListGivenCards(PreviousSelected);
            TempList = firstList.ListValidCardsAlternateColors(); //has to use templist
            var thisPile = Piles.PileList[whichOne];
            if (thisPile.CardList.Count == 0)
            {
                lastOne = TempList.Count - 1;
                return true;
            }
            var oldCard = Piles.GetLastCard(whichOne);
            if (oldCard.Value == EnumCardValueList.LowAce)
                return false;

            return TempList.CanMoveCardsAlternateColors(oldCard, ref lastOne);

        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
    }
}
