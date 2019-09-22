using CommonBasicStandardLibraries.Exceptions;
using HeapSolitaireCP;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace HeapSolitaireWPF
{
    public class CustomWasteUI : UserControl
    {
        public void Init(HeapSolitaireViewModel thisMod)
        {
            Grid thisGrid = new Grid();
            var tempMod = thisMod.Waste1;
            if (tempMod!.PileList!.Count != 23)
                throw new BasicBlankException("Needed to have 23 piles");
            AddAutoRows(thisGrid, 5);
            AddAutoColumns(thisGrid, 5);
            tempMod.PileList.ForEach(thisPile =>
            {
                SinglePileUI thisSingle = new SinglePileUI();
                thisSingle.Margin = new Thickness(0, 0, 20, 20);
                thisSingle.Init(thisMod, thisPile);
                AddControlToGrid(thisGrid, thisSingle, thisPile.Row - 1, thisPile.Column - 1);
            });
            Content = thisGrid;
        }
    }
}