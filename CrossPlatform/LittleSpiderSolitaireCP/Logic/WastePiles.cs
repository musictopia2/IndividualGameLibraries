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
//i think this is the most common things i like to do
namespace LittleSpiderSolitaireCP.Logic
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator)
        {
        }
        public void AddCards(DeckRegularDict<SolitaireCard> thisList)
        {
            if (thisList.Count != 8)
                throw new BasicBlankException($"Needs 8 cards, not {thisList.Count}");
            int x = 0;
            Discards!.PileList!.ForEach(thisPile =>
            {
                thisPile.ObjectList.Add(thisList[x]); //has to be 0 based.
                x++;
            });
        }
        protected override void AfterFirstLoad()
        {
            var thisList = Enumerable.Range(4, 4).ToCustomBasicList();
            Discards!.RemoveSpecificDiscardPiles(thisList);
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int x = 0;
            Discards!.PileList!.ForEach(thisPile =>
            {
                thisPile.ObjectList.Add(thisCol[x]); //has to be 0 based.
                x++;
            });
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
            if (Discards!.HasCard(whichOne) == false)
                return false; //because not filled.
            var oldCard = Discards.GetLastCard(whichOne);
            var newCard = Discards.GetLastCard(PreviousSelected);
            if (oldCard.Value - 1 == newCard.Value)
                return true;
            if (oldCard.Value + 1 == newCard.Value)
                return true;
            if (oldCard.Value == EnumCardValueList.King && newCard.Value == EnumCardValueList.LowAce)
                return true;
            return oldCard.Value == EnumCardValueList.LowAce && newCard.Value == EnumCardValueList.King;
        }
    }
}
