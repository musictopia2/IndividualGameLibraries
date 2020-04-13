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
//i think this is the most common things i like to do
namespace BeleaguredCastleCP.Logic
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


            for (x = 1; x <= 6; x++)
            {
                foreach (var thisPile in Piles.PileList)
                {
                    thisPile.TempList.Add(thisCol[y]);
                    y += 1;
                }
            }
            foreach (var thisPile in Piles.PileList)
            {
                thisPile.CardList.ReplaceRange(thisPile.TempList);
                if (thisPile.CardList.Count > 6)
                    throw new BasicBlankException("The card list cannot be more than 6 to start with.");
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
            if (PreviousSelected == -1)
                throw new BasicBlankException("There was no card selected");
            if (Piles.HasCardInColumn(whichOne) == false)
                return true;
            var oldCard = Piles.GetLastCard(whichOne);
            var newCard = Piles.GetLastCard(PreviousSelected);
            return newCard.Value + 1 == oldCard.Value;
        }
    }
}
