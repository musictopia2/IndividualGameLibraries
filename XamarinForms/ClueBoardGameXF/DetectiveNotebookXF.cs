using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using ClueBoardGameCP;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ClueBoardGameXF
{
    public class DetectiveNotebookXF : ContentView
    {
        public void LoadControls(BaseHandXF<CardInfo, CardCP, CardXF> hand)
        {
            ClueBoardGameViewModel thisMod = Resolve<ClueBoardGameViewModel>();
            Grid thisGrid = new Grid();
            AddAutoRows(thisGrid, 2);
            AddAutoColumns(thisGrid, 3);
            thisGrid.RowSpacing = 0;
            thisGrid.ColumnSpacing = 0;
            StackLayout firstRow = new StackLayout();
            StackLayout secondRow = new StackLayout();
            StackLayout thirdRow = new StackLayout();
            AddControlToGrid(thisGrid, firstRow, 0, 0);
            Grid.SetRowSpan(firstRow, 2);
            AddControlToGrid(thisGrid, secondRow, 0, 1);
            AddControlToGrid(thisGrid, thirdRow, 0, 2);
            AddControlToGrid(thisGrid, hand, 1, 1);
            Grid.SetColumnSpan(hand, 2);
            GlobalClass thisG = Resolve<GlobalClass>();
            var thisList = thisG.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsRoom).ToCustomBasicList();
            Button thisBut;
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem, thisMod);
                firstRow.Children.Add(thisBut);
            });
            thisList = thisG.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsCharacter).ToCustomBasicList();
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem, thisMod);
                secondRow.Children.Add(thisBut);
            });
            thisList = thisG.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsWeapon).ToCustomBasicList();
            if (thisList.Count != 6)
                throw new BasicBlankException("There has to be 6 weapons when loading controls");

            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem, thisMod);
                thirdRow.Children.Add(thisBut);
            });
            Content = thisGrid;
        }
        private Button GetButton(DetectiveInfo thisDet, ClueBoardGameViewModel thisMod)
        {
            Button output = new Button();
            output.Text = thisDet.Name;
            output.FontSize = 10;
            output.CommandParameter = thisDet;
            Binding thisBind = new Binding(nameof(DetectiveInfo.IsChecked));
            DetectiveConverter thisC = new DetectiveConverter();
            thisBind.Converter = thisC;
            output.BindingContext = thisDet;
            output.SetBinding(BackgroundColorProperty, thisBind);
            thisBind = new Binding(nameof(ClueBoardGameViewModel.FillClueCommand));
            thisBind.Source = thisMod;
            output.SetBinding(Button.CommandProperty, thisBind);
            output.CommandParameter = thisDet;
            return output;
        }
    }
}