using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using BasicGameFrameworkLibrary.SolitaireClasses.GraphicsObservable;
using EagleWingsSolitaireCP.Logic;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using System.Windows;
using EagleWingsSolitaireCP.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
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
        public void Init(EagleWingsSolitaireMainViewModel thisMod)
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
