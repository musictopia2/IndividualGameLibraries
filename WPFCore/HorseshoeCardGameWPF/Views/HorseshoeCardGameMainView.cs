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
using HorseshoeCardGameCP.Cards;
using HorseshoeCardGameCP.Data;
using HorseshoeCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HorseshoeCardGameWPF.Views
{
    public class HorseshoeCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly HorseshoeCardGameVMData _model;
        private readonly HorseshoeCardGameGameContainer _gameContainer;
        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>> _playerHandWPF;

        private readonly SeveralPlayersTrickWPF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem> _trick1;

        private readonly PlayerBoardWPF<HorseshoeCardGameCardInformation> _temp1 = new PlayerBoardWPF<HorseshoeCardGameCardInformation>();
        private readonly PlayerBoardWPF<HorseshoeCardGameCardInformation> _temp2 = new PlayerBoardWPF<HorseshoeCardGameCardInformation>();

        public HorseshoeCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            HorseshoeCardGameVMData model,
            HorseshoeCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>>();

            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem>();



            StackPanel mainStack = new StackPanel();
            mainStack.Children.Add(_trick1);

            _score.AddColumn("Cards Left", false, nameof(HorseshoeCardGamePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tricks Won", true, nameof(HorseshoeCardGamePlayerItem.TricksWon), rightMargin: 10);
            _score.AddColumn("Previous Score", false, nameof(HorseshoeCardGamePlayerItem.PreviousScore));
            _score.AddColumn("Total Score", false, nameof(HorseshoeCardGamePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(HorseshoeCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HorseshoeCardGameMainViewModel.Status));


            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(HorseshoeCardGameMainViewModel.RestoreScreen)
                };
            }


            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);

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
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            HorseshoeCardGameSaveInfo save = cons!.Resolve<HorseshoeCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, ts.TagUsed);

            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetSelf();
            HorseshoeCardGamePlayerItem thisPlayer;
            if (_gameContainer.SingleInfo.Id == 1)
                thisPlayer = _gameContainer.PlayerList[2];
            else
                thisPlayer = _gameContainer.PlayerList[1];
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
