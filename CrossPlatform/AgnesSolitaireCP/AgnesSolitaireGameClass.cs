using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace AgnesSolitaireCP
{
    [SingletonGame]
    public class AgnesSolitaireGameClass : SolitaireGameClass<AgnesSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        public AgnesSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
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
        private WastePiles? _thisWaste;
        public override Task NewGameAsync()
        {
            if (_thisWaste == null)
                _thisWaste = (WastePiles)_thisMod.WastePiles1!;
            return base.NewGameAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }
        public override void DrawCard()
        {
            if (_thisMod.DeckPile!.CardsLeft() <= 2)
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
