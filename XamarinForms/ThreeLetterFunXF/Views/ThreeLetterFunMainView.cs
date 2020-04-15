using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP.Data;
using ThreeLetterFunCP.GraphicsCP;
using ThreeLetterFunCP.Logic;
using ThreeLetterFunCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ThreeLetterFunXF.Views
{
    public class ThreeLetterFunMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, INewCard
    {
        private readonly IEventAggregator _aggregator;
        private readonly ThreeLetterFunMainGameClass _mainGame;
        private readonly ThreeLetterFunVMData _model;
        private readonly GameBoard _gameBoard;
        private readonly ScoreBoardXF _score;
        private readonly TileHandXF _tileBoard1 = new TileHandXF();
        private readonly CardBoardXF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsXF> _gameBoard1 = new CardBoardXF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsXF>();
        private readonly CardGraphicsXF _currentCard; //not sure why
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
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ThreeLetterFunMainViewModel.RestoreScreen));
            }

            _tempCard = new ThreeLetterFunCardData();
            _tempCard.Visible = false;
            _currentCard = new CardGraphicsXF();
            _currentCard.SendSize(ThreeLetterFunCardGraphicsCP.TagUsed, _tempCard); //i think.

            var winLabel = GetDefaultLabel();
            winLabel.SetBinding(Label.TextProperty, new Binding(nameof(ThreeLetterFunMainViewModel.PlayerWon)));
            _score = new ScoreBoardXF();
            _score.AddColumn("Cards Won", true, nameof(ThreeLetterFunPlayerItem.CardsWon));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
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

            

            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            if (_mainGame.BasicData.MultiPlayer)
            {
                _score.LoadLists(_mainGame.PlayerList);
                //hopefully no need for the other controls since something else handles it now.
            }
            else
            {
                _score.IsVisible = false;
            }
            _tileBoard1.Init(_model);
            _gameBoard1.LoadList(_gameBoard, ThreeLetterFunCardGraphicsCP.TagUsed);
            return this.RefreshBindingsAsync(_aggregator);
        }

        void INewCard.ShowNewCard()
        {
            _currentCard.BindingContext = null;
            if (_model!.CurrentCard != null)
                _currentCard!.BindingContext = _model.CurrentCard;
            else
                _currentCard!.BindingContext = _tempCard; //hopefully even here, setting to nothing won't hurt.
            _currentCard.IsVisible = true;
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
            return Task.CompletedTask;
        }
    }
}
