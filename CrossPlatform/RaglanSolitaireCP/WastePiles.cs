using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace RaglanSolitaireCP
{
    public class WastePiles : WastePilesCP
    {
        public WastePiles(IBasicGameVM thisMod) : base(thisMod) { }
        public override void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            base.ClearBoard(thisCol);
            //below is custom code.
            int z = 7;
            int q = 0;
            int y = 0;

            Piles.PileList.ForEach(thisPile =>
            {
                q++;
                if (q > 3)
                    z--;
                z.Times(x =>
                {
                    var thisCard = thisCol[y];
                    thisPile.TempList.Add(thisCard);
                    y++;
                });
            });
            Piles.PileList.ForEach(thisPile => thisPile.CardList.ReplaceRange(thisPile.TempList));
        }
        public override bool CanAddSingleCard(int whichOne, SolitaireCard thisCard)
        {
            if (Piles.HasCardInColumn(whichOne) == false)
                return true;
            var oldCard = Piles.GetLastCard(whichOne);
            if (thisCard.Color == oldCard.Color)
                return false;
            return oldCard.Value - 1 == thisCard.Value;
        }

        public override bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1; //until i figure out what else to do.
            return false;
        }

        public override bool CanMoveToAnotherPile(int whichOne)
        {
            var thisCard = Piles.GetLastCard(PreviousSelected);
            return CanAddSingleCard(whichOne, thisCard);
        }
    }
}
