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
using GalaxyCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using GalaxyCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using GalaxyCardGameCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using GalaxyCardGameCP.Logic;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace GalaxyCardGameXF.Views
{
    public class GalaxyCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, INewWinCard
    {
        private readonly IEventAggregator _aggregator;
        private readonly GalaxyCardGameVMData _model;
        private readonly GalaxyCardGameGameContainer _gameContainer;
        private readonly BaseDeckXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickXF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>> _trick1;

        private readonly DeckOfCardsXF<GalaxyCardGameCardInformation> _nextCard;
        private readonly StackLayout _planetStack;
        private readonly Grid _moonGrid;

        public GalaxyCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            GalaxyCardGameVMData model,
            GalaxyCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            gameContainer.SaveRoot.LoadWin(this);
            _deckGPile = new BaseDeckXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            _nextCard = new DeckOfCardsXF<GalaxyCardGameCardInformation>();
            _planetStack = new StackLayout();
            _moonGrid = new Grid();
            _planetStack.Orientation = StackOrientation.Horizontal;
            AddLeftOverColumn(_moonGrid, 50);
            AddLeftOverColumn(_moonGrid, 50);
            AddAutoRows(_moonGrid, 1);

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(GalaxyCardGameMainViewModel.RestoreScreen));
            }

            _score.AddColumn("Cards Left", false, nameof(GalaxyCardGamePlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GalaxyCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GalaxyCardGameMainViewModel.Status));

            var endButton = GetGamingButton("End Turn", nameof(GalaxyCardGameMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_nextCard);
            otherStack.Children.Add(_deckGPile);
            mainStack.Children.Add(otherStack);

            mainStack.Children.Add(_moonGrid);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_playerHandWPF);
            //StackLayout finalStack = new StackLayout();
            //finalStack.Children.Add(firstInfo.GetContent);
            var thisBut = GetSmallerButton("Create New Moon", nameof(GalaxyCardGameMainViewModel.MoonAsync));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            otherStack.Children.Add(thisBut);
            //otherStack.Children.Add(finalStack);
            mainStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_planetStack);
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.VerticalOptions = LayoutOptions.Start;



            if (restoreP != null)
            {
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
            BaseHandXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>> thisPlanet = new BaseHandXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            thisPlanet.LoadList(thisPlayer.PlanetHand!, ts.TagUsed);
            thisPlanet.Margin = new Thickness(5, 5, 5, 5);
            _planetStack!.Children.Add(thisPlanet);
            MainRummySetsXF<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>, MoonClass, SavedSet> thisMoon = new MainRummySetsXF<EnumSuitList, EnumColorList, GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>, MoonClass, SavedSet>();
            thisMoon.Divider = 2;
            thisMoon.Init(thisPlayer.Moons!, ts.TagUsed);
            thisMoon.Margin = new Thickness(5, 5, 5, 5);
            thisMoon.HorizontalOptions = LayoutOptions.Fill; //lots of options here.
            if (ScreenUsed == EnumScreen.LargeTablet)
                thisMoon.HeightRequest = 300; //bad news is on tablets, no source code to go by.
            else
                thisMoon.HeightRequest = 150; //well see.
            AddControlToGrid(_moonGrid!, thisMoon, 0, _moonGrid!.Children.Count); //hopefully this works.
        }

        private void ShowNewCard()
        {
            _nextCard!.BindingContext = null;
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
