using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace LifeBoardGameXF
{
    public class GamePage : MultiPlayerPage<LifeBoardGameViewModel, LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisColor!.LoadLists(ThisMod!.ColorChooser!);
            _thisPop!.LoadLists(ThisMod.PopUpList!);
            _popScore!.LoadLists(_mainGame!.PlayerList!);
            _thisBoard.LoadBoard();
            _thisSpin!.LoadBoard();
            _thisScore!.LoadLists(_mainGame!.PlayerList!);
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
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<CarPieceCP, CarPieceXF,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        #region "Other Controls"
        private ListChooserXF? _thisPop; //i think.  if i am wrong. rethink.
        private readonly GameBoardXF _thisBoard = new GameBoardXF(); //has to be here or can't register properly.
        private ScoreBoardXF? _thisScore;
        private LifeHandXF? _thisHand;
        private LifePileXF? _thisPile;
        private SpinnerXF? _thisSpin;
        private LifeBoardGameMainGameClass? _mainGame;
        private CarPieceXF? _thisCar;
        private ScoreBoardXF? _popScore;
        #endregion
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _mainGame = OurContainer!.Resolve<LifeBoardGameMainGameClass>();
            _thisColor = new EnumPickerXF<CarPieceCP, CarPieceXF, EnumColorChoice
                , ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(LifeBoardGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(LifeBoardGameViewModel.Instructions));
            Binding thisBind = new Binding(nameof(LifeBoardGameViewModel.ColorVisible));
            _thisColor.GraphicsHeight = 248 * .8f;
            _thisColor.GraphicsWidth = 136 * .8f;
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        private StackLayout GetDetailStack()
        {
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(LifeBoardGameViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(LifeBoardGameViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(LifeBoardGameViewModel.Status));
            StackLayout output = new StackLayout();
            output.Children.Add(firstInfo.GetContent);
            AddVerticalLabelGroup("Space Details", nameof(LifeBoardGameViewModel.GameDetails), output);
            return output;
        }
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            #region "Create Controls"
            SimpleLabelGridXF genderInfo = new SimpleLabelGridXF();
            genderInfo.AddRow("Turn", nameof(LifeBoardGameViewModel.NormalTurn));
            genderInfo.AddRow("Instructions", nameof(LifeBoardGameViewModel.Instructions));
            StackLayout genderStack = new StackLayout();
            LifeVisibleXF lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.Gender;
            SetVisibleBindingStatus(genderStack, lifeV);
            genderStack.Margin = new Thickness(5, 5, 5, 5);
            GenderXF thisG = new GenderXF(EnumGender.Boy);
            genderStack.Children.Add(thisG);
            thisG = new GenderXF(EnumGender.Girl);
            genderStack.Children.Add(thisG);
            _thisCar = new CarPieceXF();
            _thisCar.HeightRequest = 93;
            _thisCar.WidthRequest = 51;
            _thisCar.Init(); //i think.  not sure if something else will be needed or not (?)
            _thisCar.Margin = new Thickness(0, 20, 0, 0);
            genderStack.Children.Add(_thisCar);
            genderStack.Children.Add(genderInfo.GetContent);
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.GameBoard;
            SetVisibleBindingStatus(_thisBoard, lifeV);
            _thisHand = new LifeHandXF();
            _thisPile = new LifePileXF();
            _thisSpin = new SpinnerXF();
            _thisSpin.HorizontalOptions = LayoutOptions.Start;
            _thisSpin.VerticalOptions = LayoutOptions.Start;
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.Spinner;
            SetVisibleBindingStatus(_thisSpin, lifeV);
            Button subButton = GetGamingButton("Submit", nameof(LifeBoardGameViewModel.SubmitCommand));
            subButton.HorizontalOptions = LayoutOptions.Start;
            subButton.VerticalOptions = LayoutOptions.Start;
            subButton.Margin = new Thickness(10, 10, 0, 0);
            subButton.FontSize = 80;
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.SubmitOption;
            SetVisibleBindingStatus(subButton, lifeV);
            _thisPop = new ListChooserXF();
            _thisPop.ItemHeight = 50; //try to experiment.
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Money", true, nameof(LifeBoardGamePlayerItem.MoneyEarned), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("Salary", true, nameof(LifeBoardGamePlayerItem.Salary), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("S 1", true, nameof(LifeBoardGamePlayerItem.FirstStock));
            _thisScore.AddColumn("Tiles", true, nameof(LifeBoardGamePlayerItem.TilesCollected));
            _thisScore.AddColumn("Career", true, nameof(LifeBoardGamePlayerItem.Career1), rightMargin: 10);
            _thisScore.AddColumn("S 2", true, nameof(LifeBoardGamePlayerItem.SecondStock));
            _thisScore.AddColumn("S Career", true, nameof(LifeBoardGamePlayerItem.Career2));
            _thisScore.AddColumn("Loans", true, nameof(LifeBoardGamePlayerItem.Loans), useCurrency: true, rightMargin: 10);
            _thisScore.WidthRequest = 500; //experiment with this part.
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.Scoreboard;
            SetVisibleBindingStatus(_thisScore, lifeV);
            Grid lifeParentGrid = new Grid();
            AddAutoColumns(lifeParentGrid, 1);
            AddLeftOverRow(lifeParentGrid, 10);
            AddLeftOverRow(lifeParentGrid, 5);
            Grid lifeBodyGrid = new Grid();
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.Gender;
            lifeV.IsOpposites = true;
            SetVisibleBindingStatus(lifeParentGrid, lifeV);
            StackLayout spinPlusHandStack = new StackLayout();
            StackLayout houseStack = new StackLayout();
            StackLayout handStack = new StackLayout();
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.CardList;
            SetVisibleBindingStatus(handStack, lifeV);
            handStack.VerticalOptions = LayoutOptions.Start;
            handStack.Orientation = StackOrientation.Horizontal;
            handStack.Children.Add(_thisHand);
            houseStack.HorizontalOptions = LayoutOptions.Start;
            houseStack.VerticalOptions = LayoutOptions.Start;
            houseStack.Children.Add(handStack);
            Button spinButton = GetGamingButton("Spin", nameof(LifeBoardGameViewModel.SpinCommand));
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.SpinButton;
            SetVisibleBindingStatus(spinButton, lifeV);
            houseStack.Children.Add(spinButton);
            _thisBoard.HorizontalOptions = LayoutOptions.Start;
            _thisBoard.VerticalOptions = LayoutOptions.Start;
            _thisHand.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<LifeBaseCard>.EnumHandList.Vertical;
            StackLayout otherStack = new StackLayout();
            otherStack.Children.Add(subButton);
            StackLayout finDetail = GetDetailStack();
            otherStack.Children.Add(finDetail);
            handStack.Children.Add(otherStack);
            spinPlusHandStack.Children.Add(_thisSpin);
            spinPlusHandStack.Children.Add(_thisPile);
            StackLayout detailStack = GetDetailStack();
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.Scoreboard;
            SetVisibleBindingStatus(detailStack, lifeV);
            #endregion
            #region "LayoutControls"
            lifeBodyGrid.Children.Add(genderStack);
            lifeBodyGrid.Children.Add(lifeParentGrid);
            lifeBodyGrid.Children.Add(houseStack);
            Grid controlGrid = new Grid();
            AddControlToGrid(lifeParentGrid, _thisBoard, 0, 0);
            AddControlToGrid(lifeParentGrid, controlGrid, 1, 0);
            AddAutoRows(controlGrid, 1);
            AddAutoColumns(controlGrid, 1);
            AddLeftOverColumn(controlGrid, 1);
            AddControlToGrid(controlGrid, detailStack, 0, 1);
            Grid finalGrid = new Grid();
            AddAutoColumns(finalGrid, 1);
            AddLeftOverRow(finalGrid, 30);
            AddLeftOverRow(finalGrid, 70);
            AddControlToGrid(controlGrid, finalGrid, 0, 0);
            AddControlToGrid(finalGrid, _thisScore, 0, 0);
            StackLayout popStack = new StackLayout();
            lifeBodyGrid.Children.Add(popStack); //has to be this way.
            lifeV = new LifeVisibleXF();
            lifeV.Category = EnumVisibleCategory.PopUp;
            SetVisibleBindingStatus(popStack, lifeV);
            popStack.Children.Add(_thisPop);
            Button finalBut = GetGamingButton("Submit", nameof(LifeBoardGameViewModel.SubmitCommand));
            finalBut.HorizontalOptions = LayoutOptions.Start;
            finalBut.VerticalOptions = LayoutOptions.Start;
            popStack.Children.Add(finalBut);
            Button endBut = GetGamingButton("End Turn", nameof(LifeBoardGameViewModel.EndTurnCommand));
            endBut.HorizontalOptions = LayoutOptions.Start;
            endBut.VerticalOptions = LayoutOptions.Start;
            popStack.Children.Add(endBut);
            StackLayout temps = GetDetailStack();
            popStack.Children.Add(temps);
            _popScore = new ScoreBoardXF();
            _popScore.AddColumn("Nick Name", true, nameof(LifeBoardGamePlayerItem.NickName));
            _popScore.AddColumn("Salary", true, nameof(LifeBoardGamePlayerItem.Salary), useCurrency: true, rightMargin: 10);
            _popScore.AddColumn("Tiles", true, nameof(LifeBoardGamePlayerItem.TilesCollected));
            popStack.Children.Add(_popScore);
            AddControlToGrid(finalGrid, spinPlusHandStack, 1, 0);
            thisStack.Children.Add(lifeBodyGrid);
            #endregion
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, BoardProportion>(); //here too.
            OurContainer!.RegisterType<BasicGameLoader<LifeBoardGamePlayerItem, LifeBoardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<LifeBoardGameViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
        private void SetVisibleBindingStatus(View thisControl, IValueConverter thisConverter)
        {
            Binding thisBind = new Binding(nameof(LifeBoardGameViewModel.GameStatus));
            thisBind.Converter = thisConverter;
            thisControl.SetBinding(IsVisibleProperty, thisBind);
        }
        public void Handle(NewTurnEventModel message)
        {
            _thisCar!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
            _thisCar.IsVisible = true;
        }
    }
}