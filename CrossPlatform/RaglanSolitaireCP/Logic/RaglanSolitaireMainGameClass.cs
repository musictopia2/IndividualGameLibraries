using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.MainClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RaglanSolitaireCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace RaglanSolitaireCP.Logic
{
    [SingletonGame]
    public class RaglanSolitaireMainGameClass : SolitaireGameClass<RaglanSolitaireSaveInfo>
    {
        public RaglanSolitaireMainGameClass(ISolitaireData solitaireData1,
            ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IScoreData score
            )
            : base(solitaireData1, thisState, aggregator, score)
        {
        }
        protected override async Task<bool> HasOtherAsync(int pile)
        {
            if (GlobalClass.Stock!.ObjectSelected() == 0)
            {
                return await base.HasOtherAsync(pile);
            }
            int wastes = _thisMod!.WastePiles1!.OneSelected();
            if (wastes > -1)
            {
                await UIPlatform.ShowMessageAsync("Cannot choose both from the waste and the stock");
                return true;
            }
            var thisCard = GlobalClass.Stock.HandList.GetSpecificItem(GlobalClass.Stock.ObjectSelected());
            if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal move");
                return true;
            }
            _thisMod.WastePiles1.AddSingleCard(pile, thisCard);
            GlobalClass.Stock.HandList.RemoveObjectByDeck(thisCard.Deck);
            return true;
        }
        protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
        {
            GlobalClass.Stock!.HandList.RemoveObjectByDeck(thisCard.Deck);
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
            {
                if (GlobalClass.Stock!.ObjectSelected() > 0)
                    return new SolitaireCard();
                return thisCard;
            }
            thisCard = GlobalClass.Stock!.HandList.GetSpecificItem(GlobalClass.Stock.ObjectSelected());
            return thisCard;
        }

        protected override void AfterShuffleCards()
        {
            var aceList = GetAceList();
            AfterShuffle(aceList);
        }
        protected override void PopulateDeck(IEnumerableDeck<SolitaireCard> leftOverList)
        {
            GlobalClass.Stock!.HandList.ReplaceRange(leftOverList);
        }
        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            GlobalClass.Stock!.HandList.ReplaceRange(SaveRoot.StockCards);
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.StockCards = GlobalClass.Stock!.HandList.ToRegularDeckDict();
            await base.FinishSaveAsync();
        }

    }
}