using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GalaxyCardGameCP.Cards;
using GalaxyCardGameCP.Data;
using GalaxyCardGameCP.Logic;
using GalaxyCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GalaxyCardGameWPF.Views
{
    public class GalaxyCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, INewWinCard
    {
        private readonly IEventAggregator _aggregator;
        private readonly GalaxyCardGameVMData _model;
        private readonly GalaxyCardGameGameContainer _gameContainer;
        private readonly BaseDeckWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>> _trick1;

        private readonly DeckOfCardsWPF<GalaxyCardGameCardInformation> _nextCard;
        private readonly StackPanel _planetStack;
        private readonly Grid _moonGrid;

        public GalaxyCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            GalaxyCardGameVMData model,
            GalaxyCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _model.WinUI = this;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            gameContainer.SaveRoot.LoadWin(this);
            _deckGPile = new BaseDeckWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();
            _trick1.Width = 500;
            _nextCard = new DeckOfCardsWPF<GalaxyCardGameCardInformation>();
            _planetStack = new StackPanel();
            _moonGrid = new Grid();
            _planetStack.Orientation = Orientation.Horizontal;
            AddLeftOverColumn(_moonGrid, 50);
            AddLeftOverColumn(_moonGrid, 50);
            AddAutoRows(_moonGrid, 1);
            _nextCard.SendSize(ts.TagUsed, _gameContainer.SaveRoot!.WinningCard);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(GalaxyCardGameMainViewModel.RestoreScreen)
                };
            }
            var endButton = GetGamingButton("End Turn", nameof(GalaxyCardGameMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.VerticalAlignment = VerticalAlignment.Top;

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_nextCard);
            otherStack.Children.Add(_deckGPile);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(GalaxyCardGamePlayerItem.ObjectCount)); //very common.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(GalaxyCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GalaxyCardGameMainViewModel.Status));

            mainStack.Children.Add(_moonGrid);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_playerHandWPF);
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(firstInfo.GetContent);
            var button = GetGamingButton("Create New Moon", nameof(GalaxyCardGameMainViewModel.MoonAsync));
            finalStack.Children.Add(endButton);
            finalStack.Children.Add(button);
            otherStack.Children.Add(finalStack);
            mainStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_planetStack);
            otherStack.Children.Add(_score);
            mainStack.Children.Add(otherStack);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



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

            GalaxyCardGameSaveInfo save = cons!.Resolve<GalaxyCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);

            LoadPlayerControls();



            ShowNewCard();

            return this.RefreshBindingsAsync(_aggregator);
        }

        private void LoadPlayerControls()
        {
            _planetStack!.Children.Clear(); //hopefully this simple.
            _moonGrid!.Children.Clear();
            GalaxyCardGamePlayerItem thisPlayer;
            thisPlayer = _gameContainer!.PlayerList!.GetSelf();
            int myID = thisPlayer.Id;
            LoadSinglePlayer(thisPlayer);
            if (myID == 1)
                thisPlayer = _gameContainer.PlayerList[2];
            else
                thisPlayer = _gameContainer.PlayerList[1];
            LoadSinglePlayer(thisPlayer);
        }
        private void LoadSinglePlayer(GalaxyCardGamePlayerItem thisPlayer)
        {
            BaseHandWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>> thisPlanet = new BaseHandWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();
            thisPlanet.LoadList(thisPlayer.PlanetHand!, ts.TagUsed);
            thisPlanet.Margin = new Thickness(5, 5, 5, 5);
            _planetStack!.Children.Add(thisPlanet);
            MainRummySetsWPF<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>, MoonClass, SavedSet> thisMoon = new MainRummySetsWPF<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>, MoonClass, SavedSet>();
            thisMoon.Divider = 2;
            thisMoon.Init(thisPlayer.Moons!, ts.TagUsed);
            thisMoon.Margin = new Thickness(5, 5, 5, 5);
            thisMoon.HorizontalAlignment = HorizontalAlignment.Stretch;
            thisMoon.Height = 300; //bad news is on tablets, no source code to go by.
            AddControlToGrid(_moonGrid!, thisMoon, 0, _moonGrid!.Children.Count); //hopefully this works.
        }

        private void ShowNewCard()
        {
            _nextCard!.DataContext = null;
            if (_gameContainer!.SaveRoot!.WinningCard.Deck == 0)
                _nextCard.SendSize(ts.TagUsed, _gameContainer.SaveRoot.WinningCard);
            else
            {
                var thisCard = new GalaxyCardGameCardInformation();
                thisCard.Populate(_gameContainer.SaveRoot.WinningCard.Deck); //clone it.
                _nextCard.SendSize(ts.TagUsed, thisCard);
            }
        }
        void INewWinCard.ShowNewCard()
        {
            ShowNewCard();

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
