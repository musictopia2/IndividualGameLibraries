using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ConnectFourCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ConnectFourWPF
{
    public class GamePage : MultiPlayerWindow<ConnectFourViewModel, ConnectFourPlayerItem, ConnectFourSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            ConnectFourSaveInfo thisSave = OurContainer!.Resolve<ConnectFourSaveInfo>();
            _thisBoard!.CreateControls(thisSave.GameBoard);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ConnectFourSaveInfo thisSave = OurContainer!.Resolve<ConnectFourSaveInfo>();
            _thisBoard!.UpdateControls(thisSave.GameBoard);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;
        private EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        GameBoardWPF? _thisBoard;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(ConnectFourViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ConnectFourViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(ConnectFourViewModel.ColorVisible));
            _chooseColorStack.SetBinding(VisibilityProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard = new GameBoardWPF();
            thisStack.Children.Add(_thisBoard);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ConnectFourViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConnectFourViewModel.Status)); // this may have to show the status to begin with (?)
            thisStack.Children.Add(firstInfo.GetContent);
            AddRestoreCommand(thisStack);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ConnectFourPlayerItem, ConnectFourSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<ConnectFourViewModel>();
        }
    }
}