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
using LifeCardGameCP.Cards;
using LifeCardGameCP.Data;
using LifeCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace LifeCardGameWPF.Views
{
    public class LifeCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly LifeCardGameVMData _model;
        private readonly LifeCardGameGameContainer _gameContainer;
        private readonly BaseDeckWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF> _currentCard;
        private readonly CustomBasicList<LifeHandWPF> _lifeList = new CustomBasicList<LifeHandWPF>();
        private readonly StackPanel _storyStack = new StackPanel();

        public LifeCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            LifeCardGameVMData model,
            LifeCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _storyStack.Orientation = Orientation.Horizontal;
            _currentCard = new BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(LifeCardGameMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_currentCard);
            mainStack.Children.Add(otherStack);

            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);

            Button button;
            button = GetGamingButton("Years Passed", nameof(LifeCardGameMainViewModel.YearsPassedAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Play Card", nameof(LifeCardGameMainViewModel.PlayCardAsync));
            otherStack.Children.Add(button);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(LifeCardGameMainViewModel.OtherScreen),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            otherStack.Children.Add(parent);



            _score.AddColumn("Cards Left", true, nameof(LifeCardGamePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Points", true, nameof(LifeCardGamePlayerItem.Points));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(LifeCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(LifeCardGameMainViewModel.Status));

            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            StackPanel finalStack = new StackPanel();
            finalStack.Orientation = Orientation.Horizontal;
            finalStack.Children.Add(mainStack);
            finalStack.Children.Add(_storyStack);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            _currentCard.Margin = new Thickness(5, 5, 5, 5);
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            LifeCardGameSaveInfo save = cons!.Resolve<LifeCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _currentCard!.Init(_model.CurrentPile!, "");
            var thisList = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                LifeHandWPF thisHand = new LifeHandWPF();
                thisHand.HandType = HandObservable<LifeCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 4;
                thisHand.Height = 900;
                thisHand.LoadList(thisPlayer.LifeStory!, "");
                thisPlayer.LifeStory!.ThisScroll = thisHand;
                _storyStack!.Children.Add(thisHand);
                _lifeList!.Add(thisHand);
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
