using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace ClueBoardGameXF
{
    public class PredictionAccusationXF : ContentView
    {
        public void LoadControls(Button predictionButton, Button accusationButton, ClueBoardGameGameContainer gameContainer)
        {
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
            StackLayout finalStack = new StackLayout();
            AddControlToGrid(thisGrid, finalStack, 1, 1);
            Grid.SetColumnSpan(finalStack, 2);
            finalStack.Children.Add(predictionButton);
            finalStack.Children.Add(accusationButton);
            GamePackageViewModelBinder.ManuelElements.Add(predictionButton);
            GamePackageViewModelBinder.ManuelElements.Add(accusationButton);
            var thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsRoom).ToCustomBasicList();
            Button thisBut;
            var thisLabel = GetLabel(EnumCardType.IsRoom);
            firstRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                firstRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsCharacter).ToCustomBasicList();
            thisLabel = GetLabel(EnumCardType.IsCharacter);
            secondRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                secondRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsWeapon).ToCustomBasicList();
            thisLabel = GetLabel(EnumCardType.IsWeapon);
            thirdRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                thirdRow.Children.Add(thisBut);
            });
            Content = thisGrid;
        }
        private Label GetLabel(EnumCardType thisCat)
        {
            Label output = new Label();
            output.FontSize = 14;
            output.TextColor = Color.White;
            output.HorizontalOptions = LayoutOptions.Center;
            output.FontAttributes = FontAttributes.Bold;
            switch (thisCat)
            {
                case EnumCardType.IsRoom:
                    output.Text = "Rooms";
                    break;
                case EnumCardType.IsWeapon:
                    output.Text = "Weapons";
                    break;
                case EnumCardType.IsCharacter:
                    output.Text = "Characters";
                    break;
                default:
                    break;
            }
            return output;
        }
        private Button GetButton(DetectiveInfo thisDet)
        {
            Button output = new Button();
            output.Text = thisDet.Name;
            output.FontSize = 10;
            PredictionConverter thisC = new PredictionConverter();
            Binding firstBind;
            string secondName;
            switch (thisDet.Category)
            {
                case EnumCardType.IsRoom:
                    firstBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentRoomName));
                    secondName = nameof(ClueBoardGameMainViewModel.CurrentRoomClick);
                    break;
                case EnumCardType.IsWeapon:
                    firstBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentWeaponName));
                    secondName = nameof(ClueBoardGameMainViewModel.CurrentWeaponClick);
                    break;
                case EnumCardType.IsCharacter:
                    firstBind = new Binding(nameof(ClueBoardGameMainViewModel.CurrentCharacterName));
                    secondName = nameof(ClueBoardGameMainViewModel.CurrentCharacterClick);
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
            output.CommandParameter = thisDet.Name;
            firstBind.Converter = thisC;
            firstBind.ConverterParameter = thisDet.Name;
            output.SetBinding(BackgroundColorProperty, firstBind);
            output.SetName(secondName);
            GamePackageViewModelBinder.ManuelElements.Add(output);
            return output;
        }
    }
}