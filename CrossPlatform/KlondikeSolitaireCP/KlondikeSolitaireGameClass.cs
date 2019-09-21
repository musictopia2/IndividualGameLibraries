using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
namespace KlondikeSolitaireCP
{
    [SingletonGame]
    public class KlondikeSolitaireGameClass : SolitaireGameClass<KlondikeSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        public KlondikeSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
        }

        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }

        protected override void AfterShuffleCards()
        {
            _thisMod.MainPiles1!.ClearBoard();
            AfterShuffle();
        }
    }
}
