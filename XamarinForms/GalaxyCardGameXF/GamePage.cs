using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.Exceptions;
using GalaxyCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GalaxyCardGameXF
{
    public class GamePage : MultiPlayerPage<GalaxyCardGameViewModel, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>, INewWinCard
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            GalaxyCardGameSaveInfo saveRoot = OurContainer!.Resolve<GalaxyCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            LoadPlayerControls();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            GalaxyCardGameSaveInfo saveRoot = OurContainer!.Resolve<GalaxyCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            LoadPlayerControls();
            return Task.CompletedTask;
        }
        private void LoadPlayerControls()
        {
            _planetStack!.Children.Clear(); //hopefully this simple.
            _moonGrid!.Children.Clear();
            GalaxyCardGamePlayerItem thisPlayer;
            thisPlayer = _mainGame!.PlayerList!.GetSelf();
            int myID = thisPlayer.Id;
            LoadSinglePlayer(thisPlayer);
            if (myID == 1)
                thisPlayer = _mainGame.PlayerList[2];
            else
                thisPlayer = _mainGame.PlayerList[1];
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
        private BaseDeckXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>? _deckGPile;

        private ScoreBoardXF? _thisScore;
        private BaseHandXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>? _trick1;
        private GalaxyCardGameMainGameClass? _mainGame;
        private DeckOfCardsXF<GalaxyCardGameCardInformation>? _nextCard;
        private StackLayout? _planetStack;
        private Grid? _moonGrid;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<GalaxyCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsXF<GalaxyCardGameCardInformation>>();
            _nextCard = new DeckOfCardsXF<GalaxyCardGameCardInformation>();
            _planetStack = new StackLayout();
            _moonGrid = new Grid();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _planetStack.Orientation = StackOrientation.Horizontal;
            AddLeftOverColumn(_moonGrid, 50);
            AddLeftOverColumn(_moonGrid, 50);
            AddAutoRows(_moonGrid, 1);
            ThisMod!.WinUI = this;
            _nextCard.SendSize(ts.TagUsed, _mainGame.SaveRoot!.WinningCard);
            var endButton = GetSmallerButton("End Turn", nameof(GalaxyCardGameViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_nextCard);
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(GalaxyCardGamePlayerItem.ObjectCount)); //this time, can't be rotated or not enough room.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GalaxyCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GalaxyCardGameViewModel.Status));
            thisStack.Children.Add(_moonGrid);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_playerHand);
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(firstInfo.GetContent);
            var thisBut = GetSmallerButton("Create New Moon", nameof(GalaxyCardGameViewModel.MoonCommand));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(finalStack);
            thisStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_planetStack);
            otherStack.Children.Add(_thisScore);
            thisStack.Children.Add(otherStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.VerticalOptions = LayoutOptions.Start;
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<GalaxyCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<GalaxyCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<GalaxyCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<GalaxyCardGameCardInformation> ThisSort = new SortSimpleCards<GalaxyCardGameCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, CustomSize>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>>();
        }
        void INewWinCard.ShowNewCard()
        {
            _nextCard!.BindingContext = null;
            if (_mainGame!.SaveRoot!.WinningCard.Deck == 0)
                _nextCard.SendSize(ts.TagUsed, _mainGame.SaveRoot.WinningCard);
            else
            {
                var thisCard = new GalaxyCardGameCardInformation();
                thisCard.Populate(_mainGame.SaveRoot.WinningCard.Deck); //clone it.
                _nextCard.SendSize(ts.TagUsed, thisCard);
            }
        }
    }
    public class CustomSize : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    throw new BasicBlankException("Phone not supported this time");
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 1.0f;
                return 1.2f; //can tweak as needed.
            }
        }
    }
}