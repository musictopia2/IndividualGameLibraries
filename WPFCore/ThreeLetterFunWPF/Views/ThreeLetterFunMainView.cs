using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using ThreeLetterFunCP.Logic;
using ThreeLetterFunCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ThreeLetterFunWPF.Views
{
    public class ThreeLetterFunMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, INewCard
    {
        private readonly IEventAggregator _aggregator;
        private readonly ThreeLetterFunMainGameClass _mainGame;
        private readonly ThreeLetterFunVMData _model;
        private readonly GameBoard _gameBoard;
        private readonly ScoreBoardWPF _score;
        private readonly TileHandWPF _tileBoard1 = new TileHandWPF();
        private readonly CardBoardWPF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsWPF> _gameBoard1 = new CardBoardWPF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsWPF>();
        private readonly CardGraphicsWPF _currentCard; //not sure why
        private readonly ThreeLetterFunCardData _tempCard; //not sure.
        public ThreeLetterFunMainView(IEventAggregator aggregator,
            TestOptions test,
            ThreeLetterFunMainGameClass mainGame,
            ThreeLetterFunVMData model,
            GameBoard gameBoard
            )
        {
            _aggregator = aggregator;
            _mainGame = mainGame;
            _model = model;
            _gameBoard = gameBoard;
            model.NewUI = this;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ThreeLetterFunMainViewModel.RestoreScreen)
                };
            }
            _tempCard = new ThreeLetterFunCardData();
            _tempCard.Visible = false;
            _currentCard = new CardGraphicsWPF();
            _currentCard.SendSize(ThreeLetterFunCardGraphicsCP.TagUsed, _tempCard); //i think.

            var winLabel = GetDefaultLabel();
            winLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(ThreeLetterFunMainViewModel.PlayerWon)));
            _score = new ScoreBoardWPF();
            _score.AddColumn("Cards Won", true, nameof(ThreeLetterFunPlayerItem.CardsWon));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(_currentCard);
            mainStack.Children.Add(winLabel);
            mainStack.Children.Add(_gameBoard1);
            mainStack.Children.Add(_tileBoard1);
            var thisBut = GetGamingButton("Play", nameof(ThreeLetterFunMainViewModel.PlayAsync));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Give Up", nameof(ThreeLetterFunMainViewModel.GiveUpAsync));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Take Back", nameof(ThreeLetterFunMainViewModel.TakeBack));
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_score);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }



            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            if (_mainGame.BasicData.MultiPlayer)
            {
                _score.LoadLists(_mainGame.PlayerList);
                //hopefully no need for the other controls since something else handles it now.
            }
            else
            {
                _score.Visibility = Visibility.Collapsed;
            }
            _tileBoard1.Init(_model);
            _gameBoard1.LoadList(_gameBoard, ThreeLetterFunCardGraphicsCP.TagUsed);
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }

        void INewCard.ShowNewCard()
        {
            _currentCard.DataContext = null;
            if (_model!.CurrentCard != null)
                _currentCard!.DataContext = _model.CurrentCard;
            else
                _currentCard!.DataContext = _tempCard; //hopefully even here, setting to nothing won't hurt.
            _currentCard.Visibility = Visibility.Visible;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
