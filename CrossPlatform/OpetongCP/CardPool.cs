using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OpetongCP
{
    public class CardPool : GameBoardViewModel<RegularRummyCard>
    {
        private readonly OpetongMainGameClass _mainGame;
        public CardPool(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<OpetongMainGameClass>();
            Rows = 2;
            Columns = 4;
            HasFrame = true;
            Text = "Card Pool";
        }
        protected override async Task ClickProcessAsync(RegularRummyCard thisObject)
        {
            await _mainGame.DrawFromPoolAsync(thisObject.Deck);
        }
        public int HowManyCardsNeeded
        {
            get
            {
                int nums = ObjectList.Count(items => items.Visible == false);
                if (nums > 2)
                    throw new BasicBlankException("Since a player gets 2 turns each time at the most; cannot have more than 2 cards that are needed");
                return nums;
            }
        }
        public void NewGame(DeckRegularDict<RegularRummyCard> thisCol)
        {
            if (thisCol.Count != 8)
                throw new BasicBlankException("There must be 8 cards for the card pool");
            DeckRegularDict<RegularRummyCard> newCol = new DeckRegularDict<RegularRummyCard>();
            thisCol.ForEach(oldCard =>
            {
                RegularRummyCard newCard = new RegularRummyCard();
                newCard.Populate(oldCard.Deck); //this is the way to clone it.
                newCol.Add(newCard);
            });
            ObjectList.ReplaceRange(newCol);
        }
        public void ProcessNewCards(DeckRegularDict<RegularRummyCard> thisCol)
        {
            if (ObjectList.Count != 8)
                throw new BasicBlankException("Must have 8 cards");
            if (thisCol.Count > 2)
                throw new BasicBlankException("There cannot be more than 2 new cards added");
            var newList = ObjectList.Where(items => items.Visible == false).ToRegularDeckDict();
            int x = 0;
            newList.ForEach(oldCard =>
            {
                RegularRummyCard newCard = new RegularRummyCard();
                newCard.Populate(thisCol[x].Deck);
                TradeObject(oldCard.Deck, newCard);
                x++;
            });
            if (ObjectList.Any(items => items.Visible == false))
                throw new BasicBlankException("The card was not changed.  Find out what happened");
        }
        public void HideCard(int deck)
        {
            var thisCard = ObjectList.GetSpecificItem(deck);
            thisCard.Visible = false;
        }
    }
}