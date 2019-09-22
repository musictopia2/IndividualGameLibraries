using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
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
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using PaydayCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace PaydayWPF
{
    public class GamePage : MultiPlayerWindow<PaydayViewModel, PaydayPlayerItem, PaydaySaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            PaydaySaveInfo saveRoot = OurContainer!.Resolve<PaydaySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _diceControl!.LoadDiceViewModel(ThisMod.ThisCup!);
            _thisBoard.LoadBoard();
            _thisPopUp!.LoadLists(ThisMod.PopUpList!);
            _mailPile!.Init(ThisMod.MailPile!, "");
            _dealPile!.Init(ThisMod.DealPile!, "");
            _mailList!.LoadList(ThisMod.CurrentMailList!, "");
            _dealList!.LoadList(ThisMod.CurrentDealList!, "");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            PaydaySaveInfo saveRoot = OurContainer!.Resolve<PaydaySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _mailPile!.UpdatePile(ThisMod.MailPile!);
            _dealPile!.UpdatePile(ThisMod.DealPile!);
            _mailList!.UpdateList(ThisMod.CurrentMailList!);
            _dealList!.UpdateList(ThisMod.CurrentDealList!);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;

        private EnumPickerWPF<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        private PaydayMainGameClass? _mainGame;
        private readonly GameBoardWPF _thisBoard = new GameBoardWPF();
        private ScoreBoardWPF? _thisScore;
        private ListChooserWPF? _thisPopUp;
        private BasePileWPF<MailCard, CardGraphicsCP, MailCardWPF>? _mailPile;
        private BasePileWPF<DealCard, CardGraphicsCP, DealCardWPF>? _dealPile;
        private BaseHandWPF<MailCard, CardGraphicsCP, MailCardWPF>? _mailList;
        private BaseHandWPF<DealCard, CardGraphicsCP, DealCardWPF>? _dealList;
        private PawnPiecesWPF<EnumColorChoice>? _currentPiece;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<PawnPiecesCP<EnumColorChoice>, PawnPiecesWPF<EnumColorChoice>, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(PaydayViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(PaydayViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(PaydayViewModel.ColorVisible));
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
            _thisPopUp = new ListChooserWPF();
            _mailPile = new BasePileWPF<MailCard, CardGraphicsCP, MailCardWPF>();
            _dealPile = new BasePileWPF<DealCard, CardGraphicsCP, DealCardWPF>();
            _mailList = new BaseHandWPF<MailCard, CardGraphicsCP, MailCardWPF>();
            _dealList = new BaseHandWPF<DealCard, CardGraphicsCP, DealCardWPF>();
            _mailList.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<MailCard>.EnumHandList.Vertical;
            _dealList.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<DealCard>.EnumHandList.Vertical;
            _mailList.HorizontalAlignment = HorizontalAlignment.Left;
            _dealList.HorizontalAlignment = HorizontalAlignment.Left;
            _mailList.Height = 500;
            _dealList.Height = 500;
            var thisRoll = GetGamingButton("Roll Dice", nameof(PaydayViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            //Binding thisBind = new Binding(nameof(PaydayViewModel.GameStatus));
            //thisBind.Converter = new ShowRollConverter();
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(thisRoll);
            otherStack.Children.Add(_diceControl);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Money", true, nameof(PaydayPlayerItem.MoneyHas), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("Loans", true, nameof(PaydayPlayerItem.Loans), useCurrency: true, rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Main Turn", nameof(PaydayViewModel.NormalTurn));
            firstInfo.AddRow("Other Turn", nameof(PaydayViewModel.OtherLabel));
            firstInfo.AddRow("Progress", nameof(PaydayViewModel.MonthLabel));
            firstInfo.AddRow("Status", nameof(PaydayViewModel.Status));
            var firstContent = firstInfo.GetContent;
            StackPanel tempStack = new StackPanel();
            AddVerticalLabelGroup("Instructions", nameof(PaydayViewModel.Instructions), tempStack);
            ScrollViewer tempScroll = new ScrollViewer();
            tempScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tempScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            tempScroll.Content = tempStack;
            StackPanel firstStack = new StackPanel();
            firstStack.Orientation = Orientation.Horizontal;
            firstStack.Children.Add(_thisBoard);
            thisStack.Children.Add(firstStack);
            firstStack.Margin = new Thickness(3, 3, 3, 3);
            Grid rightGrid = new Grid();
            firstStack.Children.Add(rightGrid);
            AddPixelRow(rightGrid, 225);
            AddLeftOverRow(rightGrid, 1);
            AddAutoColumns(rightGrid, 1);
            Grid grid1 = new Grid();
            AddControlToGrid(rightGrid, grid1, 0, 0);
            AddAutoRows(grid1, 1);
            AddPixelColumn(grid1, 200);
            AddPixelColumn(grid1, 150);
            AddPixelColumn(grid1, 200);
            AddLeftOverColumn(grid1, 1);
            StackPanel stack1 = new StackPanel();
            AddControlToGrid(grid1, stack1, 0, 0);
            stack1.Children.Add(_mailPile);
            stack1.Children.Add(_dealPile);
            stack1.Children.Add(otherStack);
            AddControlToGrid(grid1, tempScroll, 0, 1); // instructions on card
            AddControlToGrid(grid1, firstContent, 0, 2);
            AddControlToGrid(grid1, _thisScore, 0, 3);
            Grid grid2 = new Grid();
            AddControlToGrid(rightGrid, grid2, 1, 0);
            AddAutoRows(grid2, 1);
            AddAutoColumns(grid2, 1);
            AddLeftOverColumn(grid2, 1);
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(_dealList);
            AddControlToGrid(grid2, finalStack, 0, 0);
            _currentPiece = new PawnPiecesWPF<EnumColorChoice>();
            _currentPiece.Visibility = Visibility.Collapsed; // until proven to need it
            _currentPiece.Width = 80;
            _currentPiece.Height = 80;
            _currentPiece.Margin = new Thickness(5, 5, 5, 5);
            _currentPiece.Init();
            finalStack.Children.Add(_currentPiece);
            AddControlToGrid(grid2, _mailList, 0, 1);
            tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.DataContext = ThisMod!.PopUpList; //i think.
            tempStack.SetBinding(VisibilityProperty, GetVisibleBinding(nameof(ListViewPicker.Visible)));
            tempStack.Children.Add(_thisPopUp);
            Button finals = GetGamingButton("Submit", nameof(PaydayViewModel.SubmitPopUpChoiceCommand));
            finals.HorizontalAlignment = HorizontalAlignment.Left;
            finals.VerticalAlignment = VerticalAlignment.Top;
            tempStack.Children.Add(finals);
            finals.DataContext = ThisMod;
            AddControlToGrid(grid2, tempStack, 0, 1); // they will share.
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(finalStack); //hopefully still okay.  hopefully does not have to fit it elsewhere.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<PaydayPlayerItem, PaydaySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<PaydayViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportionWPF>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, PaydayPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "main");
        }
        public void Handle(NewTurnEventModel message)
        {
            if (_mainGame == null)
                _mainGame = OurContainer!.Resolve<PaydayMainGameClass>();
            _currentPiece!.MainColor = _mainGame.SingleInfo!.Color.ToColor(); //not hooking up to bindings this time.
            _currentPiece.Visibility = Visibility.Visible;
        }
    }
}