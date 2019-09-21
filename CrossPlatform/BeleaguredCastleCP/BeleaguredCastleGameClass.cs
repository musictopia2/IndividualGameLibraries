using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
namespace BeleaguredCastleCP
{
    [SingletonGame]
    public class BeleaguredCastleGameClass : SolitaireGameClass<BeleaguredCastleSaveInfo>
    {
        public BeleaguredCastleGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            //_thisMod = thisMod;
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
