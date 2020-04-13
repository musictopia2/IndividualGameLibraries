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
namespace EasyGoSolitaireCP.Logic
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
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Discards!.HasCard(whichOne) == false)
                return true;
            var oldCard = Discards.GetLastCard(whichOne);
            if (thisCard.Suit != oldCard.Suit)
                return false;
            return oldCard.Value - 1 == thisCard.Value;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            return false;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            if (PreviousSelected == -1)
                throw new BasicBlankException("There is no pile even selected");
            if (Discards!.HasCard(whichOne) == false)
                return false; //can only fill an empty spot with a card from the deck
            var oldCard = Discards.GetLastCard(whichOne);
            var newCard = Discards.GetLastCard(PreviousSelected);
            if (newCard.Suit != oldCard.Suit)
                return false;
            return oldCard.Value - 1 == newCard.Value;
        }
    }
}
