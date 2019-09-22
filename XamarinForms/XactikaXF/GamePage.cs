using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using XactikaCP;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace XactikaXF
{
    public class GamePage : MultiPlayerPage<XactikaViewModel, XactikaPlayerItem, XactikaSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisScore!.LoadLists(_mainGame!.SaveRoot!.PlayerList); // the data should come from the playeritem.
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _trick1!.Init(_mainGame.TrickArea1!, _mainGame.TrickArea1!, "");
            _gameOptions1!.LoadLists(ThisMod.ModeChoose1!);
            _stats1.LoadBoard();
            _bid1!.LoadLists(ThisMod);
            _shape1!.Init(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _thisScore!.UpdateLists(_mainGame!.SaveRoot!.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame.TrickArea1!);
            return Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF>? _playerHand;
        private SeveralPlayersTrickXF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF, XactikaPlayerItem>? _trick1;
        private readonly StatBoardXF _stats1 = new StatBoardXF();
        private StackLayout? _gameStack1;
        private ListChooserXF? _gameOptions1;
        private BidUI? _bid1;
        private ChooseShapeXF? _shape1;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            var thisBind = new Binding(nameof(XactikaViewModel.ModeVisible));
            _gameStack1!.SetBinding(IsVisibleProperty, thisBind);
            thisGrid.Children.Add(_gameStack1);
        }
        private XactikaMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<XactikaMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            _gameOptions1 = new ListChooserXF();
            _gameStack1 = new StackLayout();
            _bid1 = new BidUI();
            _shape1 = new ChooseShapeXF();
            _gameOptions1.Orientation = StackOrientation.Horizontal;
            BasicSetUp();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF>();
            _trick1 = new SeveralPlayersTrickXF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF, XactikaPlayerItem>(); //hopefully no need for heightrequest (?)
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(RoundButton); //since most games are in rounds.
            thisStack.Children.Add(GameButton);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", false, nameof(XactikaPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Bid Amount", false, nameof(XactikaPlayerItem.BidAmount));
            _thisScore.AddColumn("Tricks Won", false, nameof(XactikaPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(XactikaPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", false, nameof(XactikaPlayerItem.TotalScore));
            SimpleLabelGridXF FirstInfo = new SimpleLabelGridXF();
            FirstInfo.AddRow("Turn", nameof(XactikaViewModel.NormalTurn));
            FirstInfo.AddRow("Status", nameof(XactikaViewModel.Status));
            FirstInfo.AddRow("Round", nameof(XactikaViewModel.RoundNumber));
            FirstInfo.AddRow("Mode", nameof(XactikaViewModel.GameModeText));
            Button thisBut = GetGamingButton("Submit Game Option", nameof(XactikaViewModel.ModeCommand));
            if (ScreenUsed == EnumScreen.LargeTablet)
            {
                _gameOptions1.ItemHeight = 130;
                _gameOptions1.ItemWidth = 500;
                thisBut.FontSize = 125;
            }
            else
            {
                _gameOptions1.ItemHeight = 80;
                _gameOptions1.ItemWidth = 300;
                thisBut.FontSize = 100;
            }
            _gameStack1.Children.Add(_gameOptions1);
            _gameStack1.Children.Add(thisBut);
            StackLayout shapeStack = new StackLayout();
            var thisBind = new Binding(nameof(XactikaViewModel.ShapeVisible));
            shapeStack.Children.Add(_shape1);
            thisBut = GetGamingButton("Choose Shape", nameof(XactikaViewModel.ChooseShapeNumberCommand));
            thisBut.SetBinding(Button.IsVisibleProperty, thisBind); // because in this case, the button would not be visible but still needs to know what shape was selected.
            shapeStack.Children.Add(thisBut);
            Grid tempGrid = new Grid();
            AddAutoRows(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            AddAutoColumns(tempGrid, 2);
            StackLayout TempStack = new StackLayout();
            TempStack.Orientation = StackOrientation.Horizontal;
            TempStack.Children.Add(_trick1);
            TempStack.Children.Add(shapeStack);
            AddControlToGrid(tempGrid, TempStack, 0, 0);
            AddControlToGrid(tempGrid, _bid1, 0, 0); // if one is visible, then the other is not
            AddControlToGrid(tempGrid, _stats1, 0, 2);
            AddControlToGrid(tempGrid, _thisScore, 0, 1);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(FirstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<XactikaViewModel>();
            OurContainer!.RegisterType<DeckViewModel<XactikaCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<XactikaPlayerItem, XactikaSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<XactikaCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, XactikaDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterType<SeveralPlayersTrickViewModel<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo>>();
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>("main"); //here too.
            OurContainer.RegisterSingleton(_stats1.ThisElement, "main");
            OurContainer.RegisterType<StandardWidthHeight>(); //i think i forgot this too.  hopefully no need for custom widths.
        }
    }
}