using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.GraphicsObservable;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Exceptions;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BeleaguredCastleXF
{
    public class CustomWasteUI : ContentView
    {
        private Grid? _thisGrid;
        private SolitairePilesCP? _thisMod;
        public void Init(SolitairePilesCP mod, BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> thisMain)
        {
            _thisMod = mod;
            if (_thisMod.PileList.Count != 8)
                throw new BasicBlankException("Must have 8 piles");
            _thisGrid = new Grid();
            AddAutoRows(_thisGrid, 4);
            AddLeftOverColumn(_thisGrid, 40);
            AddAutoColumns(_thisGrid, 1);
            AddLeftOverColumn(_thisGrid, 40);
            AddControlToGrid(_thisGrid, thisMain, 0, 1);
            Grid.SetRowSpan(thisMain, 4); //all 4 for main.
            int x;
            for (x = 1; x <= 4; x++)
            {
                var ThisSingle = new SinglePileUI();
                var ThisPile = _thisMod.PileList[x - 1];
                ThisSingle.Init(ThisPile, _thisMod); // the first 4 are reversed
                AddControlToGrid(_thisGrid, ThisSingle, x - 1, 0);
            }
            for (x = 5; x <= 8; x++)
            {
                var ThisSingle = new SinglePileUI();
                var ThisPile = _thisMod.PileList[x - 1];
                ThisSingle.Init(ThisPile, _thisMod); //no reverse anymore.
                ThisSingle.Margin = new Thickness(20, 0, 0, 0);
                AddControlToGrid(_thisGrid, ThisSingle, x - 5, 2);
            }
            Content = _thisGrid;
        }
    }
}