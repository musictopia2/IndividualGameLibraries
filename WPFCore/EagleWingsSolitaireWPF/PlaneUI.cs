using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BaseSolitaireClassesCP.Cards;
using BaseSolitaireClassesCP.GraphicsViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using EagleWingsSolitaireCP;
using SolitaireGraphicsWPFCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
//i think this is the most common things i like to do
namespace EagleWingsSolitaireWPF
{
    public class PlaneUI : UserControl
    {
        private StackPanel? _thisStack;
        private void LoadPiles(CustomBasicList<PileInfoCP> thisList, WastePiles thisWaste)
        {
            thisList.ForEach(thisPile =>
            {
                var pileG = new IndividualSolitairePileWPF();
                pileG.MainMod = thisWaste.Piles;
                pileG.ThisPile = thisPile;
                pileG.Margin = new Thickness(0, 13, 0, 0);
                _thisStack!.Children.Add(pileG);
                pileG.Init();
            });
        }
        public void Init(EagleWingsSolitaireViewModel thisMod)
        {
            var thisDeck = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            thisDeck.Init(thisMod.Heel1!, ts.TagUsed);
            WastePiles tempWaste = (WastePiles)thisMod.WastePiles1!;
            if (tempWaste.Piles.PileList.Count != 8)
                throw new BasicBlankException("Must have 8 piles in order to initialize the plane ui");
            var firstList = tempWaste.Piles.PileList.Take(4).ToCustomBasicList();
            _thisStack = new StackPanel();
            _thisStack.Orientation = Orientation.Horizontal;
            LoadPiles(firstList, tempWaste);
            _thisStack.Children.Add(thisDeck);
            var lastList = tempWaste.Piles.PileList.Skip(4).ToCustomBasicList();
            LoadPiles(lastList, tempWaste);
            Content = _thisStack;
        }
    }
}
