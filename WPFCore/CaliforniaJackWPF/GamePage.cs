using AndyCristinaGamePackageCP.CommonProportionClasses;
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
using CaliforniaJackCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CaliforniaJackWPF
{
    public class GamePage : MultiPlayerWindow<CaliforniaJackViewModel, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            CaliforniaJackSaveInfo saveRoot = OurContainer!.Resolve<CaliforniaJackSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            CaliforniaJackSaveInfo saveRoot = OurContainer!.Resolve<CaliforniaJackSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>? _deckGPile;

        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>? _trick1;
        private CaliforniaJackMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<CaliforniaJackMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>();
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsWPF<CaliforniaJackCardInformation>>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(CaliforniaJackPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tricks Won", true, nameof(CaliforniaJackPlayerItem.TricksWon), rightMargin: 10);
            _thisScore.AddColumn("Points", true, nameof(CaliforniaJackPlayerItem.Points), rightMargin: 10);
            _thisScore.AddColumn("Total Score", true, nameof(CaliforniaJackPlayerItem.TotalScore), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(CaliforniaJackViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(CaliforniaJackViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(CaliforniaJackViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<CaliforniaJackViewModel>();
            OurContainer!.RegisterType<DeckViewModel<CaliforniaJackCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<CaliforniaJackCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<CaliforniaJackCardInformation> ThisSort = new SortSimpleCards<CaliforniaJackCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //change view model for area if not using 2 player.
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
            //this is the most common stuff.
        }
    }
}