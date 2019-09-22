using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using GolfCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GolfCardGameXF
{
    public class GamePage : MultiPlayerPage<GolfCardGameViewModel, GolfCardGamePlayerItem, GolfCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            GolfCardGameSaveInfo saveRoot = OurContainer!.Resolve<GolfCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _otherPile!.Init(ThisMod.Pile2!, ts.TagUsed);
            _hidden!.Init(ThisMod.HiddenCards1!, ts.TagUsed);
            _begin!.LoadList(ThisMod.Beginnings1!, ts.TagUsed);
            _golf!.LoadList(ThisMod.GolfHand1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            GolfCardGameSaveInfo saveRoot = OurContainer!.Resolve<GolfCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _otherPile!.UpdatePile(ThisMod.Pile2!);
            _hidden!.UpdateLists(ThisMod.HiddenCards1!);
            _begin!.UpdateList(ThisMod.Beginnings1!);
            _golf!.UpdateList(ThisMod.GolfHand1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _deckGPile;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _hidden;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _otherPile;
        private CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _begin;
        private CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _golf;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardXF();
            _hidden = new BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _otherPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _begin = new CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _golf = new CardBoardXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(RoundButton);
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Knock", nameof(GolfCardGameViewModel.KnockedCommand));
            var thisBind = new Binding(nameof(GolfCardGameViewModel.KnockedVisible)); // hopefully its this simple (?)
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_begin);
            thisBut = GetGamingButton("Choose First Cards", nameof(GolfCardGameViewModel.ChooseFirstCardsCommand));
            thisBind = new Binding(nameof(GolfCardGameViewModel.ChooseFirstCardsVisible));
            thisBut.SetBinding(Button.IsVisibleProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_hidden);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_golf);
            otherStack.Children.Add(_otherPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.UseAbbreviationForTrueFalse = true;
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 40);
            AddLeftOverColumn(finalGrid, 60); // this is for scoreboard
            _thisScore.AddColumn("Knocked", false, nameof(GolfCardGamePlayerItem.Knocked), useTrueFalse: true); // well see how this work.  hopefully this simple.
            _thisScore.AddColumn("1 Changed", false, nameof(GolfCardGamePlayerItem.FirstChanged), useTrueFalse: true);
            _thisScore.AddColumn("2 Changed", false, nameof(GolfCardGamePlayerItem.SecondChanged), useTrueFalse: true);
            _thisScore.AddColumn("Previous Score", false, nameof(GolfCardGamePlayerItem.PreviousScore), rightMargin: 20);
            _thisScore.AddColumn("Total Score", false, nameof(GolfCardGamePlayerItem.TotalScore), rightMargin: 20);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GolfCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GolfCardGameViewModel.Status));
            firstInfo.AddRow("Round", nameof(GolfCardGameViewModel.Round));
            firstInfo.AddRow("Instructions", nameof(GolfCardGameViewModel.Instructions));
            thisStack.Children.Add(finalGrid);
            AddControlToGrid(finalGrid, firstInfo.GetContent, 0, 0);
            AddControlToGrid(finalGrid, _thisScore, 0, 1);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<GolfCardGamePlayerItem, GolfCardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<GolfCardGameViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            OurContainer.RegisterSingleton<IDeckCount, GolfDeck>();
        }
    }
}