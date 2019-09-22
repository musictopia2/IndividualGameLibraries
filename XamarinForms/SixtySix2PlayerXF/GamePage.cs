using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
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
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using SixtySix2PlayerCP;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SixtySix2PlayerXF
{
    public class GamePage : MultiPlayerPage<SixtySix2PlayerViewModel, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            SixtySix2PlayerSaveInfo saveRoot = OurContainer!.Resolve<SixtySix2PlayerSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed);
            _deckGPile.StartListeningMainDeck();
            _guide1.LoadList(ThisMod);
            _marriage1!.LoadList(ThisMod.Marriage1!, ts.TagUsed);
            var thisCard = new SixtySix2PlayerCardInformation();
            IProportionImage thisP = OurContainer.Resolve<IProportionImage>(ts.TagUsed);
            SKSize thisSize = thisCard.DefaultSize.GetSizeUsed(thisP.Proportion);
            var heights = thisSize.Height / 1.5f;
            _deckGPile.Margin = new Thickness(9, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            SixtySix2PlayerSaveInfo saveRoot = OurContainer!.Resolve<SixtySix2PlayerSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _marriage1!.UpdateList(ThisMod.Marriage1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>? _deckGPile;
        private BasePileXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>? _trick1;
        private SixtySix2PlayerMainGameClass? _mainGame;
        private readonly GuideUI _guide1 = new GuideUI();
        private BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>? _marriage1;
        private StackLayout? _deckStack;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<SixtySix2PlayerMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _discardGPile = new BasePileXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _marriage1 = new BaseHandXF<SixtySix2PlayerCardInformation, ts, DeckOfCardsXF<SixtySix2PlayerCardInformation>>();
            _deckStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_trick1);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(SixtySix2PlayerPlayerItem.ObjectCount));
            _thisScore.AddColumn("Tricks Won", true, nameof(SixtySix2PlayerPlayerItem.TricksWon));
            _thisScore.AddColumn("Score Round", true, nameof(SixtySix2PlayerPlayerItem.ScoreRound));
            _thisScore.AddColumn("Game Points Round", true, nameof(SixtySix2PlayerPlayerItem.GamePointsRound));
            _thisScore.AddColumn("Total Points Game", true, nameof(SixtySix2PlayerPlayerItem.GamePointsGame));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SixtySix2PlayerViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SixtySix2PlayerViewModel.Status));
            firstInfo.AddRow("Trump", nameof(SixtySix2PlayerViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Bonus", nameof(SixtySix2PlayerViewModel.BonusPoints));
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var thisBut = GetGamingButton("Go Out", nameof(SixtySix2PlayerViewModel.GoOutCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Announce Marriage", nameof(SixtySix2PlayerViewModel.AnnounceMarriageCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_guide1);
            thisStack.Children.Add(_marriage1);
            MainGrid!.Children.Add(thisStack); // forgot this too.
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