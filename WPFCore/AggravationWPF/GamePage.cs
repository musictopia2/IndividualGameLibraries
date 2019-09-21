using AggravationCP;
using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace AggravationWPF
{
    public class GamePage : MultiPlayerWindow<AggravationViewModel, AggravationPlayerItem, AggravationSaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _diceControl!.LoadDiceViewModel(ThisMod.ThisCup!);
            _thisBoard.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;

        private EnumPickerWPF<MarblePiecesCP<EnumColorChoice>, MarblePiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        readonly CompleteGameBoard<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoard<GameBoardGraphicsCP>();
        private MarblePiecesWPF<EnumColorChoice>? _ourPiece;
        private AggravationMainGameClass? _mainGame;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<MarblePiecesCP<EnumColorChoice>, MarblePiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(AggravationViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(AggravationViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(AggravationViewModel.ColorVisible));
            _chooseColorStack.SetBinding(VisibilityProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<AggravationMainGameClass>(); //hopefully don't need to subscibe again (?)
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _ourPiece = new MarblePiecesWPF<EnumColorChoice>();
            Grid tempGrid = new Grid();
            thisStack.Children.Add(tempGrid);
            _ourPiece.Width = 80;
            _ourPiece.Height = 80;
            _ourPiece.Init(); //i think.
            var thisRoll = GetGamingButton("Roll Dice", nameof(AggravationViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            var endButton = GetGamingButton("End Turn", nameof(AggravationViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(AggravationViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(AggravationViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(AggravationViewModel.Status));
            StackPanel firstStack = new StackPanel();
            firstStack.Margin = new Thickness(3, 3, 3, 3);
            firstStack.Children.Add(firstInfo.GetContent);
            firstStack.Children.Add(_ourPiece);
            otherStack.Children.Add(thisRoll);
            otherStack.Children.Add(_diceControl);
            firstStack.Children.Add(otherStack);
            tempGrid.Children.Add(firstStack);
            tempGrid.Children.Add(_thisBoard);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>(""); //here too.
            OurContainer!.RegisterType<BasicGameLoader<AggravationPlayerItem, AggravationSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<AggravationViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, AggravationPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
        public void Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
        }
    }
}