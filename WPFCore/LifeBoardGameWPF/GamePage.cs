using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace LifeBoardGameWPF
{
    public class GamePage : MultiPlayerWindow<LifeBoardGameViewModel, LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _thisPop!.LoadLists(ThisMod.PopUpList!);
            _popScore!.LoadLists(_mainGame!.PlayerList!);
            _thisBoard.LoadBoard();
            _thisSpin!.LoadBoard();
            _thisScore!.LoadLists(_mainGame.PlayerList!);
            _thisHand!.LoadList(ThisMod.HandList!, "");
            _thisPile!.Init(ThisMod.ChosenPile!, "");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _thisScore!.UpdateLists(_mainGame!.PlayerList!);
            _thisHand!.UpdateList(ThisMod!.HandList!);
            _thisPile!.UpdatePile(ThisMod.ChosenPile!);
            _popScore!.UpdateLists(_mainGame.PlayerList!);
            return Task.CompletedTask;
        }
        private StackPanel? _chooseColorStack;
        private EnumPickerWPF<CarPieceCP, CarPieceWPF,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        #region "Other Controls"
        private ListChooserWPF? _thisPop; //i think.  if i am wrong. rethink.
        private readonly GameBoardWPF _thisBoard = new GameBoardWPF(); //has to be here or can't register properly.
        private ScoreBoardWPF? _thisScore;
        private LifeHandWPF? _thisHand;
        private LifePileWPF? _thisPile;
        private SpinnerWPF? _thisSpin;
        private LifeBoardGameMainGameClass? _mainGame;
        private CarPieceWPF? _thisCar;
        private ScoreBoardWPF? _popScore;
        #endregion
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _mainGame = OurContainer!.Resolve<LifeBoardGameMainGameClass>();
            _thisColor = new EnumPickerWPF<CarPieceCP, CarPieceWPF,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(LifeBoardGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(LifeBoardGameViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(LifeBoardGameViewModel.ColorVisible));
            _thisColor.GraphicsHeight = 248;
            _thisColor.GraphicsWidth = 136;
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
            #region "Create Controls"
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(LifeBoardGameViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(LifeBoardGameViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(LifeBoardGameViewModel.Status));
            SimpleLabelGrid genderInfo = new SimpleLabelGrid();
            genderInfo.AddRow("Turn", nameof(LifeBoardGameViewModel.NormalTurn));
            genderInfo.AddRow("Instructions", nameof(LifeBoardGameViewModel.Instructions));
            StackPanel genderStack = new StackPanel();
            LifeVisibleWPF lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.Gender;
            SetVisibleBindingStatus(genderStack, lifeV);
            genderStack.Margin = new Thickness(5, 5, 5, 5);
            GenderWPF thisG = new GenderWPF(EnumGender.Boy);
            genderStack.Children.Add(thisG);
            thisG = new GenderWPF(EnumGender.Girl);
            genderStack.Children.Add(thisG);
            _thisCar = new CarPieceWPF();
            _thisCar.Height = 186;
            _thisCar.Width = 102;
            _thisCar.Init(); //i think.  not sure if something else will be needed or not (?)
            _thisCar.Margin = new Thickness(0, 20, 0, 0);
            genderStack.Children.Add(_thisCar);
            genderStack.Children.Add(genderInfo.GetContent);
            lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.GameBoard;
            SetVisibleBindingStatus(_thisBoard, lifeV);
            _thisHand = new LifeHandWPF();
            _thisPile = new LifePileWPF();
            _thisSpin = new SpinnerWPF();
            _thisSpin.HorizontalAlignment = HorizontalAlignment.Left;
            _thisSpin.VerticalAlignment = VerticalAlignment.Top;
            lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.Spinner;
            SetVisibleBindingStatus(_thisSpin, lifeV);
            Button subButton = GetGamingButton("Submit", nameof(LifeBoardGameViewModel.SubmitCommand));
            subButton.HorizontalAlignment = HorizontalAlignment.Left;
            subButton.VerticalAlignment = VerticalAlignment.Top;
            subButton.Margin = new Thickness(10, 10, 0, 0);
            subButton.FontSize = 200;
            lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.SubmitOption;
            SetVisibleBindingStatus(subButton, lifeV);
            _thisPop = new ListChooserWPF();
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Money", true, nameof(LifeBoardGamePlayerItem.MoneyEarned), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("Loans", true, nameof(LifeBoardGamePlayerItem.Loans), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("Stock 1", true, nameof(LifeBoardGamePlayerItem.FirstStock));
            _thisScore.AddColumn("Stock 2", true, nameof(LifeBoardGamePlayerItem.SecondStock));
            _thisScore.AddColumn("Career", true, nameof(LifeBoardGamePlayerItem.Career1), rightMargin: 10);
            _thisScore.AddColumn("Salary", true, nameof(LifeBoardGamePlayerItem.Salary), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("Tiles", true, nameof(LifeBoardGamePlayerItem.TilesCollected));
            _thisScore.AddColumn("Car I.", true, nameof(LifeBoardGamePlayerItem.CarIsInsured), useTrueFalse: true);
            _thisScore.AddColumn("House I.", true, nameof(LifeBoardGamePlayerItem.HouseIsInsured), useTrueFalse: true);
            _thisScore.AddColumn("House N.", true, nameof(LifeBoardGamePlayerItem.HouseName));
            _thisScore.AddColumn("S Career", true, nameof(LifeBoardGamePlayerItem.Career2));
            lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.Scoreboard;
            SetVisibleBindingStatus(_thisScore, lifeV);
            StackPanel lifeParentStack = new StackPanel();
            lifeParentStack.Orientation = Orientation.Horizontal;
            Grid lifeBodyGrid = new Grid();
            lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.Gender;
            lifeV.IsOpposites = true;
            SetVisibleBindingStatus(lifeParentStack, lifeV);
            StackPanel spinPlusParentStack = new StackPanel();
            StackPanel handStack = new StackPanel();
            handStack.Orientation = Orientation.Horizontal;
            handStack.Children.Add(_thisHand);
            _thisBoard.HorizontalAlignment = HorizontalAlignment.Left;
            _thisBoard.VerticalAlignment = VerticalAlignment.Top;
            _thisHand.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<LifeBaseCard>.EnumHandList.Vertical;
            _thisHand.Height = 900;
            handStack.Children.Add(subButton);
            spinPlusParentStack.Children.Add(_thisSpin);
            spinPlusParentStack.Children.Add(_thisPile);
            spinPlusParentStack.Children.Add(handStack);
            StackPanel detailStack = new StackPanel();
            detailStack.Children.Add(firstInfo.GetContent);
            AddVerticalLabelGroup("Space Details", nameof(LifeBoardGameViewModel.GameDetails), detailStack);
            #endregion
            #region "LayoutControls"
            lifeBodyGrid.Children.Add(genderStack);
            lifeBodyGrid.Children.Add(lifeParentStack);
            Grid controlGrid = new Grid();
            lifeParentStack.Children.Add(_thisBoard);
            lifeParentStack.Children.Add(controlGrid);
            AddAutoRows(controlGrid, 1);
            AddPixelColumn(controlGrid, 800);
            AddPixelColumn(controlGrid, 180);
            AddControlToGrid(controlGrid, detailStack, 0, 1);
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1);
            AddControlToGrid(controlGrid, finalGrid, 0, 0);
            AddLeftOverRow(finalGrid, 30);
            AddLeftOverRow(finalGrid, 70);
            AddControlToGrid(finalGrid, _thisScore, 0, 0);
            StackPanel popStack = new StackPanel();
            lifeV = new LifeVisibleWPF();
            lifeV.Category = EnumVisibleCategory.PopUp;
            SetVisibleBindingStatus(popStack, lifeV);
            popStack.Children.Add(_thisPop);
            Button finalBut = GetGamingButton("Submit", nameof(LifeBoardGameViewModel.SubmitCommand));
            finalBut.HorizontalAlignment = HorizontalAlignment.Left;
            finalBut.VerticalAlignment = VerticalAlignment.Top;
            popStack.Children.Add(finalBut);
            _popScore = new ScoreBoardWPF();
            _popScore.AddColumn("Nick Name", true, nameof(LifeBoardGamePlayerItem.NickName));
            _popScore.AddColumn("Salary", true, nameof(LifeBoardGamePlayerItem.Salary), useCurrency: true, rightMargin: 10);
            popStack.Children.Add(_popScore);
            AddControlToGrid(finalGrid, popStack, 0, 0);
            Grid.SetRowSpan(popStack, 2);
            AddControlToGrid(finalGrid, spinPlusParentStack, 1, 0);
            Grid.SetRowSpan(spinPlusParentStack, 2);
            thisStack.Children.Add(lifeBodyGrid);
            #endregion
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(); //here too.
            OurContainer!.RegisterType<BasicGameLoader<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer.RegisterNonSavedClasses<LifeBoardGameViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, CardProportion>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
        private void SetVisibleBindingStatus(FrameworkElement thisControl, IValueConverter thisConverter)
        {
            Binding thisBind = new Binding(nameof(LifeBoardGameViewModel.GameStatus));
            thisBind.Converter = thisConverter;
            thisControl.SetBinding(VisibilityProperty, thisBind);
        }
        public void Handle(NewTurnEventModel message)
        {
            _thisCar!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
            _thisCar.Visibility = Visibility.Visible;
        }
    }
}