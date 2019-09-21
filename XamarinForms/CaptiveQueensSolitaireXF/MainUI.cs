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
using Xamarin.Forms;
using CaptiveQueensSolitaireCP;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGameFramework.MultiplePilesViewModels;
using BaseSolitaireClassesCP.Cards;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;

//i think this is the most common things i like to do
namespace CaptiveQueensSolitaireXF
{
    public class MainUI : ContentView
    {
        private Grid? _thisGrid;
        private CustomMain? _mainMod;
        public void Init()
        {
            _thisGrid = new Grid();
            AddAutoColumns(_thisGrid, 4);
            AddAutoRows(_thisGrid, 4);
            var thisWaste = new SimpleWasteUI();
            thisWaste.Init();
            thisWaste.HorizontalOptions = LayoutOptions.Start;
            thisWaste.VerticalOptions = LayoutOptions.Start;
            AddControlToGrid(_thisGrid, thisWaste, 1, 1);
            Grid.SetRowSpan(thisWaste, 2);
            Grid.SetColumn(thisWaste, 2);
            thisWaste.Margin = new Thickness(-150, 10, 10, 10);
            BasicPileInfo<SolitaireCard> thisPile;
            int x;
            _mainMod = Resolve<CustomMain>();
            for (x = 1; x <= 2; x++)
            {
                thisPile = _mainMod.PileList![x - 1];
                AddPile(thisPile, 0, x);
            }

            for (x = 3; x <= 4; x++)
            {
                thisPile = _mainMod.PileList![x - 1];
                AddPile(thisPile, x - 2, 3);
            }

            for (x = 5; x <= 6; x++)
            {
                // 4 and 5
                thisPile = _mainMod.PileList![x - 1];
                AddPile(thisPile, 3, x - 4);
            }

            for (x = 7; x <= 8; x++)
            {
                thisPile = _mainMod.PileList![x - 1];
                AddPile(thisPile, x - 6, 0);
            }
            Content = _thisGrid;
        }
        private void AddPile(BasicPileInfo<SolitaireCard> thisPile, int row, int column)
        {
            PrivateBasicIndividualPileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> pileUI = new PrivateBasicIndividualPileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            pileUI.MainMod = _mainMod;
            pileUI.Margin = new Thickness(15, 15, 15, 15);
            pileUI.ThisPile = thisPile;
            pileUI.Init(ts.TagUsed);
            AddControlToGrid(_thisGrid!, pileUI, row, column);
        }
    }
}
