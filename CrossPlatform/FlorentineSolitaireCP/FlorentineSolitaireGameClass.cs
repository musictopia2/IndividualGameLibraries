using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FlorentineSolitaireCP
{
    [SingletonGame]
    public class FlorentineSolitaireGameClass : SolitaireGameClass<FlorentineSolitaireSaveInfo>
    {
        public FlorentineSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod) { }
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
                CardList![5]
            };
            CardList.RemoveAt(5);
            AfterShuffle(thisList);
        }
    }
}
