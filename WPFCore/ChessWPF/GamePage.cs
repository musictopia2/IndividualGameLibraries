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
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ChessCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ChessWPF
{
    public class GamePage : MultiPlayerWindow<ChessViewModel, ChessPlayerItem, ChessSaveInfo>
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

        private EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        private readonly GameBoardWPF _thisBoard = new GameBoardWPF();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(ChessViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ChessViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(ChessViewModel.ColorVisible));
            _chooseColorStack.SetBinding(VisibilityProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard.Margin = new Thickness(3, 3, 3, 3);
            var thisBind = GetVisibleBinding(nameof(ChessViewModel.MainOptionsVisible));
            thisStack.SetBinding(StackPanel.VisibilityProperty, thisBind);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ChessViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ChessViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(ChessViewModel.Instructions));
            // if there are any other data that goes here, will be added
            var thisContent = firstInfo.GetContent;
            MainGrid!.Children.Add(thisStack);
            // if there is no end turn, just comment it out.
            var endButton = GetGamingButton("End Turn", nameof(ChessViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_thisBoard);
            thisStack.Children.Add(otherStack);
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(endButton);
            var thisBut = GetGamingButton("Undo All Moves", nameof(ChessViewModel.UndoMovesCommand));
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Show Tie", nameof(ChessViewModel.TieCommand));
            tempStack.Children.Add(thisBut);
            StackPanel FinalStack = new StackPanel();
            FinalStack.Children.Add(tempStack);
            FinalStack.Children.Add(thisContent);
            otherStack.Children.Add(FinalStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<ChessPlayerItem, ChessSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<ChessViewModel>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "main");
        }
    }
}