using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkuckCardGameCP.Cards;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SkuckCardGameWPF.Views
{
    public class SkuckCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SkuckCardGameVMData _model;
        private readonly SkuckCardGameGameContainer _gameContainer;
        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>> _trick1;

        private readonly PlayerBoardWPF<SkuckCardGameCardInformation> _temp1 = new PlayerBoardWPF<SkuckCardGameCardInformation>();
        private readonly PlayerBoardWPF<SkuckCardGameCardInformation> _temp2 = new PlayerBoardWPF<SkuckCardGameCardInformation>();


        public SkuckCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            SkuckCardGameVMData model,
            SkuckCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, SkuckCardGameCardInformation, ts, DeckOfCardsWPF<SkuckCardGameCardInformation>>();

            _playerHandWPF.Divider = 1.2;
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SkuckCardGameMainViewModel.RestoreScreen)
                };
            }

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_trick1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(SkuckCardGameMainViewModel.BidScreen)
            };
            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(SkuckCardGameMainViewModel.SuitScreen)
            };
            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(SkuckCardGameMainViewModel.ChoosePlayScreen)
            };
            otherStack.Children.Add(parent);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Strength Hand", false, nameof(SkuckCardGamePlayerItem.StrengthHand));
            _score.AddColumn("Tie Breaker", false, nameof(SkuckCardGamePlayerItem.TieBreaker));
            _score.AddColumn("Bid Amount", false, nameof(SkuckCardGamePlayerItem.BidAmount), visiblePath: nameof(SkuckCardGamePlayerItem.BidVisible));
            _score.AddColumn("Tricks Taken", false, nameof(SkuckCardGamePlayerItem.TricksWon));
            _score.AddColumn("Cards In Hand", false, nameof(SkuckCardGamePlayerItem.ObjectCount));
            _score.AddColumn("Perfect Rounds", false, nameof(SkuckCardGamePlayerItem.PerfectRounds));
            _score.AddColumn("Total Score", false, nameof(SkuckCardGamePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Round", nameof(SkuckCardGameMainViewModel.RoundNumber));
            firstInfo.AddRow("Trump", nameof(SkuckCardGameMainViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Turn", nameof(SkuckCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SkuckCardGameMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            Grid finalGrid = new Grid();
            Grid tempGrid = new Grid();
            AddLeftOverRow(tempGrid, 1);
            AddLeftOverRow(tempGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            AddControlToGrid(finalGrid, tempGrid, 0, 1);
            AddControlToGrid(tempGrid, _temp1, 0, 0);
            AddControlToGrid(tempGrid, _temp2, 1, 0);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SkuckCardGameSaveInfo save = cons!.Resolve<SkuckCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
            SkuckCardGamePlayerItem thisPlayer;
            if (_gameContainer.SingleInfo!.Id == 1)
                thisPlayer = _gameContainer.PlayerList![2];
            else
                thisPlayer = _gameContainer.PlayerList![1];

            _temp1.LoadList(_gameContainer.SingleInfo.TempHand!);
            _temp2.LoadList(thisPlayer.TempHand!);
            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
