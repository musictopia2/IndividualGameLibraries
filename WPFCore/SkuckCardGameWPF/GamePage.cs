using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using SkuckCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SkuckCardGameWPF
{
    public class GamePage : MultiPlayerWindow<SkuckCardGameViewModel, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            SkuckCardGameSaveInfo saveRoot = OurContainer!.Resolve<SkuckCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _bid1.LoadList(ThisMod);
            _suit1.LoadList(ThisMod);
            _play1.LoadList();
            SkuckCardGamePlayerItem thisPlayer;
            if (_mainGame.SingleInfo!.Id == 1)
                thisPlayer = _mainGame.PlayerList![2];
            else
                thisPlayer = _mainGame.PlayerList![1];

            _temp1.LoadList(_mainGame.SingleInfo.TempHand!);
            _temp2.LoadList(thisPlayer.TempHand!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            SkuckCardGameSaveInfo saveRoot = OurContainer!.Resolve<SkuckCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            SkuckCardGamePlayerItem thisPlayer;
            if (_mainGame.SingleInfo.Id == 1)
                thisPlayer = _mainGame.PlayerList[2];
            else
                thisPlayer = _mainGame.PlayerList[1];
            _temp1.UpdateList(_mainGame.SingleInfo.TempHand!);
            _temp2.UpdateList(thisPlayer.TempHand!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>>? _trick1;
        private SkuckCardGameMainGameClass? _mainGame;
        private readonly BidControl _bid1 = new BidControl();
        private readonly TrumpControl _suit1 = new TrumpControl();
        private readonly PlayControl _play1 = new PlayControl();
        private readonly PlayerBoardWPF<SkuckCardGameCardInformation> _temp1 = new PlayerBoardWPF<SkuckCardGameCardInformation>();
        private readonly PlayerBoardWPF<SkuckCardGameCardInformation> _temp2 = new PlayerBoardWPF<SkuckCardGameCardInformation>();
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<SkuckCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>>();
            //Binding ThisBind = GetVisibleBinding(nameof(SkuckCardGameViewModel.TrickVisible));
            //_trick1.SetBinding(VisibilityProperty, ThisBind);
            _playerHandWPF.Divider = 1.2;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_bid1);
            otherStack.Children.Add(_suit1);
            otherStack.Children.Add(_play1);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Strength Hand", false, nameof(SkuckCardGamePlayerItem.StrengthHand));
            _thisScore.AddColumn("Tie Breaker", false, nameof(SkuckCardGamePlayerItem.TieBreaker));
            _thisScore.AddColumn("Bid Amount", false, nameof(SkuckCardGamePlayerItem.BidAmount), visiblePath: nameof(SkuckCardGamePlayerItem.BidVisible));
            _thisScore.AddColumn("Tricks Taken", false, nameof(SkuckCardGamePlayerItem.TricksWon));
            _thisScore.AddColumn("Cards In Hand", false, nameof(SkuckCardGamePlayerItem.ObjectCount));
            _thisScore.AddColumn("Perfect Rounds", false, nameof(SkuckCardGamePlayerItem.PerfectRounds));
            _thisScore.AddColumn("Total Score", false, nameof(SkuckCardGamePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Round", nameof(SkuckCardGameViewModel.RoundNumber));
            firstInfo.AddRow("Trump", nameof(SkuckCardGameViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Turn", nameof(SkuckCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SkuckCardGameViewModel.Status));
            thisStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            Grid finalGrid = new Grid();
            Grid TempGrid = new Grid();
            AddLeftOverRow(TempGrid, 1);
            AddLeftOverRow(TempGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            AddControlToGrid(finalGrid, TempGrid, 0, 1);
            AddControlToGrid(TempGrid, _temp1, 0, 0);
            AddControlToGrid(TempGrid, _temp2, 1, 0);
            MainGrid!.Children.Add(finalGrid);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SkuckCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<SkuckCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<SkuckCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<SkuckCardGameCardInformation> ThisSort = new SortSimpleCards<SkuckCardGameCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem, SkuckCardGameSaveInfo>>();
            OurContainer.RegisterType<StandardWidthHeight>();
        }
    }
}