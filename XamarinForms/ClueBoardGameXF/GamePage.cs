using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameBoards;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
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
using ClueBoardGameCP;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace ClueBoardGameXF
{
    public class GamePage : MultiPlayerPage<ClueBoardGameViewModel, ClueBoardGamePlayerItem, ClueBoardGameSaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _diceControl!.LoadDiceViewModel(ThisMod.ThisCup!);
            _thisBoard.LoadBoard();
            _thisPile!.Init(ThisMod.ThisPile!, "");
            _thisHand!.LoadList(ThisMod.HandList!, "");
            _mainGame!.PopulateDetectiveNoteBook(); //i think.
            _thisDet!.LoadControls(_thisHand);
            _thisPre!.LoadControls(_predictionButton!, _accusationButton!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            _thisHand!.UpdateList(ThisMod!.HandList!);
            _thisPile!.UpdatePile(ThisMod.ThisPile!);
            return Task.CompletedTask;
        }
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        private readonly CompleteGameBoardXF<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        private PawnPiecesXF<EnumColorChoice>? _thisPiece;
        private BasePileXF<CardInfo, CardCP, CardXF>? _thisPile;
        private BaseHandXF<CardInfo, CardCP, CardXF>? _thisHand;
        private DetectiveNotebookXF? _thisDet;
        private PredictionAccusationXF? _thisPre;
        private ClueBoardGameMainGameClass? _mainGame;
        private Button? _predictionButton;
        private Button? _accusationButton;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(ClueBoardGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ClueBoardGameViewModel.Instructions));
            Binding thisBind = new Binding(nameof(ClueBoardGameViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<ClueBoardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            _thisPiece = new PawnPiecesXF<EnumColorChoice>();
            _thisPile = new BasePileXF<CardInfo, CardCP, CardXF>();
            _thisHand = new BaseHandXF<CardInfo, CardCP, CardXF>();
            _thisDet = new DetectiveNotebookXF();
            _thisPre = new PredictionAccusationXF();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout firstStack = new StackLayout();
            firstStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(_thisBoard);
            _thisPiece = new PawnPiecesXF<EnumColorChoice>();
            _thisPiece.HeightRequest = 80;
            _thisPiece.WidthRequest = 80;
            _thisPiece.Init(); //hopefully enough (?)
            firstStack.Children.Add(_thisPiece);
            thisStack.Children.Add(firstStack);
            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            AddAutoColumns(tempGrid, 1);
            thisStack.Children.Add(tempGrid);
            AddControlToGrid(tempGrid, _thisPre, 0, 0);
            _thisBoard.HorizontalOptions = LayoutOptions.Start;
            _thisBoard.VerticalOptions = LayoutOptions.Start;
            _diceControl = new DiceListControlXF<SimpleDice>();
            Label thisLabel = new Label();
            thisLabel.FontSize = 30;
            thisLabel.TextColor = Color.White;
            thisLabel.FontAttributes = FontAttributes.Bold;
            thisLabel.BindingContext = ThisMod; //just in case.
            thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(ClueBoardGameViewModel.LeftToMove)));
            StackLayout tempStack = new StackLayout();
            var thisRoll = GetSmallerButton("Roll Dice", nameof(ClueBoardGameViewModel.RollCommand));
            tempStack.Children.Add(thisLabel);
            tempStack.Children.Add(thisRoll);
            var endButton = GetSmallerButton("End Turn", nameof(ClueBoardGameViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(_thisPile);
            tempStack.Children.Add(_diceControl);
            AddControlToGrid(tempGrid, _thisDet, 0, 1);
            AddControlToGrid(tempGrid, tempStack, 0, 2);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ClueBoardGameViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ClueBoardGameViewModel.Instructions));
            thisStack.Children.Add(firstInfo.GetContent);
            _predictionButton = GetSmallerButton("Predict", nameof(ClueBoardGameViewModel.MakePredictionCommand));
            _accusationButton = GetSmallerButton("Accusation", nameof(ClueBoardGameViewModel.MakeAccusationCommand));
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(""); //here too.
            OurContainer!.RegisterType<BasicGameLoader<ClueBoardGamePlayerItem, ClueBoardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ClueBoardGameViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, CardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ClueBoardGamePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
        public void Handle(NewTurnEventModel message)
        {
            _thisPiece!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
        }
    }
}