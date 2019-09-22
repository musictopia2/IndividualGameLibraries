using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CribbageCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CribbageXF
{
    public class GamePage : MultiPlayerPage<CribbageViewModel, CribbagePlayerItem, CribbageSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            CribbageSaveInfo saveRoot = OurContainer!.Resolve<CribbageSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _crib1!.LoadList(ThisMod!.CribFrame!, ts.TagUsed);
            _main1!.LoadList(ThisMod.MainFrame!, ts.TagUsed);
            _otherScore.LoadLists(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            CribbageSaveInfo saveRoot = OurContainer!.Resolve<CribbageSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _crib1!.UpdateList(ThisMod.CribFrame!);
            _main1!.UpdateList(ThisMod.MainFrame!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>? _deckGPile;
        private BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>? _playerHand;
        private BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>? _crib1;
        BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>? _main1;
        private readonly ScoreUI _otherScore = new ScoreUI();
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _discardGPile = new BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _crib1 = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _main1 = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _main1.Divider = 1.5;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_main1);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", false, nameof(CribbagePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Skunk Hole", false, nameof(CribbagePlayerItem.IsSkunk), useTrueFalse: true);
            _thisScore.AddColumn("First P", false, nameof(CribbagePlayerItem.FirstPosition));
            _thisScore.AddColumn("Second P", false, nameof(CribbagePlayerItem.SecondPosition));
            _thisScore.AddColumn("Score Round", false, nameof(CribbagePlayerItem.ScoreRound));
            _thisScore.AddColumn("Total Score", false, nameof(CribbagePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CribbageViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CribbageViewModel.Status));
            firstInfo.AddRow("Dealer", nameof(CribbageViewModel.Dealer));
            SimpleLabelGridXF others = new SimpleLabelGridXF();
            others.AddRow("Count", nameof(CribbageViewModel.TotalCount));
            thisStack.Children.Add(_playerHand);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            var thisBut = GetGamingButton("Continue", nameof(CribbageViewModel.ContinueCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("To Crib", nameof(CribbageViewModel.CribCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Play", nameof(CribbageViewModel.PlayCommand));
            otherStack.Children.Add(thisBut);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_crib1);
            Grid FinalGrid = new Grid();
            AddPixelRow(FinalGrid, 300); // hopefully this is enough
            AddLeftOverRow(FinalGrid, 1);
            AddLeftOverColumn(FinalGrid, 70);
            AddLeftOverColumn(FinalGrid, 30);
            AddControlToGrid(FinalGrid, thisStack, 0, 0);
            Grid.SetRowSpan(thisStack, 2);
            MainGrid!.Children.Add(FinalGrid);
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(others.GetContent);
            finalStack.Children.Add(_otherScore);
            AddControlToGrid(FinalGrid, finalStack, 0, 1);
            AddControlToGrid(FinalGrid, _thisScore, 1, 1);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<CribbagePlayerItem, CribbageSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<CribbageViewModel, CribbageCard>();
        }
    }
}