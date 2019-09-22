using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ChineseCheckersCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace ChineseCheckersWPF
{
    public class GamePage : MultiPlayerWindow<ChineseCheckersViewModel, ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _thisBoard.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;

        private EnumPickerWPF<MarblePiecesCP<EnumColorList>, MarblePiecesWPF<EnumColorList>,
            EnumColorList, ColorListChooser<EnumColorList>>? _thisColor;
        private readonly CompleteGameBoard<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoard<GameBoardGraphicsCP>();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<MarblePiecesCP<EnumColorList>, MarblePiecesWPF<EnumColorList>,
            EnumColorList, ColorListChooser<EnumColorList>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(ChineseCheckersViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ChineseCheckersViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(ChineseCheckersViewModel.ColorVisible));
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
            _thisBoard.Margin = new Thickness(-10, 0, 0, 0);
            _thisBoard.HorizontalAlignment = HorizontalAlignment.Left;
            _thisBoard.VerticalAlignment = VerticalAlignment.Top;
            Grid tempGrid = new Grid();
            AddPixelColumn(tempGrid, 300);
            AddAutoColumns(tempGrid, 1);
            var endButton = GetGamingButton("End Turn", nameof(ChineseCheckersViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ChineseCheckersViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ChineseCheckersViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(ChineseCheckersViewModel.Status));
            StackPanel tempStack = new StackPanel();
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(firstInfo.GetContent);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            AddControlToGrid(tempGrid, _thisBoard, 0, 1);
            thisStack.Children.Add(tempGrid);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(tempStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<ChineseCheckersPlayerItem, ChineseCheckersSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<ChineseCheckersViewModel>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "main");
        }
    }
}