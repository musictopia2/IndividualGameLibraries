using AgnesSolitaireCP.Data;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace AgnesSolitaireCP.Logic
{
    [SingletonGame]
    public class AgnesSolitaireMainGameClass : SolitaireGameClass<AgnesSolitaireSaveInfo>
    {
        public AgnesSolitaireMainGameClass(ISolitaireData solitaireData1,
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

        private WastePiles? _thisWaste;
        public override Task NewGameAsync()
        {
            if (_thisWaste == null)
                _thisWaste = (WastePiles)_thisMod!.WastePiles1!;
            return base.NewGameAsync();
        }
        public override void DrawCard()
        {
            if (_thisMod!.DeckPile!.CardsLeft() <= 2)
            {
                base.DrawCard();
                return;
            }
            var tempList = _thisMod.DeckPile.DrawSeveralCards(7);
            _thisWaste!.AddCards(tempList);
        }
        protected override void AfterShuffleCards()
        {
            var thisCard = CardList![28];
            CardList.RemoveSpecificItem(thisCard);
            AfterShuffle(new DeckRegularDict<SolitaireCard>() { thisCard });
        }
    }
}
