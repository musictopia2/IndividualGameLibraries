using A8RoundRummyCP;
using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace A8RoundRummyWPF
{
    public class GamePage : MultiPlayerWindow<A8RoundRummyViewModel, A8RoundRummyPlayerItem, A8RoundRummySaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            A8RoundRummySaveInfo saveRoot = OurContainer!.Resolve<A8RoundRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _roundControl!.Init(_mainGame!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            A8RoundRummySaveInfo saveRoot = OurContainer!.Resolve<A8RoundRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _roundControl!.Update(_mainGame!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private RoundUI? _roundControl;
        private A8RoundRummyMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<A8RoundRummyMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            Grid grid2 = new Grid();
            AddLeftOverColumn(grid2, 60);
            AddLeftOverColumn(grid2, 40); // can adjust as needed
            AddControlToGrid(grid2, thisStack, 0, 0);
            _roundControl = new RoundUI();
            AddControlToGrid(grid2, _roundControl, 0, 1);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Go Out", nameof(A8RoundRummyViewModel.GoOutCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(_playerHandWPF);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(A8RoundRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(A8RoundRummyViewModel.Status));
            firstInfo.AddRow("Next", nameof(A8RoundRummyViewModel.NextTurn));
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Cards Left", true, nameof(A8RoundRummyPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Total Score", true, nameof(A8RoundRummyPlayerItem.TotalScore));
            thisStack.Children.Add(_thisScore);
            MainGrid!.Children.Add(grid2);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<A8RoundRummyViewModel>();
            OurContainer!.RegisterType<DeckViewModel<A8RoundRummyCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<A8RoundRummyPlayerItem, A8RoundRummySaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<A8RoundRummyCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, A8RoundRummyDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterType<TestConfig>();
        }
    }
}