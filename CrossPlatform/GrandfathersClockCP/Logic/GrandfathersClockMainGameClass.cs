using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Messenging;
using GrandfathersClockCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace GrandfathersClockCP.Logic
{
    [SingletonGame]
    public class GrandfathersClockMainGameClass : SolitaireGameClass<GrandfathersClockSaveInfo>
    {
        public GrandfathersClockMainGameClass(ISolitaireData solitaireData1,
            ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        //rethink if i need view model.  hopefully won't happen though.

        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            await base.FinishSaveAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }
        protected override void AfterShuffleCards()
        {
            DeckRegularDict<SolitaireCard> thisList = new DeckRegularDict<SolitaireCard>
            {
                FindCardBySuitValue(EnumCardValueList.Ten, EnumSuitList.Hearts),
                FindCardBySuitValue(EnumCardValueList.Jack, EnumSuitList.Spades),
                FindCardBySuitValue(EnumCardValueList.Queen, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumCardValueList.King, EnumSuitList.Clubs),
                FindCardBySuitValue(EnumCardValueList.Two, EnumSuitList.Hearts),
                FindCardBySuitValue(EnumCardValueList.Three, EnumSuitList.Spades),
                FindCardBySuitValue(EnumCardValueList.Four, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumCardValueList.Five, EnumSuitList.Clubs),
                FindCardBySuitValue(EnumCardValueList.Six, EnumSuitList.Hearts),
                FindCardBySuitValue(EnumCardValueList.Seven, EnumSuitList.Spades),
                FindCardBySuitValue(EnumCardValueList.Eight, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumCardValueList.Nine, EnumSuitList.Clubs)
            };
            CardList!.RemoveGivenList(thisList);
            AfterShuffle(thisList);
        }
    }
}