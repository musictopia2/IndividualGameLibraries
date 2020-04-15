using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using HorseshoeCardGameCP.Cards;
using HorseshoeCardGameCP.Data;
using HorseshoeCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace HorseshoeCardGameXF.Views
{
    public class HorseshoeCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly HorseshoeCardGameVMData _model;
        private readonly HorseshoeCardGameGameContainer _gameContainer;
        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>> _playerHandWPF;

        private readonly SeveralPlayersTrickXF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem> _trick1;

        private readonly PlayerBoardXF<HorseshoeCardGameCardInformation> _temp1 = new PlayerBoardXF<HorseshoeCardGameCardInformation>();
        private readonly PlayerBoardXF<HorseshoeCardGameCardInformation> _temp2 = new PlayerBoardXF<HorseshoeCardGameCardInformation>();

        public HorseshoeCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            HorseshoeCardGameVMData model,
            HorseshoeCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem>();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(HorseshoeCardGameMainViewModel.RestoreScreen));
            }
            StackLayout mainStack = new StackLayout();
            mainStack.Children.Add(_trick1);

            _score.AddColumn("Cards", false, nameof(HorseshoeCardGamePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("T Won", true, nameof(HorseshoeCardGamePlayerItem.TricksWon), rightMargin: 10);
            _score.AddColumn("P Score", false, nameof(HorseshoeCardGamePlayerItem.PreviousScore));
            _score.AddColumn("T Score", false, nameof(HorseshoeCardGamePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(HorseshoeCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HorseshoeCardGameMainViewModel.Status));

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
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

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

            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            

            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _playerHandWPF.Dispose(); //at least will help improve performance.
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
