using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using BattleshipCP.Logic;
using BattleshipCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace BattleshipWPF.Views
{
    public class BattleshipMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageResolver _resolver;
        private readonly GameBoardWPF _boardUI;
        private ShipControlCP? _ship1;
        private readonly ShipControlWPF _shipUI;
        private GameBoardCP? _gameBoard1;
        public BattleshipMainView(IEventAggregator aggregator,
            IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _resolver = resolver;
            _aggregator.Subscribe(this);

            _boardUI = new GameBoardWPF();
            Grid mainGrid = new Grid();
            AddAutoColumns(mainGrid, 1);
            AddLeftOverColumn(mainGrid!, 1); // try as leftovers
            mainGrid!.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(mainGrid, _boardUI, 0, 0);
            StackPanel otherStack = new StackPanel();
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(mainGrid, otherStack, 0, 1);

            _shipUI = new ShipControlWPF();
            otherStack.Children.Add(_shipUI);
            StackPanel tempStack = new StackPanel();
            tempStack.Margin = new Thickness(0, 5, 0, 0);
            var thisBind = GetVisibleBinding(nameof(BattleshipMainViewModel.ShipDirectionsVisible));
            tempStack.SetBinding(StackPanel.VisibilityProperty, thisBind);
            var firstBut = GetGamingButton("Horizontal", nameof(BattleshipMainViewModel.ShipDirection));
            firstBut.CommandParameter = true;
            thisBind = new Binding(nameof(BattleshipMainViewModel.ShipsHorizontal));
            IValueConverter thisConv;
            thisConv = new DirectionConverter();
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = true; // this one is true for comparison
            //WindowHelper.SetKeyBindings(Key.F1, nameof(BattleshipMainViewModel.ShipDirectionCommand), true);
            firstBut.SetBinding(BackgroundProperty, thisBind);
            tempStack.Children.Add(firstBut);
            firstBut = GetGamingButton("Vertical", nameof(BattleshipMainViewModel.ShipDirection));
            //WindowHelper.SetKeyBindings(Key.F2, nameof(BattleshipMainViewModel.ShipDirectionCommand), false);
            firstBut.CommandParameter = false;
            thisBind = new Binding(nameof(BattleshipMainViewModel.ShipsHorizontal));
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = false;
            firstBut.SetBinding(BackgroundProperty, thisBind);
            tempStack.Children.Add(firstBut);
            otherStack.Children.Add(tempStack); // i think
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(BattleshipMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(BattleshipMainViewModel.Status));
            otherStack.Children.Add(firstInfo.GetContent);
            //for now, no hotkeys.  otherwise, requires lots of rethinking on this one.
            Content = mainGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _gameBoard1 = _resolver.Resolve<GameBoardCP>();
            _ship1 = _resolver.Resolve<ShipControlCP>();
            _gameBoard1.SpaceSize = 80;
            _boardUI!.CreateHeadersColumnsRows(_gameBoard1.RowList!.Values.ToCustomBasicList(), _gameBoard1.ColumnList!.Values.ToCustomBasicList());
            _boardUI.CreateControls(_gameBoard1.HumanList!);
            _shipUI!.LoadShips(_ship1.ShipList.Values.ToCustomBasicList(), _model!);
            return this.RefreshBindingsAsync(_aggregator);
        }

        private BattleshipMainViewModel? _model;

        Task IUIView.TryActivateAsync()
        {
            _model = (BattleshipMainViewModel)DataContext;
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
