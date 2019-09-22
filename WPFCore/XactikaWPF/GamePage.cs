using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XactikaCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace XactikaWPF
{
    public class GamePage : MultiPlayerWindow<XactikaViewModel, XactikaPlayerItem, XactikaSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _thisScore!.LoadLists(_mainGame!.SaveRoot!.PlayerList); // the data should come from the playeritem.
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame.TrickArea1!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private SeveralPlayersTrickWPF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF, XactikaPlayerItem>? _trick1;
        private readonly StatBoardWPF _stats1 = new StatBoardWPF();
        private StackPanel? _gameStack1;
        private ListChooserWPF? _gameOptions1;
        private BidUI? _bid1;
        private ChooseShapeWPF? _shape1;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            var thisBind = GetVisibleBinding(nameof(XactikaViewModel.ModeVisible));
            _gameStack1!.SetBinding(VisibilityProperty, thisBind);
            thisGrid.Children.Add(_gameStack1);
        }
        private XactikaMainGameClass? _mainGame;
        protected override async void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<XactikaMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            _gameOptions1 = new ListChooserWPF();
            _gameStack1 = new StackPanel();
            _bid1 = new BidUI();
            _shape1 = new ChooseShapeWPF();
            _gameOptions1.Orientation = Orientation.Horizontal;
            BasicSetUp();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF>();
            _trick1 = new SeveralPlayersTrickWPF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF, XactikaPlayerItem>();
            _trick1.Width = 500;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(RoundButton); //since most games are in rounds.
            thisStack.Children.Add(GameButton);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(XactikaPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Bid Amount", false, nameof(XactikaPlayerItem.BidAmount));
            _thisScore.AddColumn("Tricks Won", false, nameof(XactikaPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(XactikaPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", false, nameof(XactikaPlayerItem.TotalScore));
            SimpleLabelGrid FirstInfo = new SimpleLabelGrid();
            FirstInfo.AddRow("Turn", nameof(XactikaViewModel.NormalTurn));
            FirstInfo.AddRow("Status", nameof(XactikaViewModel.Status));
            FirstInfo.AddRow("Round", nameof(XactikaViewModel.RoundNumber));
            FirstInfo.AddRow("Mode", nameof(XactikaViewModel.GameModeText));
            Button thisBut;
            _gameOptions1.ItemHeight = 200;
            _gameOptions1.ItemWidth = 600;
            _gameStack1.Children.Add(_gameOptions1);
            thisBut = GetGamingButton("Submit Game Option", nameof(XactikaViewModel.ModeCommand));
            thisBut.FontSize = 150;
            _gameStack1.Children.Add(thisBut);
            StackPanel shapeStack = new StackPanel();
            var thisBind = GetVisibleBinding(nameof(XactikaViewModel.ShapeVisible));
            shapeStack.Children.Add(_shape1);
            thisBut = GetGamingButton("Choose Shape", nameof(XactikaViewModel.ChooseShapeNumberCommand));
            thisBut.SetBinding(Button.VisibilityProperty, thisBind); // because in this case, the button would not be visible but still needs to know what shape was selected.
            shapeStack.Children.Add(thisBut);
            Grid tempGrid = new Grid();
            AddAutoRows(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            AddAutoColumns(tempGrid, 2);
            StackPanel TempStack = new StackPanel();
            TempStack.Orientation = Orientation.Horizontal;
            TempStack.Children.Add(_trick1);
            TempStack.Children.Add(shapeStack);
            AddControlToGrid(tempGrid, TempStack, 0, 0);
            AddControlToGrid(tempGrid, _bid1, 0, 0); // if one is visible, then the other is not
            AddControlToGrid(tempGrid, _stats1, 0, 2);
            AddControlToGrid(tempGrid, _thisScore, 0, 1);
            thisStack.Children.Add(tempGrid);
            thisStack.Children.Add(_playerHandWPF);
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
            OurContainer.RegisterType<StandardWidthHeight>(); //i think i forgot this too.
        }
    }
}