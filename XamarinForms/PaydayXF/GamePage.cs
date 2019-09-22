using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
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
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using PaydayCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace PaydayXF
{
    public class GamePage : MultiPlayerPage<PaydayViewModel, PaydayPlayerItem, PaydaySaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private StackLayout? _chooseColorStack;

        private EnumPickerXF<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>,
            EnumColorChoice, ColorListChooser<EnumColorChoice>>? _thisColor;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        private PaydayMainGameClass? _mainGame;
        private readonly GameBoardXF _thisBoard = new GameBoardXF();
        private ScoreBoardXF? _thisScore;
        private ListChooserXF? _thisPopUp;
        private BasePileXF<MailCard, CardGraphicsCP, MailCardXF>? _mailPile;
        private BasePileXF<DealCard, CardGraphicsCP, DealCardXF>? _dealPile;
        private BaseHandXF<MailCard, CardGraphicsCP, MailCardXF>? _mailList;
        private BaseHandXF<DealCard, CardGraphicsCP, DealCardXF>? _dealList;
        private PawnPiecesXF<EnumColorChoice>? _currentPiece;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<PawnPiecesCP<EnumColorChoice>, PawnPiecesXF<EnumColorChoice>, EnumColorChoice, ColorListChooser<EnumColorChoice>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(PaydayViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(PaydayViewModel.Instructions));
            Binding thisBind = new Binding(nameof(PaydayViewModel.ColorVisible));
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
            _thisPopUp = new ListChooserXF();
            _thisPopUp.ItemHeight = 50;
            _mailPile = new BasePileXF<MailCard, CardGraphicsCP, MailCardXF>();
            _dealPile = new BasePileXF<DealCard, CardGraphicsCP, DealCardXF>();
            _mailList = new BaseHandXF<MailCard, CardGraphicsCP, MailCardXF>();
            _dealList = new BaseHandXF<DealCard, CardGraphicsCP, DealCardXF>();
            _mailList.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<MailCard>.EnumHandList.Vertical;
            _dealList.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<DealCard>.EnumHandList.Vertical;
            _mailList.HorizontalOptions = LayoutOptions.Start;
            _dealList.HorizontalOptions = LayoutOptions.Start;
            _mailList.HeightRequest = 500;
            _dealList.HeightRequest = 500;
            var thisRoll = GetSmallerButton("Roll Dice", nameof(PaydayViewModel.RollCommand));
            StackLayout otherStack = new StackLayout();
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(thisRoll);
            otherStack.Children.Add(_diceControl); //little iffy
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Money", true, nameof(PaydayPlayerItem.MoneyHas), useCurrency: true, rightMargin: 10);
            _thisScore.AddColumn("Loans", true, nameof(PaydayPlayerItem.Loans), useCurrency: true, rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Main Turn", nameof(PaydayViewModel.NormalTurn));
            firstInfo.AddRow("Other Turn", nameof(PaydayViewModel.OtherLabel));
            firstInfo.AddRow("Progress", nameof(PaydayViewModel.MonthLabel));
            firstInfo.AddRow("Status", nameof(PaydayViewModel.Status));
            var firstContent = firstInfo.GetContent;
            StackLayout tempStack = new StackLayout();
            AddVerticalLabelGroup("Instructions", nameof(PaydayViewModel.Instructions), tempStack);
            Button dealButton = GetSmallerButton("Choose Deal", nameof(PaydayViewModel.DealCommand));
            dealButton.VerticalOptions = LayoutOptions.Start;
            DealConverter convs = new DealConverter();
            Binding thisBind = new Binding(nameof(PaydayViewModel.GameStatus));
            thisBind.Converter = convs;
            dealButton.SetBinding(IsVisibleProperty, thisBind);
            otherStack.Children.Add(dealButton);
            ScrollView tempScroll = new ScrollView();
            tempScroll.Orientation = ScrollOrientation.Vertical;
            tempScroll.Content = tempStack;
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(_dealList);
            _currentPiece = new PawnPiecesXF<EnumColorChoice>();
            _currentPiece.IsVisible = false; // until proven to need it
            _currentPiece.WidthRequest = 70;
            _currentPiece.HeightRequest = 70;
            _currentPiece.Margin = new Thickness(5, 5, 5, 5);
            _currentPiece.Init();
            finalStack.Children.Add(_currentPiece);
            tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            tempStack.BindingContext = ThisMod!.PopUpList; //i think.
            tempStack.SetBinding(IsVisibleProperty, new Binding(nameof(ListViewPicker.Visible)));
            tempStack.Children.Add(_thisPopUp);
            Button finals = GetSmallerButton("Submit", nameof(PaydayViewModel.SubmitPopUpChoiceCommand));
            finals.HorizontalOptions = LayoutOptions.Start;
            finals.VerticalOptions = LayoutOptions.Start;
            tempStack.Children.Add(finals);
            finals.BindingContext = ThisMod;
            Grid finalGrid = new Grid();
            AddAutoColumns(finalGrid, 1);
            AddLeftOverRow(finalGrid, 90);
            AddLeftOverRow(finalGrid, 25);
            AddLeftOverRow(finalGrid, 80);
            AddControlToGrid(finalGrid, _thisBoard, 0, 0);
            Grid headerGrid = new Grid();
            AddControlToGrid(finalGrid, headerGrid, 1, 0);
            Grid endGrid = new Grid();
            AddControlToGrid(finalGrid, endGrid, 2, 0);
            AddAutoRows(endGrid, 1);
            AddAutoColumns(endGrid, 1);
            AddLeftOverColumn(endGrid, 1);
            AddControlToGrid(endGrid, finalStack, 0, 0);
            AddControlToGrid(endGrid, tempStack, 0, 1);
            

            AddControlToGrid(endGrid, _mailList, 0, 1);
            AddLeftOverColumn(headerGrid, 10);
            AddLeftOverColumn(headerGrid, 11);
            AddLeftOverColumn(headerGrid, 13);
            AddLeftOverColumn(headerGrid, 15);
            StackLayout tempGrid = new StackLayout();
            tempGrid.Children.Add(_mailPile);
            tempGrid.Children.Add(_dealPile);
            tempGrid.Children.Add(otherStack);
            AddControlToGrid(headerGrid, tempGrid, 0, 0);
            AddControlToGrid(headerGrid, tempScroll, 0, 1);
            AddControlToGrid(headerGrid, firstContent, 0, 2);
            AddControlToGrid(headerGrid, _thisScore, 0, 3);
            thisStack.Children.Add(finalGrid);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportionXF>("main"); //here too.
            OurContainer!.RegisterType<BasicGameLoader<PaydayPlayerItem, PaydaySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<PaydayViewModel>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
            OurContainer.RegisterSingleton<IProportionImage, CustomProportionXF>(); //i think
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, PaydayPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "main");
        }
        public void Handle(NewTurnEventModel message)
        {
            if (_mainGame == null)
                _mainGame = OurContainer!.Resolve<PaydayMainGameClass>();
            _currentPiece!.MainColor = _mainGame.SingleInfo!.Color.ToColor(); //not hooking up to bindings this time.
            _currentPiece.IsVisible = true;
        }
    }
}