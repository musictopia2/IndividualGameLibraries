using ClueBoardGameCP;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ClueBoardGameXF
{
    public class PredictionAccusationXF : ContentView
    {
        public void LoadControls(Button predictionButton, Button accusationButton)
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
            StackLayout finalStack = new StackLayout();
            AddControlToGrid(thisGrid, finalStack, 1, 1);
            Grid.SetColumnSpan(finalStack, 2);
            finalStack.Children.Add(predictionButton);
            finalStack.Children.Add(accusationButton);
            GlobalClass thisG = Resolve<GlobalClass>();
            var thisList = thisG.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsRoom).ToCustomBasicList();
            Button thisBut;
            var thisLabel = GetLabel(EnumCardType.IsRoom);
            firstRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                firstRow.Children.Add(thisBut);
            });
            thisList = thisG.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsCharacter).ToCustomBasicList();
            thisLabel = GetLabel(EnumCardType.IsCharacter);
            secondRow.Children.Add(thisLabel);
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                secondRow.Children.Add(thisBut);
            });
            thisList = thisG.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsWeapon).ToCustomBasicList();
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
            Binding secondBind;
            switch (thisDet.Category)
            {
                case EnumCardType.IsRoom:
                    firstBind = new Binding(nameof(ClueBoardGameViewModel.CurrentRoomName));
                    secondBind = new Binding(nameof(ClueBoardGameViewModel.CurrentRoomCommand));
                    break;
                case EnumCardType.IsWeapon:
                    firstBind = new Binding(nameof(ClueBoardGameViewModel.CurrentWeaponName));
                    secondBind = new Binding(nameof(ClueBoardGameViewModel.CurrentWeaponCommand));
                    break;
                case EnumCardType.IsCharacter:
                    firstBind = new Binding(nameof(ClueBoardGameViewModel.CurrentCharacterName));
                    secondBind = new Binding(nameof(ClueBoardGameViewModel.CurrentCharacterCommand));
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
            output.CommandParameter = thisDet.Name;
            firstBind.Converter = thisC;
            firstBind.ConverterParameter = thisDet.Name;
            output.SetBinding(BackgroundColorProperty, firstBind);
            output.SetBinding(Button.CommandProperty, secondBind);
            return output;
        }
    }
}