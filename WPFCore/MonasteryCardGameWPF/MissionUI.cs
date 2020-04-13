using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MonasteryCardGameCP.Data;
using MonasteryCardGameCP.ViewModels;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace MonasteryCardGameWPF
{
    public class MissionUI : BaseFrameWPF
    {
        private CustomBasicCollection<MissionList>? _thisList;
        private Grid? _thisGrid;
        private IUIView? _view;
        private IEventAggregator? _aggregator;
        public async Task InitAsync(MonasteryCardGameVMData model, IUIView view, IEventAggregator aggregator)
        {
            _view = view;
            _aggregator = aggregator;
            Text = "Mission List";
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
            await PopulateListAsync();
            _thisList.CollectionChanged += ListChange;
            Content = mainGrid;
        }
        private async void ListChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                await PopulateListAsync(); //hopefully this is all i need.
        }
        private Button GetDescButton()
        {
            var thisBut = GetGamingButton("Mission Description", nameof(MonasteryCardGameMainViewModel.MissionDetailAsync));
            GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            return thisBut;
        }
        private async Task PopulateListAsync()
        {
            GamePackageViewModelBinder.ManuelElements.Clear();
            _thisGrid!.Children.Clear();
            if (_thisList!.Count == 0)
            {
                var tempButton = GetDescButton();
                AddControlToGrid(_thisGrid, tempButton, 0, 0);
                await _view!.RefreshBindingsAsync(_aggregator!);
                return;
            }
            int column = 0;
            int row = 0;
            _thisList.ForEach(thisMission =>
            {
                var thisButton = GetGamingButton(thisMission.Description, nameof(MonasteryCardGameMainViewModel.SelectPossibleMission));
                GamePackageViewModelBinder.ManuelElements.Add(thisButton);
                thisButton.CommandParameter = thisMission;
                ButtonOptionConverter thisConvert = new ButtonOptionConverter();
                Binding thisBind = new Binding(nameof(MonasteryCardGameMainViewModel.MissionChosen));
                thisBind.Converter = thisConvert;
                thisBind.ConverterParameter = thisMission.Description;
                thisButton.SetBinding(BackgroundProperty, thisBind);
                AddControlToGrid(_thisGrid, thisButton, row, column);
                column++;
                if (column > 1)
                {
                    column = 0;
                    row++;
                }
            });
            row++;
            StackPanel finalStack = new StackPanel();
            finalStack.Orientation = Orientation.Horizontal;
            AddControlToGrid(_thisGrid, finalStack, row, 0);
            Grid.SetColumnSpan(finalStack, 3);
            var finalButton = GetGamingButton("Complete Mission", nameof(MonasteryCardGameMainViewModel.CompleteChosenMissionAsync));
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
                return Brushes.LimeGreen;
            return Brushes.Aqua;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}