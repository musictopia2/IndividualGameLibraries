using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CarpetSolitaireCP
{
    [SingletonGame]
    public class CarpetSolitaireGameClass : SolitaireGameClass<CarpetSolitaireSaveInfo>
    {
        public CarpetSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod) { }
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
            var aceList = GetAceList();
            AfterShuffle(aceList);
        }
    }
}
