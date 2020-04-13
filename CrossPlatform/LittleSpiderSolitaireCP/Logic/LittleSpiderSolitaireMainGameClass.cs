using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LittleSpiderSolitaireCP.Data;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace LittleSpiderSolitaireCP.Logic
{
    [SingletonGame]
    public class LittleSpiderSolitaireMainGameClass : SolitaireGameClass<LittleSpiderSolitaireSaveInfo>
    {
        public LittleSpiderSolitaireMainGameClass(ISolitaireData solitaireData1,
            ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        private WastePiles? _thisWaste;
        public override Task NewGameAsync()
        {
            if (_thisWaste == null)
                _thisWaste = (WastePiles)_thisMod!.WastePiles1;
            return base.NewGameAsync();
        }
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

        public override void DrawCard()
        {
            var thisList = _thisMod!.DeckPile!.DrawSeveralCards(8);
            _thisWaste!.AddCards(thisList);
        }
        protected override void AfterShuffleCards()
        {
            var firstKing = CardList.First(items => items.Value == EnumCardValueList.King);
            var thisList = new DeckRegularDict<SolitaireCard>
            {
                firstKing
            };
            CardList!.RemoveSpecificItem(firstKing);
            var nextKing = CardList.First(items => items.Value == EnumCardValueList.King && items.Color == firstKing.Color);
            CardList.RemoveSpecificItem(nextKing);
            thisList.Add(nextKing);
            var nextList = CardList.Where(items => items.Value == EnumCardValueList.LowAce && items.Color != firstKing.Color).ToRegularDeckDict();
            CardList.RemoveGivenList(nextList);
            thisList.AddRange(nextList);
            if (thisList.Count != 4)
                throw new BasicBlankException("Must have 4 cards for foundation");
            AfterShuffle(thisList);
            CardList.Clear();
        }


    }
}
