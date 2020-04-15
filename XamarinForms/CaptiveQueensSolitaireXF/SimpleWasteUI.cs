using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CaptiveQueensSolitaireCP.Logic;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CaptiveQueensSolitaireXF
{
    public class SimpleWasteUI : ContentView
    {
        public void Init()
        {
            CustomWaste thisWaste = Resolve<CustomWaste>(); //can't do unit testing with ui anyways.
            if (thisWaste.CardList.Count != 4)
                throw new BasicBlankException("Must have 4 piles");
            if (thisWaste.CardList.Any(items => items.Value != EnumCardValueList.Queen))
                throw new BasicBlankException("Only Queens are supported for the special waste piles");
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2);
            var thisCard = new DeckOfCardsXF<SolitaireCard>();
            thisCard.SendSize(ts.TagUsed, thisWaste.CardList.First());
            var tempSize = thisCard.ObjectSize.Height * .67f;
            thisCard.Margin = new Thickness(0, tempSize, 0, 0);
            AddControlToGrid(thisGrid, thisCard, 0, 0);
            var newCard = new DeckOfCardsXF<SolitaireCard>();
            newCard.SendSize(ts.TagUsed, thisWaste.CardList[1]);
            var tempWidth = thisCard.ObjectSize.Width;
            var tempHeight = thisCard.ObjectSize.Height;
            newCard.Margin = new Thickness(tempWidth - 10, tempHeight + 10, 0, 0);
            AddControlToGrid(thisGrid, newCard, 0, 0);
            thisCard = new DeckOfCardsXF<SolitaireCard>();
            thisCard.SendSize(ts.TagUsed, thisWaste.CardList[2]);
            thisCard.Margin = new Thickness((tempSize / 2) * -1, tempSize, 0, 0);
            AddControlToGrid(thisGrid, thisCard, 0, 1);
            newCard = new DeckOfCardsXF<SolitaireCard>();
            newCard.SendSize(ts.TagUsed, thisWaste.CardList.Last());
            newCard.Margin = new Thickness(tempWidth * -1, 0, 0, 0);
            AddControlToGrid(thisGrid, newCard, 0, 1);
            Content = thisGrid;
        }
    }
}
