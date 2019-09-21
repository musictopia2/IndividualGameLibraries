using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using CrazyEightsCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CrazyEightsXF
{
    public class GamePage : MultiPlayerPage<CrazyEightsViewModel, CrazyEightsPlayerItem, CrazyEightsSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            CrazyEightsSaveInfo saveRoot = OurContainer!.Resolve<CrazyEightsSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _thisSuit!.LoadLists(ThisMod.SuitChooser!);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            CrazyEightsSaveInfo saveRoot = OurContainer!.Resolve<CrazyEightsSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _deckGPile;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _discardGPile;
        private EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList, SuitListChooser>? _thisSuit;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _playerHand;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _thisSuit = new EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList, SuitListChooser>();

            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            Grid tempgrid = new Grid();
            AddAutoRows(tempgrid, 1);
            AddAutoColumns(tempgrid, 1);
            AddLeftOverColumn(tempgrid, 1);
            AddControlToGrid(tempgrid, otherStack, 0, 0);
            AddControlToGrid(tempgrid, _playerHand, 0, 1);
            thisStack.Children.Add(tempgrid);

            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", false, nameof(CrazyEightsPlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CrazyEightsViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CrazyEightsViewModel.Status));

            MainGrid!.Children.Add(thisStack);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            _thisSuit.GraphicsHeight = 200;
            _thisSuit.GraphicsWidth = 200; //i have to set both manually.  hopefully okay even on phone (?)
            thisStack.Children.Add(_thisSuit);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<CrazyEightsPlayerItem, CrazyEightsSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<CrazyEightsViewModel, RegularSimpleCard>();

        }
    }
}