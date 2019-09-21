using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.SpecializedGameTypes.StockClasses;
using BasicGameFramework.ViewModelInterfaces;
using System.Linq;
namespace DutchBlitzCP
{
    public class StockViewModel : StockPileVM<DutchBlitzCardInformation>
    {
        public StockViewModel(IBasicGameVM thisMod) : base(thisMod) { }

        public override string NextCardInStock()
        {
            if (DidGoOut() == true)
                return "0";
            var ThisCard = GetCard();
            return ThisCard.Display; // i think
        }
    }
    public class PublicViewModel : PublicPilesVM<DutchBlitzCardInformation>
    {

        private readonly DutchBlitzMainGameClass _mainGame;

        public PublicViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<DutchBlitzMainGameClass>();
        }

        protected override int MaximumAllowed => 10;

        public bool CanCreatePile(int deck)
        {
            var thisCard = _mainGame.DeckList!.GetSpecificItem(deck);
            return thisCard.CardValue == 1;
        }
        public bool CanAddToPile(int deck, int pile)
        {
            var thisCard = _mainGame.DeckList!.GetSpecificItem(deck);
            return CanAddToPile(thisCard, pile);
        }
        public bool CanAddToPile(DutchBlitzCardInformation newCard, int pile)
        {
            if (HasCard(pile + 1) == false)
                return false;
            if (_mainGame.ThisTest!.AllowAnyMove == true && _mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                return true; //while i test.
            var thisCard = GetLastCard(pile + 1);
            return thisCard.CardValue + 1 == newCard.CardValue;
        }
        public int PointsPlayed(int player, DeckRegularDict<DutchBlitzCardInformation> otherList)
        {
            int output = 0;
            PileList.ForEach(thisPile =>
            {
                var tempList = thisPile.ObjectList.ToRegularDeckDict();
                output += tempList.Count(items => items.Player == player);
            });
            output += otherList.Count(items => items.Player == player);
            return output;
        }
    }
}