using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.BasicWindows.Misc;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BattleshipCP;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace BattleshipWPF
{
    public class GamePage : MultiPlayerWindow<BattleshipViewModel, BattleshipPlayerItem, BattleshipSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _gameBoard1 = OurContainer!.Resolve<GameBoardCP>();
            _ships = OurContainer.Resolve<ShipControlCP>();
            _gameBoard1.SpaceSize = 80;
            _thisBoard!.CreateHeadersColumnsRows(_gameBoard1.RowList!.Values.ToCustomBasicList(), _gameBoard1.ColumnList!.Values.ToCustomBasicList());
            _thisBoard.CreateControls(_gameBoard1.HumanList!);
            _thisShip!.LoadShips(_ships.ShipList.Values.ToCustomBasicList());
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _thisBoard!.UpdateControls(_gameBoard1!.HumanList!);
            _thisShip!.UpdateShips(_ships!.ShipList.Values.ToCustomBasicList());
            return Task.CompletedTask;
        }
        private GameBoardWPF? _thisBoard;
        private ShipControlCP? _ships;
        private ShipControlWPF? _thisShip;
        private GameBoardCP? _gameBoard1;
        protected async override void AfterGameButton()
        {
            BasicSetUp();
            AddAutoColumns(MainGrid!, 1);
            AddLeftOverColumn(MainGrid!, 1); // try as leftovers
            MainGrid!.Margin = new Thickness(5, 5, 5, 5);
            _thisBoard = new GameBoardWPF();
            AddControlToGrid(MainGrid, _thisBoard, 0, 0);
            StackPanel otherStack = new StackPanel();
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(MainGrid, otherStack, 0, 1);
            otherStack.Children.Add(GameButton);
            _thisShip = new ShipControlWPF();
            otherStack.Children.Add(_thisShip);
            StackPanel tempStack = new StackPanel();
            tempStack.Margin = new Thickness(0, 5, 0, 0);
            var thisBind = GetVisibleBinding(nameof(BattleshipViewModel.ShipDirectionsVisible));
            tempStack.SetBinding(StackPanel.VisibilityProperty, thisBind);
            var firstBut = GetGamingButton("Horizontal (F1)", nameof(BattleshipViewModel.ShipDirectionCommand));
            firstBut.CommandParameter = true;
            thisBind = new Binding(nameof(BattleshipViewModel.ShipsHorizontal));
            IValueConverter thisConv;
            thisConv = new DirectionConverter();
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = true; // this one is true for comparison
            WindowHelper.SetKeyBindings(Key.F1, nameof(BattleshipViewModel.ShipDirectionCommand), true);
            firstBut.SetBinding(BackgroundProperty, thisBind);
            tempStack.Children.Add(firstBut);
            firstBut = GetGamingButton("Vertical (F2)", nameof(BattleshipViewModel.ShipDirectionCommand));
            WindowHelper.SetKeyBindings(Key.F2, nameof(BattleshipViewModel.ShipDirectionCommand), false);
            firstBut.CommandParameter = false;
            thisBind = new Binding(nameof(BattleshipViewModel.ShipsHorizontal));
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = false;
            firstBut.SetBinding(BackgroundProperty, thisBind);
            tempStack.Children.Add(firstBut);
            otherStack.Children.Add(tempStack); // i think
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(BattleshipViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(BattleshipViewModel.Status));
            otherStack.Children.Add(firstInfo.GetContent);
            await FinishUpAsync(); //forgot this part.
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<BattleshipPlayerItem, BattleshipSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<BattleshipViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
        }
    }
}