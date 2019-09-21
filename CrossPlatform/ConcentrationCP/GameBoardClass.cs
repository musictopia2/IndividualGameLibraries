using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace ConcentrationCP
{
    public class GameBoardClass : BasicMultiplePilesCP<RegularSimpleCard>
    {
        private readonly ConcentrationMainGameClass _mainGame;
        public GameBoardClass(IBasicGameVM thisMod) : base(thisMod)
        {
            HasText = false;
            HasFrame = false;
            Rows = 5;
            Columns = 10;
            Style = EnumStyleList.SingleCard;
            _mainGame = thisMod.MainContainer!.Resolve<ConcentrationMainGameClass>();
            LoadBoard(); //maybe i forgot this part.
        }
        public override void ClearBoard()
        {
            int x = 0;
            if (_mainGame.DeckList!.Count != 50)
                throw new BasicBlankException("Must have 50 cards total");
            if (PileList!.Count != 50)
                throw new BasicBlankException("There should have been 50 piles.");
            _mainGame.DeckList.ForEach(thisCard =>
            {
                x++;
                var ThisPile = PileList[x - 1]; //0 based
                ThisPile.ThisObject = thisCard;
                ThisPile.ThisObject.IsUnknown = true;
                thisCard.Visible = true;
                ThisPile.Visible = true; //try to do this to double check.
                ThisPile.ThisObject.IsSelected = false;
            });
            NewCardProcess(); //maybe can do after all is done (well see)
        }
        public void SelectedCardsGone()
        {
            PileList!.ForConditionalItems(ttems => ttems.IsSelected == true, thisPile =>
            {
                thisPile.IsSelected = false;
                thisPile.ThisObject.Visible = false;
            });
        }
        public void UnselectCards()
        {
            PileList!.ForConditionalItems(items => items.IsSelected == true, thisPile =>
            {
                thisPile.IsSelected = false;
                thisPile.ThisObject.IsUnknown = true;
            });
        }
        public bool CardsGone => PileList.All(items => items.ThisObject.Visible == false);
        public DeckRegularDict<RegularSimpleCard> GetSelectedCards()
        {
            var output = PileList.Where(items => items.IsSelected == true).Select(Items => Items.ThisObject).ToRegularDeckDict();
            if (output.Count > 2)
                throw new BasicBlankException("Cannot have more than 2 selected.  Find out what happened");
            return output;
        }
        private BasicPileInfo<RegularSimpleCard> FindPile(int deck)
        {
            return PileList.Single(items => items.ThisObject.Deck == deck);
        }
        public DeckRegularDict<RegularSimpleCard> CardsLeft() => PileList.Where(items => items.Visible == true && items.IsSelected == false).Select(Items => Items.ThisObject).ToRegularDeckDict();
        public bool WasSelected(int deck)
        {
            var thisPile = FindPile(deck);
            return thisPile.IsSelected;
        }
        public void SelectCard(int deck)
        {
            var thisPile = FindPile(deck);
            if (thisPile.ThisObject.Visible == false)
                throw new BasicBlankException("Cannot select card because it was not visible");
            thisPile.IsSelected = true;
            thisPile.ThisObject.IsUnknown = false;
        }
    }
}