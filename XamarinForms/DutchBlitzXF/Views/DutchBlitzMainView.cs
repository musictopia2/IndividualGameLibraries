using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DutchBlitzCP.Cards;
using DutchBlitzCP.Data;
using DutchBlitzCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace DutchBlitzXF.Views
{
    public class DutchBlitzMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DutchBlitzVMData _model;
        private readonly BaseDeckXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF> _discardGPile;
        private readonly ScoreBoardXF _score;


        private readonly PublicPilesXF _public1 = new PublicPilesXF();
        private readonly BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF> _yourDiscard = new BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();
        private readonly BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF> _yourStock = new BasicMultiplePilesXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();


        public DutchBlitzMainView(IEventAggregator aggregator,
            TestOptions test,
            DutchBlitzVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(DutchBlitzMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(_public1);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Stock Left", false, nameof(DutchBlitzPlayerItem.StockLeft));
            _score.AddColumn("Points Round", false, nameof(DutchBlitzPlayerItem.PointsRound));
            _score.AddColumn("Points Game", false, nameof(DutchBlitzPlayerItem.PointsGame));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DutchBlitzMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DutchBlitzMainViewModel.Status));
            firstInfo.AddRow("Error", nameof(DutchBlitzMainViewModel.ErrorMessage));


            var button = GetGamingButton("Dutch", nameof(DutchBlitzMainViewModel.DutchAsync));
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_yourDiscard);
            otherStack.Children.Add(_yourStock);
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);

            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            DutchBlitzSaveInfo save = cons!.Resolve<DutchBlitzSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _public1.Init(_model.PublicPiles1!);
            _yourDiscard.Init(_model.DiscardPiles!, "");
            _yourStock.Init(_model.StockPile!.StockFrame, "");

            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {


            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {

            return Task.CompletedTask;
        }
    }
}
