using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.CollectionClasses; //just in case i want to use the new custom classes.
using CommonBasicStandardLibraries.Exceptions;
using MonopolyCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace MonopolyCardGameWPF
{
    public class GamePage : MultiPlayerWindow<MonopolyCardGameViewModel, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            MonopolyCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonopolyCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _cardDetail!.LoadControls();
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                MonopolyHandWPF thisHand = new MonopolyHandWPF();
                thisHand.HandType = HandViewModel<MonopolyCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 1.6;
                thisHand.Height = 900;
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
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
        private BaseDeckWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private readonly StackPanel _tradeStack = new StackPanel();
        private MonopolyCardGameMainGameClass? _mainGame;
        private CustomBasicList<MonopolyHandWPF>? _customList;
        private readonly ShowCardUI? _cardDetail = new ShowCardUI();
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<MonopolyCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<MonopolyCardGameCardInformation, MonopolyCardGameGraphicsCP, CardGraphicsWPF>();
            _customList = new CustomBasicList<MonopolyHandWPF>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            thisStack.Children.Add(_cardDetail);
            Grid finalGrid = new Grid();
            AddPixelColumn(finalGrid, 900);
            AddAutoColumns(finalGrid, 1);
            _tradeStack.Orientation = Orientation.Horizontal;
            AddControlToGrid(finalGrid, _tradeStack, 0, 1);
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            StackPanel tempStack = new StackPanel();
            var thisBut = GetGamingButton("Resume", nameof(MonopolyCardGameViewModel.ResumeCommand));
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Go Out", nameof(MonopolyCardGameViewModel.GoOutCommand));
            tempStack.Children.Add(thisBut);
            otherStack.Children.Add(tempStack);
            thisStack.Children.Add(otherStack);
            _thisScore.AddColumn("Cards Left", true, nameof(MonopolyCardGamePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Previous Money", true, nameof(MonopolyCardGamePlayerItem.PreviousMoney), useCurrency: true);
            _thisScore.AddColumn("Total Money", true, nameof(MonopolyCardGamePlayerItem.TotalMoney), useCurrency: true);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MonopolyCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonopolyCardGameViewModel.Status));
            thisStack.Children.Add(_playerHandWPF);
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
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion => 1.3f;
    }
}