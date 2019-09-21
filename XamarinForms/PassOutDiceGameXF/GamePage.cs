using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using AndyCristinaGamePackageXF;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using PassOutDiceGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using CommonBasicStandardLibraries.Exceptions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using BasicGameFramework.BasicGameDataClasses;
namespace PassOutDiceGameXF
{
    public class GamePage : MultiPlayerPage<PassOutDiceGameViewModel, PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        readonly GameBoardXF _thisBoard = new GameBoardXF();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColorChoice>, CheckerChooserXF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(PassOutDiceGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(PassOutDiceGameViewModel.Instructions));
            Binding thisBind = new Binding(nameof(PassOutDiceGameViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            Grid grid = new Grid();
            AddLeftOverColumn(grid, 1);
            AddAutoColumns(grid, 1);
            AddControlToGrid(grid, _thisBoard, 0, 1);
            AddControlToGrid(grid, thisStack, 0, 0);
            MainGrid!.Children.Add(grid);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _diceControl = new DiceListControlXF<SimpleDice>();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            var endButton = GetGamingButton("End Turn", nameof(PassOutDiceGameViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(PassOutDiceGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(PassOutDiceGameViewModel.Status));
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(endButton);
            finalStack.Children.Add(_diceControl);
            AddRestoreCommand(finalStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportion>(""); //here too.
            OurContainer!.RegisterType<BasicGameLoader<PassOutDiceGamePlayerItem, PassOutDiceGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<PassOutDiceGameViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, PassOutDiceGamePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
    }
    public class CustomProportion : IProportionBoard
    {
        float IProportionBoard.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 2.4f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.7f;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 1.1f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}