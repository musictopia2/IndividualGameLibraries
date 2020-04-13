using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
namespace AgnesSolitaireCP.Logic
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
        }
        public void AddCards(DeckRegularDict<SolitaireCard> thisList)
        {
            if (thisList.Count != Piles.PileList.Count)
                throw new BasicBlankException($"Needs {Piles.PileList.Count}, not {thisList.Count}");
            int x = 0;
            Piles.PileList.ForEach(thisPile =>
            {
                var thisCard = thisList[x];
                thisPile.CardList.Add(thisCard);
                x++; //because 0 based.
            });
        }

        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int z = 8;
            int y = 0;
            Piles.PileList.ForEach(thisColumn =>
            {
                z--;
                z.Times(x =>
                {
                    var thisCard = thisCol[y];
                    thisColumn.CardList.Add(thisCard);
                    y++;
                });
            });
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false)
                return true;
            var prevCard = Piles.GetLastCard(PreviousSelected);
            return prevCard.Value - 1 == thisCard.Value && prevCard.Color == thisCard.Color;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            var firstList = Piles.ListGivenCards(PreviousSelected);
            TempList = firstList.ListValidCardsSameSuit(); //has to use templist
            var thisPile = Piles.PileList[whichOne];
            if (thisPile.CardList.Count == 0)
            {
                lastOne = TempList.Count - 1;
                return true;
            }
            var oldCard = Piles.GetLastCard(whichOne);
            return TempList.CanMoveCardsSameColor(oldCard, ref lastOne);
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
    }
}
