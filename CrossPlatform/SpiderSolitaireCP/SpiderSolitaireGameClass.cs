using BaseSolitaireClassesCP.BasicVMInterfaces;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SpiderSolitaireCP
{
    [SingletonGame]
    public class SpiderSolitaireGameClass : SolitaireGameClass<SpiderSolitaireSaveInfo>
    {
        private readonly IBasicSolitaireVM _thisMod;
        private WastePiles? _tempWaste;
        public SpiderSolitaireGameClass(IBasicSolitaireVM thisMod) : base(thisMod)
        {
            _thisMod = thisMod;
        }
        public override Task NewGameAsync()
        {
            if (_tempWaste == null)
                _tempWaste = (WastePiles)_thisMod.WastePiles1!;
            return base.NewGameAsync();
        }
        protected override SolitaireCard CardPlayed()
        {
            var thisCard = base.CardPlayed();
            return thisCard;
            //if any changes, will be here.
        }

        protected override void AfterShuffleCards()
        {
            //at this point; it already shuffled the cards.  now figure out what to do from here
            _thisMod.MainPiles1!.ClearBoard();
            AfterShuffle();
        }
        public override void DrawCard()
        {
            CardList = _thisMod.DeckPile!.DrawSeveralCards(10, false);
            _tempWaste!.AddCards(CardList);
            10.Times(x =>
            {
                if (_tempWaste.CanMoveAll(x - 1))
                    MoveCards(x - 1);
            });
        }
        private void MoveCards(int whichOne)
        {
            var tempList = _tempWaste!.MoveList();
            if (tempList.Count != 13)
                throw new BasicBlankException("Must have 13 cards.  Otherwise, can't move the cards");
            _thisMod.MainPiles1!.AddCards(tempList);
            _tempWaste.RemoveCards(whichOne);
        }
        public override async Task AfterMovingCardsAsync(int whichOne)
        {
            if (_tempWaste!.CanMoveAll(whichOne) == false)
            {
                await base.AfterMovingCardsAsync(whichOne);
                return;
            }
            MoveCards(whichOne);
            if (_thisMod.Score == HowManyCards)
            {
                await ShowWinAsync();
                return;
            }
            await base.AfterMovingCardsAsync(whichOne);
        }
    }
}