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
using BaseSolitaireClassesCP.PileViewModels;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using BaseSolitaireClassesCP.MiscClasses;
//i think this is the most common things i like to do
namespace AgnesSolitaireCP
{
    public class WastePiles : WastePilesCP
    {
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
        public WastePiles(IBasicGameVM thisMod) : base(thisMod)
        {
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
