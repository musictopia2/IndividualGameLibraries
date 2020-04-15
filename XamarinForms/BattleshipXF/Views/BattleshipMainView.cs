using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BattleshipCP.Logic;
using BattleshipCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace BattleshipXF.Views
{
    public class BattleshipMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageResolver _resolver;
        private readonly GameBoardXF _boardUI;
        private readonly ShipControlCP? _ship1;
        private readonly ShipControlXF _shipUI;
        private readonly GameBoardCP? _gameBoard1;
        public BattleshipMainView(IEventAggregator aggregator,
            IGamePackageResolver resolver
            )
        {

            _aggregator = aggregator;
            _resolver = resolver;
            _aggregator.Subscribe(this);



            _boardUI = new GameBoardXF();
            Grid mainGrid = new Grid();
            AddAutoColumns(mainGrid, 1);
            AddLeftOverColumn(mainGrid!, 1); // try as leftovers
            mainGrid!.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(mainGrid, _boardUI, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(mainGrid, otherStack, 0, 1);

            _shipUI = new ShipControlXF();
            otherStack.Children.Add(_shipUI);
            StackLayout tempStack = new StackLayout();
            tempStack.Margin = new Thickness(0, 5, 0, 0);
            var thisBind = new Binding(nameof(BattleshipMainViewModel.ShipDirectionsVisible));
            tempStack.SetBinding(StackLayout.IsVisibleProperty, thisBind);
            var firstBut = GetGamingButton("Horizontal", nameof(BattleshipMainViewModel.ShipDirection));
            firstBut.CommandParameter = true;
            thisBind = new Binding(nameof(BattleshipMainViewModel.ShipsHorizontal));
            IValueConverter thisConv;
            thisConv = new DirectionConverter();
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = true; // this one is true for comparison
            firstBut.SetBinding(BackgroundColorProperty, thisBind);
            tempStack.Children.Add(firstBut);
            firstBut = GetGamingButton("Vertical", nameof(BattleshipMainViewModel.ShipDirection));
            firstBut.CommandParameter = false;
            thisBind = new Binding(nameof(BattleshipMainViewModel.ShipsHorizontal));
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = false;
            firstBut.SetBinding(BackgroundColorProperty, thisBind);
            tempStack.Children.Add(firstBut);
            otherStack.Children.Add(tempStack); // i think
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(BattleshipMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(BattleshipMainViewModel.Status));
            otherStack.Children.Add(firstInfo.GetContent);

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _gameBoard1 = _resolver.Resolve<GameBoardCP>();
            _ship1 = _resolver.Resolve<ShipControlCP>();
            _gameBoard1.SpaceSize = 80;
            _boardUI!.CreateHeadersColumnsRows(_gameBoard1.RowList!.Values.ToCustomBasicList(), _gameBoard1.ColumnList!.Values.ToCustomBasicList());
            _boardUI.CreateControls(_gameBoard1.HumanList!);
            //maybe can have here.

            Content = mainGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            _shipUI!.LoadShips(_ship1!.ShipList.Values.ToCustomBasicList(), _model!);
            return this.RefreshBindingsAsync(_aggregator);
        }

        private BattleshipMainViewModel? _model;

        Task IUIView.TryActivateAsync()
        {
            _model = (BattleshipMainViewModel)BindingContext;
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
