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
using EagleWingsSolitaireCP.Data;
using BasicGameFrameworkLibrary.DrawableListsObservable;
//i think this is the most common things i like to do
namespace EagleWingsSolitaireCP.Logic
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
            Piles.PileList.ForEach(thisPile => thisPile.CardList.Add(thisCol[Piles.PileList.IndexOf(thisPile)]));
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false && GlobalClass.MainModel!.Heel1.IsEndOfDeck())
                return true;
            return Piles.HasCardInColumn(whichOne) == false && GlobalClass.MainModel!.Heel1!.CardsLeft() == 1;
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
            if (Piles.HasCardInColumn(Lasts) || GlobalClass.MainModel!.Heel1!.IsEndOfDeck() || GlobalClass.MainModel.Heel1.CardsLeft() == 1)
                return;
            Piles.AddCardToColumn(Lasts, GlobalClass.MainModel.Heel1.DrawCard());
            if (GlobalClass.MainModel.Heel1.CardsLeft() == 1)
                GlobalClass.MainModel.Heel1.DeckStyle = DeckObservablePile<SolitaireCard>.EnumStyleType.AlwaysKnown;
        }
    }
}
