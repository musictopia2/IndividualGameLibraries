using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Specialized;
using System.Globalization;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using MonasteryCardGameCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using MonasteryCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using System.Threading.Tasks;
using BasicGameFrameworkLibrary.BasicEventModels;

namespace MonasteryCardGameXF
{
    public class MissionUI : BaseFrameXF
    {
        private CustomBasicCollection<MissionList>? _thisList;
        private Grid? _thisGrid;
        private IUIView? _view;
        private IEventAggregator? _aggregator;
        public async Task InitAsync(MonasteryCardGameVMData model, IUIView view, IEventAggregator aggregator)
        {
            Text = "Mission List";
            _view = view;
            _aggregator = aggregator;
            _thisList = model.CompleteMissions;
            Grid mainGrid = new Grid();
            var thisRect = ThisFrame.GetControlArea();
            _thisGrid = new Grid();
            _thisGrid.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 10, 3, 3);
            AddAutoRows(_thisGrid, 6);
            AddLeftOverColumn(_thisGrid, 50);
            AddLeftOverColumn(_thisGrid, 50);
            mainGrid.Children.Add(ThisDraw);
            mainGrid.Children.Add(_thisGrid);
            HorizontalOptions = LayoutOptions.Fill;
            await PopulateListAsync();
            _thisList.CollectionChanged += ListChange;
            Content = mainGrid;
        }


        public void Dispose()
        {
            _thisList!.CollectionChanged -= ListChange;
            _thisGrid!.Children.Clear();
        }

        private async void ListChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                await PopulateListAsync();
            }    
        }
        private Button GetDescButton()
        {
            var thisBut = GetSmallerButton("Mission Description", nameof(MonasteryCardGameMainViewModel.MissionDetailAsync));
            GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            return thisBut;
        }
        private async Task PopulateListAsync()
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
                var thisButton = GetSmallerButton(thisMission.Description, nameof(MonasteryCardGameMainViewModel.SelectPossibleMission));
                GamePackageViewModelBinder.ManuelElements.Add(thisButton);
                thisButton.CommandParameter = thisMission;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    thisButton.HeightRequest = 60;
                else
                    thisButton.HeightRequest = 80;
                ButtonOptionConverter thisConvert = new ButtonOptionConverter();
                Binding thisBind = new Binding(nameof(MonasteryCardGameMainViewModel.MissionChosen));
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
            var finalButton = GetSmallerButton("Complete", nameof(MonasteryCardGameMainViewModel.CompleteChosenMissionAsync));
            GamePackageViewModelBinder.ManuelElements.Add(finalButton);
            finalStack.Children.Add(finalButton);
            finalButton = GetDescButton();
            finalStack.Children.Add(finalButton);
            await _view!.RefreshBindingsAsync(_aggregator!);
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