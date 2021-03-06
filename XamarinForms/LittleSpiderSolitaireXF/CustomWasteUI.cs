using BasicGameFrameworkLibrary.MultiplePilesObservable;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using LittleSpiderSolitaireCP.Logic;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace LittleSpiderSolitaireXF
{
    public class CustomWasteUI : ContentView
    {
        private Grid? _thisGrid;
        public void Init(BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> main)
        {
            WastePiles thisWaste = Resolve<WastePiles>();
            if (thisWaste.Discards!.PileList!.Count != 8)
                throw new BasicBlankException("There must be 8 piles total");
            _thisGrid = new Grid();
            Margin = new Thickness(10, 5, 5, 5);
            AddAutoColumns(_thisGrid, 4);
            AddAutoRows(_thisGrid, 3);
            var thisList = thisWaste.Discards.PileList.Take(4).ToCustomBasicList();
            main.Margin = new Thickness(0, 0, 0, 0);
            CustomMain thisMain = Resolve<CustomMain>();
            main.Init(thisMain.Piles, ts.TagUsed);
            LoadList(0, thisList, thisWaste);
            AddControlToGrid(_thisGrid, main, 1, 0);
            Grid.SetColumnSpan(main, 4);
            thisList = thisWaste.Discards.PileList.Skip(4).ToCustomBasicList();
            LoadList(2, thisList, thisWaste);
            Content = _thisGrid;
        }
        private void LoadList(int row, CustomBasicList<BasicPileInfo<SolitaireCard>> thisList, WastePiles thisWaste)
        {
            int x = 0;
            thisList.ForEach(thisItem =>
            {
                var thisBasic = new PrivateBasicIndividualPileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
                thisBasic.ThisPile = thisItem;
                thisBasic.Margin = new Thickness(3, 3, 3, 3);
                thisBasic.MainMod = thisWaste.Discards;
                thisBasic.Init(ts.TagUsed);
                AddControlToGrid(_thisGrid!, thisBasic, row, x);
                x++;
            });
        }
    }
}