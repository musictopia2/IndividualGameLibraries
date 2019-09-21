using AggravationCP;
using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using AndyCristinaGamePackageXF;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
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
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace AggravationXF
{
    public class GamePage : MultiPlayerPage<AggravationViewModel, AggravationPlayerItem, AggravationSaveInfo>, IHandle<NewTurnEventModel>
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
        private EnumPickerXF<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        readonly CompleteGameBoardXF<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        private MarblePiecesXF<EnumColorChoice>? _ourPiece;
        private AggravationMainGameClass? _mainGame;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(AggravationViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(AggravationViewModel.Instructions));
            Binding thisBind = new Binding(nameof(AggravationViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<AggravationMainGameClass>(); //hopefully don't need to subscibe again (?)
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _ourPiece = new MarblePiecesXF<EnumColorChoice>();
            _ourPiece.Margin = new Thickness(3, 3, 3, 3);
            _ourPiece.HorizontalOptions = LayoutOptions.Start;
            _ourPiece.VerticalOptions = LayoutOptions.Start;
            
            //Grid tempGrid = new Grid();
            //thisStack.Children.Add(tempGrid);
            // _thisBoard.InputTransparent = true;
            //_thisBoard.Margin = new Thickness(0, 40, 0, 0);
            //_ourPiece.Init(); //i think.
            Button thisRoll;
            if (ScreenUsed == EnumScreen.SmallPhone)
                thisRoll = GetSmallerButton("Roll Dice", nameof(AggravationViewModel.RollCommand));
            else
            {
                thisRoll = GetGamingButton("Roll Dice", nameof(AggravationViewModel.RollCommand));
                thisRoll.FontSize += 20;
            }
            thisRoll.Margin = new Thickness(3, 3, 0, 0);
            thisRoll.HorizontalOptions = LayoutOptions.Start;
            thisRoll.VerticalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal; //somehow when put together, does not let me click on anything else.
            otherStack.Spacing = 2;
            _diceControl = new DiceListControlXF<SimpleDice>();
            var endButton = GetGamingButton("End Turn", nameof(AggravationViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(AggravationViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(AggravationViewModel.Instructions));
            firstInfo.AddRow("Temp Space", nameof(AggravationViewModel.TempSpace));
            //firstInfo.AddRow("Status", nameof(AggravationViewModel.Status));
            otherStack.Children.Add(_ourPiece);
            otherStack.Children.Add(thisRoll);
            otherStack.Children.Add(_diceControl);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_thisBoard);
            thisStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<AggravationPlayerItem, AggravationSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<AggravationViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, AggravationPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterType<BoardPosition>(); //to register the gameboard position.
        }
        public void Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
        }
    }
}