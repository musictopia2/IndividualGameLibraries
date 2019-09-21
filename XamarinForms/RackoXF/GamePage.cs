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
using RackoCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace RackoXF
{
    public class GamePage : MultiPlayerPage<RackoViewModel, RackoPlayerItem, RackoSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            RackoSaveInfo saveRoot = OurContainer!.Resolve<RackoSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _hand!.Init(_mainGame!);
            _current!.Init(ThisMod.Pile2!, "");
            _current.StartAnimationListener("otherpile");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            RackoSaveInfo saveRoot = OurContainer!.Resolve<RackoSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _hand!.Update(_mainGame!);
            _current!.UpdatePile(ThisMod.Pile2!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>? _discardGPile;
        private BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>? _current;
        private ScoreBoardXF? _thisScore;
        private RackoUI? _hand; //use this instead.
        private RackoMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<RackoMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _current = new BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>();
            _hand = new RackoUI();
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 2);
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            MainGrid!.Children.Add(finalGrid); // forgot this.
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            thisStack.Children.Add(_deckGPile);
            thisStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(_current);
            var thisBut = GetSmallerButton("Discard Current Card", nameof(RackoViewModel.DiscardCurrentCommand));
            thisStack.Children.Add(thisBut);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RackoViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RackoViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            thisBut = GetSmallerButton("Racko", nameof(RackoViewModel.RackoCommand));
            thisStack.Children.Add(thisBut);
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            _thisScore.AddColumn("Score Round", true, nameof(RackoPlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", true, nameof(RackoPlayerItem.TotalScore));
            int x;
            for (x = 1; x <= 10; x++)
                _thisScore.AddColumn("Section" + x, false, "Value" + x, nameof(RackoPlayerItem.CanShowValues));// 2 bindings.
            _hand = new RackoUI();
            AddControlToGrid(finalGrid, _hand, 0, 1); // first column
            AddControlToGrid(finalGrid, _thisScore, 1, 0);
            Grid.SetColumnSpan(_thisScore, 2);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RackoViewModel>();
            OurContainer!.RegisterType<DeckViewModel<RackoCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<RackoPlayerItem, RackoSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<RackoCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RackoDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
        }
    }
}