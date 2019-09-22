using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BladesOfSteelCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BladesOfSteelWPF
{
    public class GamePage : MultiPlayerWindow<BladesOfSteelViewModel, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            BladesOfSteelSaveInfo saveRoot = OurContainer!.Resolve<BladesOfSteelSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _yourFace!.Init(ThisMod.YourFaceOffCard!, ts.TagUsed);
            _opponentFace!.Init(ThisMod.OpponentFaceOffCard!, ts.TagUsed);
            _mainDefenseCards!.LoadList(ThisMod.MainDefense1!, ts.TagUsed);
            _yourAttack!.LoadList(ThisMod.YourAttackPile!, ts.TagUsed);
            _yourDefense!.LoadList(ThisMod.YourDefensePile!, ts.TagUsed);
            _opponentAttack!.LoadList(ThisMod.OpponentAttackPile!, ts.TagUsed);
            _opponentDefense!.LoadList(ThisMod.OpponentDefensePile!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            BladesOfSteelSaveInfo saveRoot = OurContainer!.Resolve<BladesOfSteelSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _yourFace!.UpdatePile(ThisMod.YourFaceOffCard!);
            _opponentFace!.UpdatePile(ThisMod.OpponentFaceOffCard!);
            _mainDefenseCards!.UpdateList(ThisMod.MainDefense1!);
            _yourAttack!.UpdateList(ThisMod.YourAttackPile!);
            _yourDefense!.UpdateList(ThisMod.YourDefensePile!);
            _opponentAttack!.UpdateList(ThisMod.OpponentAttackPile!);
            _opponentDefense!.UpdateList(ThisMod.OpponentDefensePile!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _deckGPile;
        private BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _playerHandWPF;
        private BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _yourFace;
        private BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _opponentFace;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _mainDefenseCards;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _opponentDefense;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _opponentAttack;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _yourDefense;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _yourAttack;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _discardGPile = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _yourFace = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentFace = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _mainDefenseCards = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentDefense = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentAttack = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _yourDefense = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _yourAttack = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            ScoringGuideWPF tempScore = new ScoringGuideWPF();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(tempScore);
            _thisScore.AddColumn("Cards Left", true, nameof(BladesOfSteelPlayerItem.ObjectCount), rightMargin: 5);
            _thisScore.AddColumn("Score", true, nameof(BladesOfSteelPlayerItem.Score), rightMargin: 5);
            otherStack.Children.Add(_thisScore);
            thisStack.Children.Add(otherStack);
            MainGrid!.Children.Add(thisStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel firstStack = new StackPanel();
            AddVerticalLabelGroup("Instructions", nameof(BladesOfSteelViewModel.Instructions), firstStack);
            otherStack.Children.Add(firstStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(BladesOfSteelViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(BladesOfSteelViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(BladesOfSteelViewModel.OtherPlayer));
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            Grid playerArea = new Grid();
            AddAutoColumns(playerArea, 3);
            AddAutoRows(playerArea, 2);
            _opponentDefense.Margin = new Thickness(0, 0, 0, 20);
            AddControlToGrid(playerArea, _opponentDefense, 0, 2);
            AddControlToGrid(playerArea, _opponentAttack, 0, 1);
            _opponentAttack.Margin = new Thickness(0, 0, 0, 20);
            AddControlToGrid(playerArea, _mainDefenseCards, 1, 0);
            AddControlToGrid(playerArea, _yourAttack, 1, 1);
            AddControlToGrid(playerArea, _yourDefense, 1, 2);
            thisStack.Children.Add(playerArea);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            AddControlToGrid(playerArea, otherStack, 0, 0);
            StackPanel faceStack = new StackPanel();
            faceStack.Orientation = Orientation.Horizontal;
            var thisBind = GetVisibleBinding(nameof(BladesOfSteelViewModel.IsFaceOff));
            faceStack.SetBinding(StackPanel.VisibilityProperty, thisBind);
            _yourFace.Margin = new Thickness(0, 0, 30, 0); // i think
            faceStack.Children.Add(_yourFace);
            faceStack.Children.Add(_opponentFace);
            thisStack.Children.Add(faceStack);
            otherStack = new StackPanel();
            thisBind = GetVisibleBinding(nameof(BladesOfSteelViewModel.CommandsVisible));
            otherStack.SetBinding(StackPanel.VisibilityProperty, thisBind);
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(BladesOfSteelViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.VerticalAlignment = VerticalAlignment.Center;
            otherStack.Children.Add(endButton);
            var otherBut = GetGamingButton("Pass", nameof(BladesOfSteelViewModel.PassCommand));
            otherStack.Children.Add(otherBut);
            otherBut.HorizontalAlignment = HorizontalAlignment.Left;
            otherBut.VerticalAlignment = VerticalAlignment.Center;
            otherStack.Children.Add(_playerHandWPF);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<BladesOfSteelViewModel, RegularSimpleCard>(aceLow: false);
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
        }
    }
}