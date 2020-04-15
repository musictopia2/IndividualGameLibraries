using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.GraphicsObservable;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using EagleWingsSolitaireCP.Logic;
using EagleWingsSolitaireCP.ViewModels;
using System.Linq;
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace EagleWingsSolitaireXF
{
    public class PlaneUI : ContentView
    {
        private StackLayout? _thisStack;
        private void LoadPiles(CustomBasicList<PileInfoCP> thisList, WastePiles thisWaste)
        {

            thisList.ForEach(thisPile =>
            {
                var pileG = new IndividualSolitairePileXF();
                pileG.MainMod = thisWaste.Piles;
                pileG.ThisPile = thisPile;
                pileG.Margin = new Thickness(0, 13, 0, 0);
                _thisStack!.Children.Add(pileG);
                pileG.Init(_needsPopulating);
            });
        }
        private static bool _needsPopulating = true;
        public void Init(EagleWingsSolitaireMainViewModel thisMod)
        {
            var thisDeck = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            thisDeck.Init(thisMod.Heel1!, ts.TagUsed);
            WastePiles tempWaste = (WastePiles)thisMod.WastePiles1!;
            if (tempWaste.Piles.PileList.Count != 8)
                throw new BasicBlankException("Must have 8 piles in order to initialize the plane ui");
            var firstList = tempWaste.Piles.PileList.Take(4).ToCustomBasicList();
            _thisStack = new StackLayout();
            _thisStack.Orientation = StackOrientation.Horizontal;
            LoadPiles(firstList, tempWaste);
            _thisStack.Children.Add(thisDeck);
            var lastList = tempWaste.Piles.PileList.Skip(4).ToCustomBasicList();
            LoadPiles(lastList, tempWaste);
            Content = _thisStack;
            _needsPopulating = false;
        }
    }
}