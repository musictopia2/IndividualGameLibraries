using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using LifeCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace LifeCardGameXF
{
    public class GamePage : MultiPlayerPage<LifeCardGameViewModel, LifeCardGamePlayerItem, LifeCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            LifeCardGameSaveInfo saveRoot = OurContainer!.Resolve<LifeCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                LifeHandXF thisHand = new LifeHandXF();
                thisHand.HandType = HandViewModel<LifeCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 4;
                thisHand.HeightRequest = 900; //iffy.
                thisHand.LoadList(thisPlayer.LifeStory!, "");
                thisPlayer.LifeStory!.ThisScroll = thisHand;
                _storyStack!.Children.Add(thisHand);
                _lifeList!.Add(thisHand);
            });
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            LifeCardGameSaveInfo saveRoot = OurContainer!.Resolve<LifeCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _currentCard!.UpdatePile(ThisMod.CurrentPile!);
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            int x = 0;
            if (_lifeList!.Count != thisList.Count)
                throw new BasicBlankException("Does not reconcile upon update");
            thisList.ForEach(thisPlayer =>
            {
                _lifeList[x].UpdateList(thisPlayer.LifeStory!);
                x++;
            });
            return Task.CompletedTask;
        }
        private BaseDeckXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>? _playerHand;
        private BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>? _currentCard;
        private CustomBasicList<LifeHandXF>? _lifeList;
        private readonly StackLayout _storyStack = new StackLayout();
        private LifeCardGameMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<LifeCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _currentCard = new BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _lifeList = new CustomBasicList<LifeHandXF>();
            _deckGPile = new BaseDeckXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsXF>();
            _storyStack.Orientation = StackOrientation.Horizontal;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_currentCard);
            thisStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            Button thisBut;
            thisBut = GetGamingButton("Years Passed", nameof(LifeCardGameViewModel.YearsPassedCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Play Card", nameof(LifeCardGameViewModel.PlayCardCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Other", nameof(LifeCardGameViewModel.OtherCommand));
            otherStack.Children.Add(thisBut);
            var thisBind = new Binding(nameof(LifeCardGameViewModel.OtherVisible));
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            thisBind = new Binding(nameof(LifeCardGameViewModel.OtherText));
            thisBut.SetBinding(ContentProperty, thisBind);
            _thisScore.AddColumn("Cards Left", true, nameof(LifeCardGamePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Points", true, nameof(LifeCardGamePlayerItem.Points));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(LifeCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(LifeCardGameViewModel.Status));
            thisStack.Children.Add(_playerHand);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            StackLayout FinalStack = new StackLayout();
            FinalStack.Orientation = StackOrientation.Horizontal;
            FinalStack.Children.Add(thisStack);
            FinalStack.Children.Add(_storyStack);
            MainGrid!.Children.Add(FinalStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _currentCard.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<LifeCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<LifeCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<LifeCardGamePlayerItem, LifeCardGameSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<LifeCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, LifeCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
        }
    }
}