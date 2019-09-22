using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using GoFishCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GoFishXF
{
    public class GamePage : MultiPlayerPage<GoFishViewModel, GoFishPlayerItem, GoFishSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            GoFishSaveInfo saveRoot = OurContainer!.Resolve<GoFishSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _ask1.Init();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            GoFishSaveInfo saveRoot = OurContainer!.Resolve<GoFishSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _deckGPile;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _playerHand;
        private readonly AskUI _ask1 = new AskUI();
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(GoFishViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Start;
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(GoFishPlayerItem.ObjectCount));
            _thisScore.AddColumn("Pairs", true, nameof(GoFishPlayerItem.Pairs));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GoFishViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GoFishViewModel.Status));
            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(endButton);
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_thisScore);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(_ask1);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<GoFishPlayerItem, GoFishSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<GoFishViewModel, RegularSimpleCard>();
            OurContainer.RegisterType<StandardPickerSizeClass>();
        }
    }
}