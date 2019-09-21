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
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using SixtySix2PlayerCP;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SixtySix2PlayerWPF
{
    public class GamePage : MultiPlayerWindow<SixtySix2PlayerViewModel, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            SixtySix2PlayerSaveInfo saveRoot = OurContainer!.Resolve<SixtySix2PlayerSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _guide1.LoadList(ThisMod);
            _marriage1!.LoadList(ThisMod.Marriage1!, ts.TagUsed);
            var thisCard = new SixtySix2PlayerCardInformation();
            IProportionImage thisP = OurContainer.Resolve<IProportionImage>(ts.TagUsed);
            SKSize thisSize = thisCard.DefaultSize.GetSizeUsed(thisP.Proportion);
            var heights = thisSize.Height / 1.5f;
            _deckGPile.Margin = new Thickness(6, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            SixtySix2PlayerSaveInfo saveRoot = OurContainer!.Resolve<SixtySix2PlayerSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _marriage1!.UpdateList(ThisMod.Marriage1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>? _deckGPile;
        private BasePileWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>? _trick1;
        private SixtySix2PlayerMainGameClass? _mainGame;
        private readonly GuideUI _guide1 = new GuideUI();
        private BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>? _marriage1;
        private StackPanel? _deckStack;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<SixtySix2PlayerMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _discardGPile = new BasePileWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _marriage1 = new BaseHandWPF<SixtySix2PlayerCardInformation, ts, DeckOfCardsWPF<SixtySix2PlayerCardInformation>>();
            _deckStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_trick1);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", true, nameof(SixtySix2PlayerPlayerItem.ObjectCount));
            _thisScore.AddColumn("Tricks Won", true, nameof(SixtySix2PlayerPlayerItem.TricksWon));
            _thisScore.AddColumn("Score Round", true, nameof(SixtySix2PlayerPlayerItem.ScoreRound));
            _thisScore.AddColumn("Game Points Round", true, nameof(SixtySix2PlayerPlayerItem.GamePointsRound));
            _thisScore.AddColumn("Total Points Game", true, nameof(SixtySix2PlayerPlayerItem.GamePointsGame));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SixtySix2PlayerViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SixtySix2PlayerViewModel.Status));
            firstInfo.AddRow("Trump", nameof(SixtySix2PlayerViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Bonus", nameof(SixtySix2PlayerViewModel.BonusPoints));
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            var thisBut = GetGamingButton("Go Out", nameof(SixtySix2PlayerViewModel.GoOutCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Announce Marriage", nameof(SixtySix2PlayerViewModel.AnnounceMarriageCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_marriage1);
            Grid FinalGrid = new Grid();
            AddLeftOverColumn(FinalGrid, 60);
            AddLeftOverColumn(FinalGrid, 40); // hopefully that works.
            AddControlToGrid(FinalGrid, thisStack, 0, 0);
            AddControlToGrid(FinalGrid, _guide1, 0, 1);
            MainGrid!.Children.Add(FinalGrid); // forgot this too.
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SixtySix2PlayerViewModel>();
            OurContainer!.RegisterType<DeckViewModel<SixtySix2PlayerCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<SixtySix2PlayerCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<SixtySix2PlayerCardInformation> ThisSort = new SortSimpleCards<SixtySix2PlayerCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>>();
        }
    }
}