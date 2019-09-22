using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
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
using DutchBlitzCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace DutchBlitzWPF
{
    public class GamePage : MultiPlayerWindow<DutchBlitzViewModel, DutchBlitzPlayerItem, DutchBlitzSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
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
        private BaseDeckWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private readonly PublicPilesWPF _public1 = new PublicPilesWPF();
        private readonly BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF> _yourDiscard = new BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
        private readonly BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF> _yourStock = new BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            _public1.Width = 700;
            _public1.Height = 500;
            StackPanel otherStack = new StackPanel();
            thisStack.Children.Add(otherStack);
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_public1);
            otherStack.Children.Add(_thisScore);
            _thisScore.AddColumn("Stock Left", false, nameof(DutchBlitzPlayerItem.StockLeft));
            _thisScore.AddColumn("Points Round", false, nameof(DutchBlitzPlayerItem.PointsRound));
            _thisScore.AddColumn("Points Game", false, nameof(DutchBlitzPlayerItem.PointsGame));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Dutch", nameof(DutchBlitzViewModel.DutchCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DutchBlitzViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DutchBlitzViewModel.Status));
            firstInfo.AddRow("Error", nameof(DutchBlitzViewModel.ErrorMessage));
            otherStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_yourDiscard);
            otherStack.Children.Add(_yourStock);
            thisStack.Children.Add(otherStack);
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