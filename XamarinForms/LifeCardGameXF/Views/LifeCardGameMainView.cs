using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using LifeCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using LifeCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using LifeCardGameCP.Cards;
using BasicGameFrameworkLibrary.DrawableListsObservable;

namespace LifeCardGameXF.Views
{
    public class LifeCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly LifeCardGameVMData _model;
        private readonly LifeCardGameGameContainer _gameContainer;
        private readonly BaseDeckXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF> _playerHandWPF;
        private readonly BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF> _currentCard;
        private readonly CustomBasicList<LifeHandXF> _lifeList = new CustomBasicList<LifeHandXF>();
        private readonly StackLayout _storyStack = new StackLayout();
        public LifeCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            LifeCardGameVMData model,
            LifeCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _deckGPile = new BaseDeckXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _currentCard = new BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(LifeCardGameMainViewModel.RestoreScreen));
            }
            _storyStack.Orientation = StackOrientation.Horizontal;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_currentCard);
            mainStack.Children.Add(otherStack);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            Button button;
            button = GetGamingButton("Years Passed", nameof(LifeCardGameMainViewModel.YearsPassedAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Play Card", nameof(LifeCardGameMainViewModel.PlayCardAsync));
            otherStack.Children.Add(button);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(LifeCardGameMainViewModel.OtherScreen))
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };
            otherStack.Children.Add(parent);



            _score.AddColumn("Cards Left", true, nameof(LifeCardGamePlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Points", true, nameof(LifeCardGamePlayerItem.Points));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(LifeCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(LifeCardGameMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            StackLayout finalStack = new StackLayout();
            finalStack.Orientation = StackOrientation.Horizontal;
            finalStack.Children.Add(mainStack);
            finalStack.Children.Add(_storyStack);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

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
                LifeHandXF thisHand = new LifeHandXF();
                thisHand.HandType = HandObservable<LifeCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 4;
                thisHand.HeightRequest = 900;
                thisHand.LoadList(thisPlayer.LifeStory!, "");
                thisPlayer.LifeStory!.ThisScroll = thisHand;
                _storyStack!.Children.Add(thisHand);
                _lifeList!.Add(thisHand);
            });

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
            return Task.CompletedTask;
        }
    }
}
