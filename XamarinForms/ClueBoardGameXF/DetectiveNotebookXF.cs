using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using ClueBoardGameCP.Cards;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.Graphics;
using ClueBoardGameCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace ClueBoardGameXF
{
    public class DetectiveNotebookXF : ContentView
    {
        public void LoadControls(BaseHandXF<CardInfo, CardCP, CardXF> hand, Grid details, ClueBoardGameGameContainer gameContainer)
        {
            Grid thisGrid = new Grid();
            AddAutoRows(thisGrid, 3);
            AddAutoColumns(thisGrid, 2);
            thisGrid.RowSpacing = 0;
            thisGrid.ColumnSpacing = 0;
            StackLayout firstRow = new StackLayout();
            StackLayout secondRow = new StackLayout();
            StackLayout thirdRow = new StackLayout();
            AddControlToGrid(thisGrid, firstRow, 0, 0);
            Grid.SetRowSpan(firstRow, 2);
            AddControlToGrid(thisGrid, secondRow, 0, 1);
            AddControlToGrid(thisGrid, thirdRow, 0, 2);
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(hand);
            finalStack.Children.Add(details);
            AddControlToGrid(thisGrid, finalStack, 1, 1);
            Grid.SetColumnSpan(finalStack, 2);
            var thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsRoom).ToCustomBasicList();
            Button thisBut;
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                firstRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsCharacter).ToCustomBasicList();
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                secondRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsWeapon).ToCustomBasicList();
            if (thisList.Count != 6)
                throw new BasicBlankException("There has to be 6 weapons when loading controls");

            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                thirdRow.Children.Add(thisBut);
            });
            Content = thisGrid;
        }
        private Button GetButton(DetectiveInfo thisDet)
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
            output.SetName(nameof(ClueBoardGameMainViewModel.FillInClue));
            GamePackageViewModelBinder.ManuelElements.Add(output); //try this way.
            output.CommandParameter = thisDet;
            return output;
        }

    }
}