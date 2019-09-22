using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using FluxxCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace FluxxWPF
{
    public class GamePage : MultiPlayerWindow<FluxxViewModel, FluxxPlayerItem, FluxxSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            FluxxSaveInfo saveRoot = OurContainer!.Resolve<FluxxSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetSelf();
            _keeper1!.LoadControls();
            _action1!.LoadControls();
            _rule1.LoadControls();
            _goal1.LoadList(ThisMod.Goal1!, "");
            _keeperHand1.LoadList(ThisMod.Keeper1!, "");
            _cardDetail1.LoadControls(EnumShowCategory.MainScreen);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            FluxxSaveInfo saveRoot = OurContainer!.Resolve<FluxxSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetSelf();
            _keeper1 = new KeeperUI();
            _action1 = new ActionUI();
            _keeper1.LoadControls();
            _action1.LoadControls(); //try this way since they are too complex otherwise.
            _rule1.UpdateControls();
            _goal1.UpdateList(ThisMod.Goal1!);
            _keeperHand1.UpdateList(ThisMod.Keeper1!);
            return Task.CompletedTask;
        }
        protected override void ComplexStartControls(Grid thisGrid)
        {
            base.ComplexStartControls(thisGrid);
            thisGrid.Children.Add(_keeper1);
            thisGrid.Children.Add(_action1);
        }
        private BaseDeckWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private FluxxHandWPF? _playerHandWPF;
        private KeeperUI? _keeper1;
        private ActionUI? _action1;
        readonly ShowCardUI _cardDetail1 = new ShowCardUI();
        readonly RuleUI _rule1 = new RuleUI();
        readonly GoalHandWPF _goal1 = new GoalHandWPF();
        readonly KeeperHandWPF _keeperHand1 = new KeeperHandWPF();
        private FluxxMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<FluxxMainGameClass>();
            _keeper1 = new KeeperUI();
            _action1 = new ActionUI();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<FluxxCardInformation, FluxxGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _cardDetail1.Width = 700;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_rule1);
            otherStack.Children.Add(_cardDetail1);
            otherStack.Children.Add(_goal1);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(FluxxViewModel.EndTurnCommand));
            otherStack.Children.Add(endButton);
            var thisBut = GetGamingButton("Discard", nameof(FluxxViewModel.DiscardCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Play", nameof(FluxxViewModel.PlayCommand));
            var thisBind = new Binding(nameof(FluxxViewModel.PlayGiveText));
            thisBut.SetBinding(ContentProperty, thisBind); // do this instead.  hopefully that works.
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Unselect All", nameof(FluxxViewModel.UnselectAllCardsCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Select All", nameof(FluxxViewModel.SelectAllCardsCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Show Keepers", nameof(FluxxViewModel.ShowKeepersCommand));
            otherStack.Children.Add(thisBut);
            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 75);
            AddAutoColumns(tempGrid, 1);
            TrueFalseConverter tConverter = new TrueFalseConverter();
            tConverter.UseAbb = false;
            DetailGameInformationWPF detail1 = new DetailGameInformationWPF();
            detail1.Margin = new Thickness(3, 3, 3, 3);
            {
                var withBlock = detail1;
                withBlock.AddRow("Plays Left", nameof(FluxxViewModel.PlaysLeft));
                withBlock.AddRow("Hand Limit", nameof(FluxxViewModel.HandLimit));
                withBlock.AddRow("Keeper Limit", nameof(FluxxViewModel.KeeperLimit));
                withBlock.AddRow("Play Limit", nameof(FluxxViewModel.PlayLimit));
                withBlock.AddRow("Another Turn", nameof(FluxxViewModel.AnotherTurn), tConverter);
                withBlock.AddRow("Current Turn", nameof(FluxxViewModel.NormalTurn));
                withBlock.AddRow("Other Turn", nameof(FluxxViewModel.OtherTurn));
                withBlock.AddRow("Status", nameof(FluxxViewModel.Status));
                withBlock.AddRow("Draw Bonus", nameof(FluxxViewModel.DrawBonus));
                withBlock.AddRow("Play Bonus", nameof(FluxxViewModel.PlayBonus));
                withBlock.AddRow("Cards Drawn", nameof(FluxxViewModel.CardsDrawn));
                withBlock.AddRow("Cards Played", nameof(FluxxViewModel.CardsPlayed));
                withBlock.AddRow("Draw Rules", nameof(FluxxViewModel.DrawRules));
                withBlock.AddRow("Previous" + Constants.vbCrLf + "Bonus", nameof(FluxxViewModel.PreviousBonus));
            }
            thisStack.Children.Add(tempGrid);
            StackPanel finalStack = new StackPanel();
            AddControlToGrid(tempGrid, detail1, 0, 1);
            AddControlToGrid(tempGrid, finalStack, 0, 0);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _keeperHand1.HandType = HandViewModel<KeeperCard>.EnumHandList.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_keeperHand1);
            finalStack.Children.Add(otherStack);
            _thisScore.UseAbbreviationForTrueFalse = true; // this time has to be abbreviated
            _thisScore.AddColumn("# In Hand", false, nameof(FluxxPlayerItem.ObjectCount));
            _thisScore.AddColumn("# Of Keepers", false, nameof(FluxxPlayerItem.NumberOfKeepers));
            _thisScore.AddColumn("Bread", false, nameof(FluxxPlayerItem.Bread), useTrueFalse: true);
            _thisScore.AddColumn("Chocolate", false, nameof(FluxxPlayerItem.Chocolate), useTrueFalse: true);
            _thisScore.AddColumn("Cookies", false, nameof(FluxxPlayerItem.Cookies), useTrueFalse: true);
            _thisScore.AddColumn("Death", false, nameof(FluxxPlayerItem.Death), useTrueFalse: true);
            _thisScore.AddColumn("Dreams", false, nameof(FluxxPlayerItem.Dreams), useTrueFalse: true);
            _thisScore.AddColumn("Love", false, nameof(FluxxPlayerItem.Love), useTrueFalse: true);
            _thisScore.AddColumn("Milk", false, nameof(FluxxPlayerItem.Milk), useTrueFalse: true);
            _thisScore.AddColumn("Money", false, nameof(FluxxPlayerItem.Money), useTrueFalse: true);
            _thisScore.AddColumn("Peace", false, nameof(FluxxPlayerItem.Peace), useTrueFalse: true);
            _thisScore.AddColumn("Sleep", false, nameof(FluxxPlayerItem.Sleep), useTrueFalse: true);
            _thisScore.AddColumn("Television", false, nameof(FluxxPlayerItem.Television), useTrueFalse: true);
            _thisScore.AddColumn("The Brain", false, nameof(FluxxPlayerItem.TheBrain), useTrueFalse: true);
            _thisScore.AddColumn("The Moon", false, nameof(FluxxPlayerItem.TheMoon), useTrueFalse: true);
            _thisScore.AddColumn("The Rocket", false, nameof(FluxxPlayerItem.TheRocket), useTrueFalse: true);
            _thisScore.AddColumn("The Sun", false, nameof(FluxxPlayerItem.TheSun), useTrueFalse: true);
            _thisScore.AddColumn("The Toaster", false, nameof(FluxxPlayerItem.TheToaster), useTrueFalse: true);
            _thisScore.AddColumn("Time", false, nameof(FluxxPlayerItem.Time), useTrueFalse: true);
            _thisScore.AddColumn("War", false, nameof(FluxxPlayerItem.War), useTrueFalse: true);
            _playerHandWPF = new FluxxHandWPF();
            _playerHandWPF.Divider = 1.2;
            finalStack.Children.Add(_playerHandWPF);
            _keeperHand1.MinWidth = 300;
            finalStack.Children.Add(_thisScore);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FluxxViewModel>();
            OurContainer!.RegisterType<DeckViewModel<FluxxCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<FluxxPlayerItem, FluxxSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<FluxxCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FluxxDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
        }
    }
}