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
using CribbageCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CribbageWPF
{
    public class GamePage : MultiPlayerWindow<CribbageViewModel, CribbagePlayerItem, CribbageSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            CribbageSaveInfo saveRoot = OurContainer!.Resolve<CribbageSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _crib1!.UpdateList(ThisMod.CribFrame!);
            _main1!.UpdateList(ThisMod.MainFrame!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>? _deckGPile;
        private BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>? _playerHandWPF;
        private BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>? _crib1;
        BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>? _main1;
        private readonly ScoreUI _otherScore = new ScoreUI();
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _discardGPile = new BasePileWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _crib1 = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _main1 = new BaseHandWPF<CribbageCard, ts, DeckOfCardsWPF<CribbageCard>>();
            _main1.Divider = 1.5;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_main1);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Cards Left", false, nameof(CribbagePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Is Skunk Hole", false, nameof(CribbagePlayerItem.IsSkunk), useTrueFalse: true);
            _thisScore.AddColumn("First Position", false, nameof(CribbagePlayerItem.FirstPosition));
            _thisScore.AddColumn("Second Position", false, nameof(CribbagePlayerItem.SecondPosition));
            _thisScore.AddColumn("Score Round", false, nameof(CribbagePlayerItem.ScoreRound));
            _thisScore.AddColumn("Total Score", false, nameof(CribbagePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(CribbageViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CribbageViewModel.Status));
            firstInfo.AddRow("Dealer", nameof(CribbageViewModel.Dealer));
            SimpleLabelGrid others = new SimpleLabelGrid();
            others.AddRow("Count", nameof(CribbageViewModel.TotalCount));
            thisStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            var thisBut = GetGamingButton("Continue", nameof(CribbageViewModel.ContinueCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("To Crib", nameof(CribbageViewModel.CribCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Play", nameof(CribbageViewModel.PlayCommand));
            otherStack.Children.Add(thisBut);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
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
            StackPanel finalStack = new StackPanel();
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