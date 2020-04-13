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
using BasicGameFrameworkLibrary.SolitaireClasses.MiscClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
//i think this is the most common things i like to do
namespace MarthaSolitaireCP.Logic
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
                    thisCard.IsUnknown = x == 1 || x == 3;
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
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            lastOne = -1; //until i figure out what else to do.
            var givList = Piles.ListGivenCards(PreviousSelected);

            TempList = givList.ListValidCardsAlternateColors();
            var thisPile = Piles.PileList[whichOne];
            SolitaireCard oldCard;
            if (thisPile.CardList.Count == 0)
            {
                lastOne = 0; //only last one.
                return true;
            }
            oldCard = Piles.GetLastCard(whichOne);
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
