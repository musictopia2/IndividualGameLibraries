using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.CollectionClasses;
using MonasteryCardGameCP;
using System;
using System.Collections.Specialized;
using System.Globalization;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace MonasteryCardGameXF
{
    public class MissionUI : BaseFrameXF
    {
        private CustomBasicCollection<MissionList>? _thisList;
        private Grid? _thisGrid;
        public void Init(MonasteryCardGameViewModel thisMod)
        {
            Text = "Mission List";
            _thisList = thisMod.CompleteMissions;
            Grid mainGrid = new Grid();
            var thisRect = ThisFrame.GetControlArea();
            _thisGrid = new Grid();
            _thisGrid.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            AddAutoRows(_thisGrid, 6);
            AddLeftOverColumn(_thisGrid, 50);
            AddLeftOverColumn(_thisGrid, 50);
            mainGrid.Children.Add(ThisDraw);
            mainGrid.Children.Add(_thisGrid);
            PopulateList();
            _thisList.CollectionChanged += ListChange;
            Content = mainGrid;
        }
        private void ListChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                PopulateList(); //hopefully this is all i need.
        }
        private Button GetDescButton()
        {
            var thisBut = GetSmallerButton("Description", nameof(MonasteryCardGameViewModel.MissionDetailCommand));
            return thisBut;
        }
        private void PopulateList()
        {
            _thisGrid!.Children.Clear();
            if (_thisList!.Count == 0)
            {
                var tempButton = GetDescButton();
                AddControlToGrid(_thisGrid, tempButton, 0, 0);
                return;
            }
            int column = 0;
            int row = 0;
            _thisList.ForEach(thisMission =>
            {
                var thisButton = GetSmallerButton(thisMission.Description, nameof(MonasteryCardGameViewModel.SelectPossibleMissionCommand));
                thisButton.CommandParameter = thisMission;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    thisButton.HeightRequest = 60;
                else
                    thisButton.HeightRequest = 80;
                ButtonOptionConverter thisConvert = new ButtonOptionConverter();
                Binding thisBind = new Binding(nameof(MonasteryCardGameViewModel.MissionChosen));
                thisBind.Converter = thisConvert;
                thisBind.ConverterParameter = thisMission.Description;
                thisButton.SetBinding(BackgroundColorProperty, thisBind);
                AddControlToGrid(_thisGrid, thisButton, row, column);
                column++;
                if (column > 1)
                {
                    column = 0;
                    row++;
                }
            });
            row++;
            StackLayout finalStack = new StackLayout();
            finalStack.Orientation = StackOrientation.Horizontal;
            AddControlToGrid(_thisGrid, finalStack, row, 0);
            Grid.SetColumnSpan(finalStack, 3);
            var finalButton = GetSmallerButton("Complete", nameof(MonasteryCardGameViewModel.CompleteChosenMissionCommand));
            finalStack.Children.Add(finalButton);
            finalButton = GetDescButton();
            finalStack.Children.Add(finalButton);
        }
    }
    public class ButtonOptionConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == parameter)
                return Color.LimeGreen;
            return Color.Aqua;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}