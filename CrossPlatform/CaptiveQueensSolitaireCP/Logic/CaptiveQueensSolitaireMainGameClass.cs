using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CaptiveQueensSolitaireCP.Data;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace CaptiveQueensSolitaireCP.Logic
{
    [SingletonGame]
    public class CaptiveQueensSolitaireMainGameClass : SolitaireGameClass<CaptiveQueensSolitaireSaveInfo>
    {
        public CaptiveQueensSolitaireMainGameClass(ISolitaireData solitaireData1,
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
            DeckRegularDict<SolitaireCard> output = new DeckRegularDict<SolitaireCard>
            {
                FindCardBySuitValue(EnumCardValueList.Queen, EnumSuitList.Spades),
                FindCardBySuitValue(EnumCardValueList.Queen, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumCardValueList.Queen, EnumSuitList.Clubs),
                FindCardBySuitValue(EnumCardValueList.Queen, EnumSuitList.Hearts)
            };
            CardList!.RemoveGivenList(output);
            output.Reverse();
            output.ForEach(thisCard => CardList.InsertBeginning(thisCard));
            _thisMod!.MainPiles1!.ClearBoard();
            AfterShuffle();
        }


    }
}
