using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicDrawables.MiscClasses;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using SorryCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using BasicGameFramework.TestUtilities;

namespace SorryWPF
{
    public class GamePage : MultiPlayerWindow<SorryViewModel, SorryPlayerItem, SorrySaveInfo>
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

        private EnumPickerWPF<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        readonly CompleteGameBoard<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoard<GameBoardGraphicsCP>();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(SorryViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(SorryViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(SorryViewModel.ColorVisible));
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
            StackPanel tempStack = new StackPanel();
            Grid tempGrid = new Grid();
            AddAutoRows(tempGrid, 1);
            AddPixelColumn(tempGrid, 500);
            AddAutoColumns(tempGrid, 1);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            AddControlToGrid(tempGrid, _thisBoard, 0, 1);
            thisStack.Children.Add(tempGrid);
            var endButton = GetGamingButton("End Turn", nameof(SorryViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SorryViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SorryViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SorryViewModel.Status));
            tempStack.Children.Add(firstInfo.GetContent);
            AddVerticalLabelGroup("Card Details", nameof(SorryViewModel.CardDetails), tempStack);
            tempStack.Children.Add(endButton);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterTests()
        {
            ThisTest!.SaveOption = EnumTestSaveCategory.RestoreOnly;
            //ThisTest.DoubleCheck = true; //try this too.
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<SorryPlayerItem, SorrySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<SorryViewModel>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterType<DrawShuffleClass<CardInfo, SorryPlayerItem>>();
            OurContainer.RegisterType<GenericCardShuffler<CardInfo>>();
            OurContainer.RegisterSingleton<IDeckCount, DeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(""); //i think.
            OurContainer.RegisterSingleton<IDrawCardNM, MultiPlayerDraw>();
        }
    }
}