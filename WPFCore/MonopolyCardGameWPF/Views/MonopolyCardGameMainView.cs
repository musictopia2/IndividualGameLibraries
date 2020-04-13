using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.Data;
using MonopolyCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace MonopolyCardGameWPF.Views
{
    public class MonopolyCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly MonopolyCardGameVMData _model;
        private readonly MonopolyCardGameGameContainer _gameContainer;
        private readonly BaseDeckWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly StackPanel _tradeStack = new StackPanel();
        private readonly CustomBasicList<MonopolyHandWPF> _customList = new CustomBasicList<MonopolyHandWPF>();
        private readonly ShowCardUI _cardDetail = new ShowCardUI();

        public MonopolyCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            MonopolyCardGameVMData model,
            MonopolyCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(MonopolyCardGameMainViewModel.RestoreScreen)
                };
            }
            mainStack.Children.Add(_cardDetail);
            Grid finalGrid = new Grid();
            AddPixelColumn(finalGrid, 900);
            AddAutoColumns(finalGrid, 1);
            _tradeStack.Orientation = Orientation.Horizontal;
            AddControlToGrid(finalGrid, _tradeStack, 0, 1);
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            StackPanel tempStack = new StackPanel();
            var thisBut = GetGamingButton("Resume", nameof(MonopolyCardGameMainViewModel.ResumeAsync));
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Go Out", nameof(MonopolyCardGameMainViewModel.GoOutAsync));
            tempStack.Children.Add(thisBut);
            otherStack.Children.Add(tempStack);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(MonopolyCardGamePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Previous Money", true, nameof(MonopolyCardGamePlayerItem.PreviousMoney), useCurrency: true);
            _score.AddColumn("Total Money", true, nameof(MonopolyCardGamePlayerItem.TotalMoney), useCurrency: true);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MonopolyCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonopolyCardGameMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            MonopolyCardGameSaveInfo save = cons!.Resolve<MonopolyCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _cardDetail!.LoadControls(_model);
            var thisList = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                MonopolyHandWPF thisHand = new MonopolyHandWPF();
                thisHand.HandType = HandObservable<MonopolyCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 1.6;
                thisHand.Height = 900;
                thisHand.LoadList(thisPlayer.TradePile!, "");
                thisPlayer.TradePile!.Scroll = thisHand;
                _tradeStack.Children.Add(thisHand);
                _customList!.Add(thisHand);
            });

            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
