using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
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
using GermanWhistCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GermanWhistXF
{
    public class GamePage : MultiPlayerPage<GermanWhistViewModel, GermanWhistPlayerItem, GermanWhistSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            GermanWhistSaveInfo saveRoot = OurContainer!.Resolve<GermanWhistSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            GermanWhistSaveInfo saveRoot = OurContainer!.Resolve<GermanWhistSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>? _deckGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>? _trick1;
        private GermanWhistMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<GermanWhistMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, GermanWhistCardInformation, ts, DeckOfCardsXF<GermanWhistCardInformation>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(GermanWhistPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tricks Won", true, nameof(GermanWhistPlayerItem.TricksWon), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(GermanWhistViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(GermanWhistViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(GermanWhistViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<GermanWhistViewModel>();
            OurContainer!.RegisterType<DeckViewModel<GermanWhistCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<GermanWhistPlayerItem, GermanWhistSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<GermanWhistCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<GermanWhistCardInformation> ThisSort = new SortSimpleCards<GermanWhistCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem, GermanWhistSaveInfo>>();
        }
    }
}