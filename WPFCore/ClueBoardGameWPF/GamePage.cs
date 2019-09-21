using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameBoards;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
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
using ClueBoardGameCP;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ClueBoardGameWPF
{
    public class GamePage : MultiPlayerWindow<ClueBoardGameViewModel, ClueBoardGamePlayerItem, ClueBoardGameSaveInfo>, IHandle<NewTurnEventModel>
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
            _thisPile!.Init(ThisMod.ThisPile!, "");
            _thisHand!.LoadList(ThisMod.HandList!, "");
            _mainGame!.PopulateDetectiveNoteBook(); //i think.
            _thisDet!.LoadControls();
            _thisPre!.LoadControls();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            _thisHand!.UpdateList(ThisMod!.HandList!);
            _thisPile!.UpdatePile(ThisMod.ThisPile!);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;
        private EnumPickerWPF<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        private readonly CompleteGameBoard<GameBoardGraphicsCP> _thisBoard = new CompleteGameBoard<GameBoardGraphicsCP>();
        private PawnPiecesWPF<EnumColorChoice>? _thisPiece;
        private BasePileWPF<CardInfo, CardCP, CardWPF>? _thisPile;
        private BaseHandWPF<CardInfo, CardCP, CardWPF>? _thisHand;
        private DetectiveNotebookWPF? _thisDet;
        private PredictionAccusationWPF? _thisPre;
        private ClueBoardGameMainGameClass? _mainGame;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(ClueBoardGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(ClueBoardGameViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(ClueBoardGameViewModel.ColorVisible));
            _chooseColorStack.SetBinding(VisibilityProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<ClueBoardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            _thisPiece = new PawnPiecesWPF<EnumColorChoice>();
            _thisPile = new BasePileWPF<CardInfo, CardCP, CardWPF>();
            _thisHand = new BaseHandWPF<CardInfo, CardCP, CardWPF>();
            _thisDet = new DetectiveNotebookWPF();
            _thisPre = new PredictionAccusationWPF();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel mainStack = new StackPanel();
            mainStack.Orientation = Orientation.Horizontal;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ClueBoardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ClueBoardGameViewModel.Status));
            StackPanel firstRowStack = new StackPanel();
            firstRowStack.Children.Add(_thisBoard);
            _thisPiece = new PawnPiecesWPF<EnumColorChoice>();
            _thisPiece.Height = 60;
            _thisPiece.Width = 60;
            _thisPiece.Init(); //hopefully enough (?)
            otherStack.Children.Add(_thisPiece);
            otherStack.Children.Add(firstInfo.GetContent);
            firstRowStack.Children.Add(otherStack);
            mainStack.Children.Add(firstRowStack);
            StackPanel secondRowStack = new StackPanel();
            mainStack.Children.Add(secondRowStack);
            StackPanel thirdRowStack = new StackPanel();
            mainStack.Children.Add(thirdRowStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_thisHand);
            otherStack.Children.Add(_thisPile);
            secondRowStack.Children.Add(otherStack);
            _thisPre.Margin = new Thickness(0, 0, 0, 30);
            secondRowStack.Children.Add(_thisPre);
            secondRowStack.Children.Add(_thisDet);
            AddVerticalLabelGroup("Instructions", nameof(ClueBoardGameViewModel.Instructions), thirdRowStack);
            var thisRoll = GetGamingButton("Roll Dice", nameof(ClueBoardGameViewModel.RollCommand));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            TextBlock thisLabel = new TextBlock();
            thisLabel.FontSize = 100;
            thisLabel.Foreground = Brushes.White;
            thisLabel.FontWeight = FontWeights.Bold;
            thisLabel.DataContext = ThisMod; //just in case.
            thisLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(ClueBoardGameViewModel.LeftToMove)));
            otherStack.Children.Add(thisLabel);
            thirdRowStack.Children.Add(otherStack);
            thirdRowStack.Children.Add(thisRoll);
            var thisButton = GetGamingButton("Predict", nameof(ClueBoardGameViewModel.MakePredictionCommand));
            thirdRowStack.Children.Add(thisButton);
            thisButton = GetGamingButton("Accusation", nameof(ClueBoardGameViewModel.MakeAccusationCommand));
            thirdRowStack.Children.Add(thisButton);
            var endButton = GetGamingButton("End Turn", nameof(ClueBoardGameViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            thirdRowStack.Children.Add(endButton);
            var nextInfo = new SimpleLabelGrid();
            nextInfo.AddRow("Room", nameof(ClueBoardGameViewModel.CurrentRoomName));
            nextInfo.AddRow("Character", nameof(ClueBoardGameViewModel.CurrentCharacterName));
            nextInfo.AddRow("Weapon", nameof(ClueBoardGameViewModel.CurrentWeaponName));
            thirdRowStack.Children.Add(nextInfo.GetContent);
            thisStack.Children.Add(mainStack);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(""); //here too.
            OurContainer!.RegisterType<BasicGameLoader<ClueBoardGamePlayerItem, ClueBoardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<ClueBoardGameViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ClueBoardGamePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
        public void Handle(NewTurnEventModel message)
        {
            _thisPiece!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
        }
    }
}