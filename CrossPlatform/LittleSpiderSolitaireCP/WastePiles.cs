using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace LittleSpiderSolitaireCP
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(IBasicGameVM thisMod) : base(thisMod)
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
