using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BattleshipCP;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace BattleshipXF
{
    public class GamePage : MultiPlayerPage<BattleshipViewModel, BattleshipPlayerItem, BattleshipSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private ShipControlXF? _thisShip;
        private GameBoardCP? _gameBoard1;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            AddAutoColumns(MainGrid!, 1);
            AddLeftOverColumn(MainGrid!, 1); // try as leftovers
            MainGrid!.Margin = new Thickness(5, 5, 5, 5);
            _thisBoard = new GameBoardWPF();
            AddControlToGrid(MainGrid, _thisBoard, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(MainGrid, otherStack, 0, 1);
            otherStack.Children.Add(GameButton);
            _thisShip = new ShipControlXF();
            otherStack.Children.Add(_thisShip);
            StackLayout tempStack = new StackLayout();
            tempStack.Margin = new Thickness(0, 5, 0, 0);
            var thisBind = new Binding(nameof(BattleshipViewModel.ShipDirectionsVisible));
            tempStack.SetBinding(IsVisibleProperty, thisBind);
            var firstBut = GetGamingButton("Horizontal (F1)", nameof(BattleshipViewModel.ShipDirectionCommand));
            firstBut.CommandParameter = true;
            thisBind = new Binding(nameof(BattleshipViewModel.ShipsHorizontal));
            IValueConverter thisConv;
            thisConv = new DirectionConverter();
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = true; // this one is true for comparison
            firstBut.SetBinding(BackgroundColorProperty, thisBind);
            tempStack.Children.Add(firstBut);
            firstBut = GetGamingButton("Vertical (F2)", nameof(BattleshipViewModel.ShipDirectionCommand));
            firstBut.CommandParameter = false;
            thisBind = new Binding(nameof(BattleshipViewModel.ShipsHorizontal));
            thisBind.Converter = thisConv;
            thisBind.ConverterParameter = false;
            firstBut.SetBinding(BackgroundColorProperty, thisBind);
            tempStack.Children.Add(firstBut);
            otherStack.Children.Add(tempStack); // i think
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(BattleshipViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(BattleshipViewModel.Status));
            otherStack.Children.Add(firstInfo.GetContent);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<BattleshipPlayerItem, BattleshipSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<BattleshipViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
        }
    }
}