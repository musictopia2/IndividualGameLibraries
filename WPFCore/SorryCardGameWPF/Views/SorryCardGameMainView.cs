using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SorryCardGameCP.Cards;
using SorryCardGameCP.Data;
using SorryCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace SorryCardGameWPF.Views
{
    public class SorryCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SorryCardGameVMData _model;
        private readonly SorryCardGameGameContainer _gameContainer;
        private readonly BaseDeckWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly StackPanel _boardStack;
        private readonly BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF> _otherPileWPF;
        private readonly CustomBasicList<BoardWPF> _boardList = new CustomBasicList<BoardWPF>();


        public SorryCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            SorryCardGameVMData model,
            SorryCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();

            _boardStack = new StackPanel();
            _otherPileWPF = new BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SorryCardGameMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _score.AddColumn("Cards Left", false, nameof(SorryCardGamePlayerItem.ObjectCount)); //very common.
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPileWPF);
            var tempLabel = GetDefaultLabel();
            tempLabel.FontSize = 40;
            tempLabel.FontWeight = FontWeights.Bold;
            tempLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(SorryCardGameMainViewModel.UpTo)));
            otherStack.Children.Add(tempLabel);
            mainStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SorryCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SorryCardGameMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SorryCardGameMainViewModel.Instructions));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_playerHandWPF);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_boardStack);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            _otherPileWPF.Margin = new Thickness(5);
            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SorryCardGameSaveInfo save = cons!.Resolve<SorryCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _otherPileWPF!.Init(_model.OtherPile!, "");
            _otherPileWPF.StartAnimationListener("otherpile");
            LoadBoard();

            return this.RefreshBindingsAsync(_aggregator);
        }

        private void LoadBoard()
        {
            var thisList = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            _boardStack!.Children.Clear();
            _boardStack.Children.Add(tempStack);
            int x = 0;
            thisList.ForEach(thisPlayer =>
            {
                x++;
                var thisControl = new BoardWPF();
                thisControl.LoadList(thisPlayer, _gameContainer.Command);
                _boardList.Add(thisControl);
                if (x == 3)
                {
                    tempStack = new StackPanel();
                    tempStack.Orientation = Orientation.Horizontal;
                    _boardStack.Children.Add(tempStack);
                }
                tempStack.Children.Add(thisControl);
            });
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _boardList.ForEach(x => x.Dispose());
            return Task.CompletedTask;
        }
    }
}
