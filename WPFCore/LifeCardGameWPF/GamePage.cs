using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
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
using LifeCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace LifeCardGameWPF
{
    public class GamePage : MultiPlayerWindow<LifeCardGameViewModel, LifeCardGamePlayerItem, LifeCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            LifeCardGameSaveInfo saveRoot = OurContainer!.Resolve<LifeCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _currentCard!.Init(ThisMod.CurrentPile!, "");
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            thisList.ForEach(thisPlayer =>
            {
                LifeHandWPF thisHand = new LifeHandWPF();
                thisHand.HandType = HandViewModel<LifeCardGameCardInformation>.EnumHandList.Vertical;
                thisHand.Divider = 4;
                thisHand.Height = 900;
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
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
        private BaseDeckWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>? _currentCard;
        private CustomBasicList<LifeHandWPF>? _lifeList;
        private readonly StackPanel _storyStack = new StackPanel();
        private LifeCardGameMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<LifeCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _currentCard = new BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _lifeList = new CustomBasicList<LifeHandWPF>();
            _deckGPile = new BaseDeckWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<LifeCardGameCardInformation, LifeCardGameGraphicsCP, CardGraphicsWPF>();
            _storyStack.Orientation = Orientation.Horizontal;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_currentCard);
            thisStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            Button thisBut;
            thisBut = GetGamingButton("Years Passed", nameof(LifeCardGameViewModel.YearsPassedCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Play Card", nameof(LifeCardGameViewModel.PlayCardCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Other", nameof(LifeCardGameViewModel.OtherCommand));
            otherStack.Children.Add(thisBut);
            var thisBind = GetVisibleBinding(nameof(LifeCardGameViewModel.OtherVisible));
            thisBut.SetBinding(VisibilityProperty, thisBind);
            thisBind = new Binding(nameof(LifeCardGameViewModel.OtherText));
            thisBut.SetBinding(ContentProperty, thisBind);
            _thisScore.AddColumn("Cards Left", true, nameof(LifeCardGamePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Points", true, nameof(LifeCardGamePlayerItem.Points));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(LifeCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(LifeCardGameViewModel.Status));
            thisStack.Children.Add(_playerHandWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            StackPanel FinalStack = new StackPanel();
            FinalStack.Orientation = Orientation.Horizontal;
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