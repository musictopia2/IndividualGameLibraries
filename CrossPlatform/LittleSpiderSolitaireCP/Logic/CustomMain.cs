using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.DataClasses;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LittleSpiderSolitaireCP.Data;
namespace LittleSpiderSolitaireCP.Logic
{
    public class CustomMain : MainPilesCP
    {
        //private readonly LittleSpiderSolitaireMainViewModel _thisMod;
        public CustomMain(IScoreData score,
            CommandContainer command,
            IGamePackageResolver resolver,
            IEventAggregator aggregator) : base(score, command, resolver, aggregator)
        {
            //_thisMod = GlobalClass.MainModel!;
        }
        public override bool CanAddCard(int pile, SolitaireCard thisCard)
        {
            if (GlobalClass.MainModel!.DeckPile!.IsEndOfDeck() && pile > 1)
                return base.CanAddCard(pile, thisCard);
            if (GlobalClass.MainModel!.DeckPile.IsEndOfDeck())
                return CanBuildFromKing(pile, thisCard);
            if (GlobalClass.MainModel!.WastePiles1!.OneSelected() == -1)
                throw new BasicBlankException($"No card selected for placing to {pile}");
            if (GlobalClass.MainModel!.WastePiles1.OneSelected() <= 3)
            {
                if (pile > 1)
                    return base.CanAddCard(pile, thisCard);
                return CanBuildFromKing(pile, thisCard);
            }
            int oldPile = GlobalClass.MainModel!.WastePiles1.OneSelected() - 4;
            if (oldPile != pile)
                return false;
            if (pile > 1)
                return base.CanAddCard(pile, thisCard);
            return CanBuildFromKing(pile, thisCard);
        }
        private bool CanBuildFromKing(int pile, SolitaireCard thisCard)
        {
            if (pile > 1)
                throw new BasicBlankException($"Kings must be 0, or 1; not {pile}");
            var oldCard = Piles.GetLastCard(pile);
            if (oldCard.Suit != thisCard.Suit)
                return false;
            return oldCard.Value - 1 == thisCard.Value;
        }
    }
}
