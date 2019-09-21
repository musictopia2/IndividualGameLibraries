using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace RaglanSolitaireCP
{
    [SingletonGame]
    public class RaglanSolitaireGameClass : SolitaireGameClass<RaglanSolitaireSaveInfo>
    {
        private readonly RaglanSolitaireViewModel _thisMod;
        public RaglanSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = (RaglanSolitaireViewModel)thisMod;
        }
        protected override async Task<bool> HasOtherAsync(int pile)
        {
            if (_thisMod.Stock1!.ObjectSelected() == 0)
            {
                return await base.HasOtherAsync(pile);
            }
            int wastes = _thisMod.WastePiles1!.OneSelected();
            if (wastes > -1)
            {
                await _thisMod.ShowGameMessageAsync("Cannot choose both from the waste and the stock");
                return true;
            }
            var thisCard = _thisMod.Stock1.HandList.GetSpecificItem(_thisMod.Stock1.ObjectSelected());
            if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
            {
                await _thisMod.ShowGameMessageAsync("Illegal move");
                return true;
            }
            _thisMod.WastePiles1.AddSingleCard(pile, thisCard);
            _thisMod.Stock1.HandList.RemoveObjectByDeck(thisCard.Deck);
            return true;
        }
        protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
        {
            _thisMod.Stock1!.HandList.RemoveObjectByDeck(thisCard.Deck);
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            if (thisCard.Deck > 0)
            {
                if (_thisMod.Stock1!.ObjectSelected() > 0)
                    return new SolitaireCard();
                return thisCard;
            }
            thisCard = _thisMod.Stock1!.HandList.GetSpecificItem(_thisMod.Stock1.ObjectSelected());
            return thisCard;
        }

        protected override void AfterShuffleCards()
        {
            var aceList = GetAceList();
            AfterShuffle(aceList);
        }
        protected override void PopulateDeck(IEnumerableDeck<SolitaireCard> leftOverList)
        {
            _thisMod.Stock1!.HandList.ReplaceRange(leftOverList);
        }
        protected async override Task ContinueOpenSavedAsync()
        {
            //anything else you need will be here
            _thisMod.Stock1!.HandList.ReplaceRange(SaveRoot.StockCards);
            await base.ContinueOpenSavedAsync();
        }
        protected async override Task FinishSaveAsync()
        {
            //anything else to finish save will be here.
            SaveRoot.StockCards = _thisMod.Stock1!.HandList.ToRegularDeckDict();
            await base.FinishSaveAsync();
        }
    }
}