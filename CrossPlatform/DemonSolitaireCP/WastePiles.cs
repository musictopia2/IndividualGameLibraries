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
namespace DemonSolitaireCP
{
    public class WastePiles : WastePilesCP
    {
        private readonly DemonSolitaireViewModel _thisMod;
        public WastePiles(IBasicGameVM thisMod) : base(thisMod)
        {
            _thisMod = (DemonSolitaireViewModel)thisMod;
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            Piles.PileList.ForEach(thisPile => thisPile.CardList.Add(thisCol[Piles.PileList.IndexOf(thisPile)]));
        }
        protected override void AfterRemovingFromLastPile(int Lasts)
        {
            if (Piles.HasCardInColumn(Lasts) || _thisMod.Heel1!.IsEndOfDeck())
                return;
            Piles.AddCardToColumn(Lasts, _thisMod.Heel1.DrawCard());
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false)
            {
                if (_thisMod.Heel1!.IsEndOfDeck() == false)
                    throw new BasicBlankException("If its not at the end of the deck; a card needs to be placed");
                return true;
            }
            var prevCard = Piles.GetLastCard(whichOne);
            return prevCard.Value - 1 == thisCard.Value && prevCard.Suit != thisCard.Suit;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            if (PreviousSelected == -1)
                throw new BasicBlankException("Cannot find out whether we can move the cards because none was selected");
            var firstList = Piles.ListGivenCards(PreviousSelected);
            TempList = firstList.ListValidCardsAlternateColors(); //has to use templist
            var thisPile = Piles.PileList[whichOne];
            lastOne = TempList.Count - 1;
            if (thisPile.CardList.Count == 0)
            {
                return true;
            }
            var prevCard = Piles.GetLastCard(whichOne);
            var thisCard = Piles.PileList[PreviousSelected].CardList.First();

            return prevCard.Value - 1 == thisCard.Value && prevCard.Suit != thisCard.Suit;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
    }
}
