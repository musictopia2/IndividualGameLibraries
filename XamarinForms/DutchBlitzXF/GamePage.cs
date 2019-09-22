using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
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
using DutchBlitzCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace DutchBlitzXF
{
    public class GamePage : MultiPlayerPage<DutchBlitzViewModel, DutchBlitzPlayerItem, DutchBlitzSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            DutchBlitzSaveInfo saveRoot = OurContainer!.Resolve<DutchBlitzSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _public1.Init(ThisMod.PublicPiles1!);
            _yourDiscard.Init(ThisMod.DiscardPiles!, "");
            _yourStock.Init(ThisMod.StockPile!.StockFrame, "");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DutchBlitzSaveInfo saveRoot = OurContainer!.Resolve<DutchBlitzSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _public1.UpdateLists(ThisMod.PublicPiles1!);
            _yourDiscard.UpdateLists(ThisMod.DiscardPiles!);
            _yourStock.UpdateLists(ThisMod.StockPile!.StockFrame);
            return Task.CompletedTask;
        }
        private BaseDeckXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private readonly PublicPilesXF _public1 = new PublicPilesXF();
        private readonly BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF> _yourDiscard = new BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();
        private readonly BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF> _yourStock = new BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();

        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(_public1);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Stock Left", false, nameof(DutchBlitzPlayerItem.StockLeft));
            _thisScore.AddColumn("Points Round", false, nameof(DutchBlitzPlayerItem.PointsRound));
            _thisScore.AddColumn("Points Game", false, nameof(DutchBlitzPlayerItem.PointsGame));
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_yourDiscard);
            otherStack.Children.Add(_yourStock);
            var thisBut = GetGamingButton("Dutch", nameof(DutchBlitzViewModel.DutchCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Error", nameof(DutchBlitzViewModel.ErrorMessage));
            firstInfo.AddRow("Turn", nameof(DutchBlitzViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DutchBlitzViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<DutchBlitzViewModel>();
            OurContainer!.RegisterType<DeckViewModel<DutchBlitzCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<DutchBlitzPlayerItem, DutchBlitzSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<DutchBlitzCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, DutchBlitzDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
        }
    }
}