using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
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
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using Rummy500CP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Rummy500XF
{
    public class GamePage : MultiPlayerPage<Rummy500ViewModel, Rummy500PlayerItem, Rummy500SaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            Rummy500SaveInfo saveRoot = OurContainer!.Resolve<Rummy500SaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _discardRummy!.LoadList(ThisMod.DiscardList1!, ts.TagUsed);
            _mainG!.Init(ThisMod.MainSets1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            Rummy500SaveInfo saveRoot = OurContainer!.Resolve<Rummy500SaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardRummy!.UpdateList(ThisMod.DiscardList1!);
            _mainG!.Update(ThisMod.MainSets1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _deckGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _playerHand;
        private BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _discardRummy;
        private MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _playerHand.Margin = new Thickness(5, 5, 5, 5);
            _playerHand.HorizontalOptions = LayoutOptions.Fill;
            _discardRummy = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet>();
            Grid finalGrid = new Grid();
            AddAutoColumns(finalGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            AddControlToGrid(finalGrid, thisStack, 0, 1);
            MainGrid!.Children.Add(finalGrid);
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Cards Left", false, nameof(Rummy500PlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Points Played", false, nameof(Rummy500PlayerItem.PointsPlayed));
            _thisScore.AddColumn("Cards Played", false, nameof(Rummy500PlayerItem.CardsPlayed));
            _thisScore.AddColumn("Score Current", false, nameof(Rummy500PlayerItem.CurrentScore));
            _thisScore.AddColumn("Score Total", false, nameof(Rummy500PlayerItem.TotalScore));
            otherStack.Children.Add(_thisScore);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Rummy500ViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Rummy500ViewModel.Status));
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_playerHand);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            Button thisBut;
            thisBut = GetGamingButton("Discard Current", nameof(Rummy500ViewModel.DiscardCurrentCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Create New Rummy Set", nameof(Rummy500ViewModel.CreateSetCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _mainG.Divider = 1.3;
            thisStack.Children.Add(_mainG);
            _discardRummy.Divider = 1.4;
            _discardRummy.ExtraControlSpace = 20;
            _discardRummy.HandType = HandViewModel<RegularRummyCard>.EnumHandList.Vertical;
            _discardRummy.HorizontalOptions = LayoutOptions.Start;
            _discardRummy.VerticalOptions = LayoutOptions.FillAndExpand;
            AddControlToGrid(finalGrid, _discardRummy, 0, 0);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(otherStack); //looks like i am forced to get to the root of the problem.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<Rummy500PlayerItem, Rummy500SaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<Rummy500ViewModel, RegularRummyCard>(aceLow: false, registerCommonProportions: false);
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}