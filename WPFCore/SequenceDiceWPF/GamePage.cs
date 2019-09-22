using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
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
using SequenceDiceCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace SequenceDiceWPF
{
    public class GamePage : MultiPlayerWindow<SequenceDiceViewModel, SequenceDicePlayerItem, SequenceDiceSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _diceControl!.LoadDiceViewModel(ThisMod.ThisCup!);
            SequenceDiceMainGameClass mainGame = OurContainer!.Resolve<SequenceDiceMainGameClass>();
            _thisBoard!.CreateControls(mainGame.SaveRoot!.GameBoard);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            SequenceDiceMainGameClass mainGame = OurContainer!.Resolve<SequenceDiceMainGameClass>();
            _thisBoard!.UpdateControls(mainGame.SaveRoot!.GameBoard);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;

        private EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        private GameBoardWPF? _thisBoard;

        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserWPF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(SequenceDiceViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(SequenceDiceViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(SequenceDiceViewModel.ColorVisible));
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
            _thisBoard = new GameBoardWPF();
            _thisBoard.Margin = new Thickness(3, 3, 3, 3);
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(tempStack);
            tempStack.Children.Add(_thisBoard);
            _diceControl = new DiceListControlWPF<SimpleDice>();
            var thisRoll = GetGamingButton("Roll Dice", nameof(SequenceDiceViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SequenceDiceViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SequenceDiceViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SequenceDiceViewModel.Status));
            tempStack.Children.Add(firstInfo.GetContent);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            MainGrid!.Children.Add(thisStack);// this is for sure needed everytime.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<SequenceDicePlayerItem, SequenceDiceSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<SequenceDiceViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, SequenceDicePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}