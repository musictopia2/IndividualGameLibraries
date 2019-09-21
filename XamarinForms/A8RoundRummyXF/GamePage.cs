using A8RoundRummyCP;
using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace A8RoundRummyXF
{
    public class GamePage : MultiPlayerPage<A8RoundRummyViewModel, A8RoundRummyPlayerItem, A8RoundRummySaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            A8RoundRummySaveInfo saveRoot = OurContainer!.Resolve<A8RoundRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _roundControl!.Update(_mainGame!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>? _playerHand;
        private RoundUI? _roundControl;
        private A8RoundRummyMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<A8RoundRummyMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            Grid grid2 = new Grid();
            AddLeftOverColumn(grid2, 65);
            AddLeftOverColumn(grid2, 40); // can adjust as needed
            AddControlToGrid(grid2, thisStack, 0, 0);
            _roundControl = new RoundUI();
            AddControlToGrid(grid2, _roundControl, 0, 1);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Go Out", nameof(A8RoundRummyViewModel.GoOutCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(_playerHand);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
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
        }
    }
}