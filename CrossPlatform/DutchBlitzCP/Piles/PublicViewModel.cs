using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using System.Linq;

namespace DutchBlitzCP.Piles
{
    public class PublicViewModel : PublicPilesVM<DutchBlitzCardInformation>
    {
        private readonly DutchBlitzGameContainer _gameContainer;

        public PublicViewModel(DutchBlitzGameContainer gameContainer) : base(gameContainer.Command)
        {
            _gameContainer = gameContainer;
        }

        protected override int MaximumAllowed => 10;

        public bool CanCreatePile(int deck)
        {
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            return thisCard.CardValue == 1;
        }
        public bool CanAddToPile(int deck, int pile)
        {
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            return CanAddToPile(thisCard, pile);
        }
        public bool CanAddToPile(DutchBlitzCardInformation newCard, int pile)
        {
            if (HasCard(pile + 1) == false)
                return false;
            if (_gameContainer.Test!.AllowAnyMove == true && _gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
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