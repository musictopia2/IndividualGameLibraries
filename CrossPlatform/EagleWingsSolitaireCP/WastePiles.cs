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
using BasicGameFramework.DrawableListsViewModels;
//i think this is the most common things i like to do
namespace EagleWingsSolitaireCP
{
    public class WastePiles : WastePilesCP
    {
        private readonly EagleWingsSolitaireViewModel _thisMod;
        public WastePiles(IBasicGameVM thisMod) : base(thisMod)
        {
            _thisMod = (EagleWingsSolitaireViewModel)thisMod;
        }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            Piles.PileList.ForEach(thisPile => thisPile.CardList.Add(thisCol[Piles.PileList.IndexOf(thisPile)]));
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false && _thisMod.Heel1!.IsEndOfDeck())
                return true;
            return Piles.HasCardInColumn(whichOne) == false && _thisMod.Heel1!.CardsLeft() == 1;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            return false;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }
        protected override void AfterRemovingFromLastPile(int Lasts)
        {
            if (Piles.HasCardInColumn(Lasts) || _thisMod.Heel1!.IsEndOfDeck() || _thisMod.Heel1.CardsLeft() == 1)
                return;
            Piles.AddCardToColumn(Lasts, _thisMod.Heel1.DrawCard());
            if (_thisMod.Heel1.CardsLeft() == 1)
                _thisMod.Heel1.DeckStyle = DeckViewModel<SolitaireCard>.EnumStyleType.AlwaysKnown;
        }
    }
}
