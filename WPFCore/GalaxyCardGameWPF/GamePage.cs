using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using GalaxyCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GalaxyCardGameWPF
{
    public class GamePage : MultiPlayerWindow<GalaxyCardGameViewModel, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>, INewWinCard
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            GalaxyCardGameSaveInfo saveRoot = OurContainer!.Resolve<GalaxyCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
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
        private BaseDeckWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>? _deckGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>? _trick1;
        private GalaxyCardGameMainGameClass? _mainGame;
        private DeckOfCardsWPF<GalaxyCardGameCardInformation>? _nextCard;
        private StackPanel? _planetStack;
        private Grid? _moonGrid;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<GalaxyCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, GalaxyCardGameCardInformation, ts, DeckOfCardsWPF<GalaxyCardGameCardInformation>>();
            _trick1.Width = 500;
            _nextCard = new DeckOfCardsWPF<GalaxyCardGameCardInformation>();
            _planetStack = new StackPanel();
            _moonGrid = new Grid();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _planetStack.Orientation = Orientation.Horizontal;
            AddLeftOverColumn(_moonGrid, 50);
            AddLeftOverColumn(_moonGrid, 50);
            AddAutoRows(_moonGrid, 1);
            ThisMod!.WinUI = this;
            _nextCard.SendSize(ts.TagUsed, _mainGame.SaveRoot!.WinningCard);
            var EndButton = GetGamingButton("End Turn", nameof(GalaxyCardGameViewModel.EndTurnCommand));
            EndButton.HorizontalAlignment = HorizontalAlignment.Left;
            EndButton.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_nextCard);
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(GalaxyCardGamePlayerItem.ObjectCount)); //very common.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(GalaxyCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GalaxyCardGameViewModel.Status));
            thisStack.Children.Add(_moonGrid);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_playerHandWPF);
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(firstInfo.GetContent);
            var ThisBut = GetGamingButton("Create New Moon", nameof(GalaxyCardGameViewModel.MoonCommand));
            finalStack.Children.Add(EndButton);
            finalStack.Children.Add(ThisBut);
            otherStack.Children.Add(finalStack);
            thisStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_planetStack);
            otherStack.Children.Add(_thisScore);
            thisStack.Children.Add(otherStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
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
            _nextCard!.DataContext = null;
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
        float IProportionImage.Proportion => 2.1f; //experiment.
    }
}