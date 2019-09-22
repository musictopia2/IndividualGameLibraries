using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using Rummy500CP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Rummy500WPF
{
    public class GamePage : MultiPlayerWindow<Rummy500ViewModel, Rummy500PlayerItem, Rummy500SaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            Rummy500SaveInfo saveRoot = OurContainer!.Resolve<Rummy500SaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardRummy!.UpdateList(ThisMod.DiscardList1!);
            _mainG!.Update(ThisMod.MainSets1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _deckGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _playerHandWPF;
        private BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _discardRummy;
        private MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet>? _mainG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _playerHandWPF.Margin = new Thickness(5, 5, 5, 5);
            _playerHandWPF.HorizontalAlignment = HorizontalAlignment.Stretch;
            _discardRummy = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet>();
            Grid finalGrid = new Grid();
            AddAutoColumns(finalGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            AddControlToGrid(finalGrid, thisStack, 0, 1);
            MainGrid!.Children.Add(finalGrid);
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Cards Left", false, nameof(Rummy500PlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Points Played", false, nameof(Rummy500PlayerItem.PointsPlayed));
            _thisScore.AddColumn("Cards Played", false, nameof(Rummy500PlayerItem.CardsPlayed));
            _thisScore.AddColumn("Score Current", false, nameof(Rummy500PlayerItem.CurrentScore));
            _thisScore.AddColumn("Score Total", false, nameof(Rummy500PlayerItem.TotalScore));
            otherStack.Children.Add(_thisScore);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Rummy500ViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Rummy500ViewModel.Status));
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            Button thisBut;
            thisBut = GetGamingButton("Discard Current", nameof(Rummy500ViewModel.DiscardCurrentCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Create New Rummy Set", nameof(Rummy500ViewModel.CreateSetCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _mainG.Divider = 1.3;
            _mainG.Height = 550; // i think
            thisStack.Children.Add(_mainG);
            _discardRummy.Divider = 1.7;
            _discardRummy.HandType = HandViewModel<RegularRummyCard>.EnumHandList.Vertical;
            _discardRummy.HorizontalAlignment = HorizontalAlignment.Left;
            _discardRummy.VerticalAlignment = VerticalAlignment.Stretch;
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