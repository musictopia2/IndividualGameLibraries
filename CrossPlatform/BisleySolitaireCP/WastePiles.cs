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
namespace BisleySolitaireCP
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(IBasicGameVM thisMod) : base(thisMod)
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
            CardList.ReplaceRange(thisCol); // i think.
        }
        protected override void AfterFirstLoad()
        {
            Discards!.RemoveFirstDiscardPiles(4);
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
        public override bool CanSelectUnselectPile(int whichOne)
        {
            if (whichOne >= 35)
                return true;
            var newPile = whichOne + 13;
            return Discards!.HasCard(newPile) == false;
        }
        public override bool CanMoveToAnotherPile(int whichOne)
        {
            if (PreviousSelected == -1)
                throw new BasicBlankException("There is no pile even selected");
            var oldCard = Discards!.GetLastCard(whichOne);
            var newCard = Discards!.GetLastCard(PreviousSelected);
            if (newCard.Suit != oldCard.Suit)
                return false; //can't fill it.
            if (oldCard.Value - 1 == newCard.Value)
                return true;
            return oldCard.Value == newCard.Value - 1;
        }
    }
}
