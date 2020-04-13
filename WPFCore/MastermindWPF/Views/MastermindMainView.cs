using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using MastermindCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using MastermindCP.ViewModels;
using System.Windows.Media;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;

namespace MastermindWPF.Views
{
    public class MastermindMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly EnumPickerWPF<CirclePieceCP<EnumColorPossibilities>, CirclePieceWPF<EnumColorPossibilities>, EnumColorPossibilities> _colors;
        public MastermindMainView(IEventAggregator aggregator, LevelClass level)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(MastermindMainViewModel.ActiveViewModel)
            };
            stack.Children.Add(parent);
            Grid grid = new Grid();
            grid.Background = Brushes.Brown;
            grid.Width = 200;
            EnumPickerWPF<CirclePieceCP<EnumColorPossibilities>, CirclePieceWPF<EnumColorPossibilities>, EnumColorPossibilities> colors = new EnumPickerWPF<CirclePieceCP<EnumColorPossibilities>, CirclePieceWPF<EnumColorPossibilities>, EnumColorPossibilities>();
            colors.Rows = 10; //try this way now.
            colors.HorizontalAlignment = HorizontalAlignment.Center;
            _colors = colors;
            var acceptBut = GetGamingButton("Accept", nameof(MastermindMainViewModel.AcceptAsync));
            var giveUpBut = GetGamingButton("Give Up", nameof(MastermindMainViewModel.GiveUpAsync));
            var label = GetDefaultLabel();
            label.FontSize = 20;
            label.Text = $"Level Chosen:  {level.LevelChosen}";
            grid.Children.Add(colors);
            grid.Margin = new Thickness(10);
            stack.Children.Add(grid);
            StackPanel other = new StackPanel();
            other.Children.Add(acceptBut);
            other.Children.Add(giveUpBut);
            other.Children.Add(label);
            stack.Children.Add(other);
            Content = stack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            MastermindMainViewModel model = (MastermindMainViewModel)DataContext;
            _colors.LoadLists(model.Color1);
            return this.RefreshBindingsAsync(_aggregator);
        }

        Task IUIView.TryActivateAsync()
        {

            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
