using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace GolfCardGameCP
{
    public class HiddenCards : BasicMultiplePilesCP<RegularSimpleCard>
    {
        readonly GolfCardGameMainGameClass _mainGame;
        public HiddenCards(IBasicGameVM thisMod) : base(thisMod)
        {
            Rows = 1;
            Columns = 2;
            HasFrame = true;
            Style = BasicMultiplePilesCP<RegularSimpleCard>.EnumStyleList.SingleCard;
            HasText = true;
            Visible = false;
            LoadBoard();
            IsEnabled = false; //try this too (?)
            if (PileList!.Count != 2)
                throw new BasicBlankException("Must have 2 piles");
            PileClickedAsync += HiddenCards_PileClickedAsync;
            _mainGame = thisMod.MainContainer!.Resolve<GolfCardGameMainGameClass>();
            int x = 0;
            PileList.ForEach(thisPile =>
            {
                x++;
                thisPile.Text = $"Card {x}";
            });
        }
        public void ChangeCard(int pile, RegularSimpleCard newCard)
        {
            var thisPile = PileList![pile];
            thisPile.ThisObject = newCard;
            thisPile.IsEnabled = false;
        }
        public void RevealCards()
        {
            PileList!.ForEach(thisPile => thisPile.ThisObject.IsUnknown = false);
        }
        public override void ClearBoard()
        {
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            if (_mainGame.SingleInfo.TempSets.Count != 2)
                throw new BasicBlankException("Needs 2 cards for the hidden cards");
            int x = 0;
            _mainGame.SingleInfo.TempSets.ForEach(thisCard =>
            {
                var thisPile = PileList![x];
                thisPile.ThisObject = thisCard;
                thisPile.ThisObject.IsUnknown = true;
                thisPile.IsEnabled = true;
                x++;
            });
            Visible = true;
        }
        private async Task HiddenCards_PileClickedAsync(int index, BasicPileInfo<RegularSimpleCard> thisPile)
        {
            await _mainGame.ChangeUnknownAsync(index);
        }
    }
}