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
using MonopolyCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace MonopolyCardGameXF
{
    public class GamePage : MultiPlayerPage<MonopolyCardGameViewModel, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            MonopolyCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonopolyCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _cardDetail!.LoadControls();
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                MonopolyHandXF thisHand = new MonopolyHandXF();
                thisHand.HandType = HandViewModel<MonopolyCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 1.6;
                thisHand.HeightRequest = 900;
                thisHand.LoadList(thisPlayer.TradePile!, "");
                thisPlayer.TradePile!.ThisScroll = thisHand;
                _tradeStack.Children.Add(thisHand);
                _customList!.Add(thisHand);
            });
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            MonopolyCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonopolyCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            int x = 0;
            if (_customList!.Count != thisList.Count)
                throw new BasicBlankException("Does not reconcile upon update");
            thisList.ForEach(thisPlayer =>
            {
                _customList[x].UpdateList(thisPlayer.TradePile!);
                x++;
            });
            return Task.CompletedTask;
        }
        private BaseDeckXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>? _deckGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>? _playerHand;
        private readonly StackLayout _tradeStack = new StackLayout();
        private MonopolyCardGameMainGameClass? _mainGame;
        private CustomBasicList<MonopolyHandXF>? _customList;
        private readonly ShowCardUI? _cardDetail = new ShowCardUI();
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<MonopolyCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsXF>();
            _customList = new CustomBasicList<MonopolyHandXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            thisStack.Children.Add(_cardDetail);
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            _tradeStack.Orientation = StackOrientation.Horizontal;
            AddControlToGrid(finalGrid, _tradeStack, 0, 1);
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            StackLayout tempStack = new StackLayout();
            var thisBut = GetGamingButton("Resume", nameof(MonopolyCardGameViewModel.ResumeCommand));
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Go Out", nameof(MonopolyCardGameViewModel.GoOutCommand));
            tempStack.Children.Add(thisBut);
            otherStack.Children.Add(tempStack);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Cards Left", true, nameof(MonopolyCardGamePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Previous Money", true, nameof(MonopolyCardGamePlayerItem.PreviousMoney), useCurrency: true);
            _thisScore.AddColumn("Total Money", true, nameof(MonopolyCardGamePlayerItem.TotalMoney), useCurrency: true);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MonopolyCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonopolyCardGameViewModel.Status));
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            MainGrid!.Children.Add(finalGrid);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MonopolyCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<MonopolyCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<MonopolyCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, MonopolyCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            //anything else that needs to be registered will be here.

        }
    }
}