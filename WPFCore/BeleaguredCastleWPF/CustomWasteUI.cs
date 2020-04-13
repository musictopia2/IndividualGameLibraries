using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.GraphicsObservable;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Exceptions;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BeleaguredCastleWPF
{
    public class CustomWasteUI : UserControl
    {

        private Grid? _thisGrid;
        private SolitairePilesCP? _thisMod;

        public void Init(SolitairePilesCP mod, BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> thisMain)
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
                var thisSingle = new SinglePileUI();
                var thisPile = _thisMod.PileList[x - 1];
                thisSingle.Init(thisPile, _thisMod); // the first 4 are reversed
                AddControlToGrid(_thisGrid, thisSingle, x - 1, 0);
            }

            for (x = 5; x <= 8; x++)
            {
                var thisSingle = new SinglePileUI();
                var thisPile = _thisMod.PileList[x - 1];
                thisSingle.Init(thisPile, _thisMod); //no reverse anymore.
                thisSingle.Margin = new Thickness(20, 0, 0, 0);
                AddControlToGrid(_thisGrid, thisSingle, x - 5, 2);
            }

            Content = _thisGrid;
        }
    }
}
