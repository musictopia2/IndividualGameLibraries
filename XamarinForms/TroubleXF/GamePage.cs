using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TroubleCP;
using Xamarin.Forms;
namespace TroubleXF
{
    public class GamePage : MultiPlayerPage<TroubleViewModel, TroublePlayerItem, TroubleSaveInfo>, IFirstPaint
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
        private readonly Grid _tempGrid = new Grid();
        private readonly StackLayout _tempStack = new StackLayout();
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<MarblePiecesCP<EnumColorChoice>, MarblePiecesXF<EnumColorChoice>, EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(TroubleViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(TroubleViewModel.Instructions));
            Binding thisBind = new Binding(nameof(TroubleViewModel.ColorVisible));
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
            _tempGrid.Margin = new Thickness(5, 5, 5, 5);
            _diceControl = new DiceListControlXF<SimpleDice>();
            _diceControl.Margin = new Thickness(10, 0, 0, 0);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TroubleViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(TroubleViewModel.Instructions));
            firstInfo.AddRow("Temp Space", nameof(TroubleViewModel.TempSpace));
            //firstInfo.AddRow("Status", nameof(TroubleViewModel.Status));
            thisStack.Children.Add(_tempGrid);
            thisStack.Children.Add(firstInfo.GetContent);
            _tempStack.Children.Add(_diceControl);
            _tempStack.InputTransparent = true; //maybe this will be okay.
            _tempGrid.Children.Add(_thisBoard);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<TroublePlayerItem, TroubleSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<TroubleViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, TroublePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterType<BoardPosition>(); //to register the gameboard position.
        }
        void IFirstPaint.PositionDice()
        {
            GameBoardGraphicsCP thisG = OurContainer!.Resolve<GameBoardGraphicsCP>();
            var thisPos = thisG.RecommendedPointForDice;
            _tempStack.Margin = new Thickness(thisPos.X, thisPos.Y, 0, 0);
            _tempGrid.Children.Add(_tempStack); //hopefully this simple.
        }
    }
}