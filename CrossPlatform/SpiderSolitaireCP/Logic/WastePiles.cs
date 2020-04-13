using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGameFrameworkLibrary.CommandClasses;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
//i think this is the most common things i like to do
namespace SpiderSolitaireCP.Logic
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
            4.Times(x =>
            {
                Piles.PileList.ForEach(thisPile =>
                {
                    var thisCard = thisCol[y];
                    thisCard.IsUnknown = x != 4;
                    thisPile.CardList.Add(thisCard);
                    y++;
                });
            });
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            return false;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            TempList = ListValidCards();
            var thisPile = Piles.PileList[whichOne];
            if (thisPile.CardList.Count == 0)
            {
                lastOne = TempList.Count - 1;
                return true;
            }
            var oldCard = Piles.GetLastCard(whichOne);
            if (oldCard.Value == EnumCardValueList.LowAce)
                return false;
            return TempList.CanMoveCardsRegardlessOfColorOrSuit(oldCard, ref lastOne);
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
        private DeckRegularDict<SolitaireCard> ListValidCards(int pile)
        {
            int beforeSelected = PreviousSelected;
            PreviousSelected = pile;
            var output = ListValidCards();
            PreviousSelected = beforeSelected;
            return output;
        }
        private DeckRegularDict<SolitaireCard> ListValidCards()
        {
            var output = Piles.ListGivenCards(PreviousSelected);
            if (output.Count == 0)
                return new DeckRegularDict<SolitaireCard>(); //decided this instead of error.
            return output.ListValidCardsSameSuit();
        }
        public DeckRegularDict<SolitaireCard> MoveList()
        {
            if (TempList.Count == 0)
                throw new BasicBlankException("There are no cards to move");
            if (TempList.Count != 13)
                throw new BasicBlankException("Must move 13 cards");
            return TempList;
        }
        public void RemoveCards(int whichOne)
        {
            if (TempList.Count != 13)
                throw new BasicBlankException("Must have 13 cards to remove the 13 cards");
            TempList.ForEach(tempCard => Piles.RemoveSpecificCardFromColumn(whichOne, tempCard.Deck));
            if (Piles.HasCardInColumn(whichOne))
            {
                var thisCard = Piles.GetLastCard(whichOne);
                Piles.RemoveFromUnknown(thisCard);
            }
            UnselectAllColumns();
        }
        public void AddCards(DeckRegularDict<SolitaireCard> thisList)
        {
            int x = 0;
            foreach (var thisPile in Piles.PileList)
            {
                if (x == thisList.Count)
                    break;
                var thisCard = thisList[x];
                Piles.RemoveFromUnknown(thisCard);
                thisPile.CardList.Add(thisCard);
                x++;
            }
        }
        public bool CanMoveAll(int whichOne)
        {
            var thisCol = ListValidCards(whichOne);
            EnumSuitList previousSuit = EnumSuitList.None;
            int x = 0;
            if (thisCol.Count != 13)
                return false;
            foreach (var thisCard in thisCol)
            {
                if (x == 0)
                    previousSuit = thisCard.Suit;
                else if (thisCard.Suit != previousSuit)
                    return false;
                x++;
            }
            TempList = thisCol;
            return true;
        }
    }
}
