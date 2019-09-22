using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using SequenceDiceCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace SequenceDiceXF
{
    public class GamePage : MultiPlayerPage<SequenceDiceViewModel, SequenceDicePlayerItem, SequenceDiceSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        private GameBoardXF? _thisBoard;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(SequenceDiceViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(SequenceDiceViewModel.Instructions));
            Binding thisBind = new Binding(nameof(SequenceDiceViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _thisBoard = new GameBoardXF();
            _thisBoard.Margin = new Thickness(2, 2, 2, 2);
            thisStack.Children.Add(_thisBoard);
            _diceControl = new DiceListControlXF<SimpleDice>();
            var thisRoll = GetGamingButton("Roll Dice", nameof(SequenceDiceViewModel.RollCommand));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            thisStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SequenceDiceViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SequenceDiceViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SequenceDiceViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            MainGrid!.Children.Add(thisStack);// this is for sure needed everytime.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<SequenceDicePlayerItem, SequenceDiceSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<SequenceDiceViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, SequenceDicePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();

        }
    }
}