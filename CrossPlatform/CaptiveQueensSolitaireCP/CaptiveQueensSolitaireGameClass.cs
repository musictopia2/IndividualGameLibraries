using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CaptiveQueensSolitaireCP
{
    [SingletonGame]
    public class CaptiveQueensSolitaireGameClass : SolitaireGameClass<CaptiveQueensSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        public CaptiveQueensSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
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
            _thisMod.MainPiles1!.ClearBoard();
            AfterShuffle();
        }
    }
}
