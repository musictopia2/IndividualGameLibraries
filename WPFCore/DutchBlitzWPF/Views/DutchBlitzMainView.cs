using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using DutchBlitzCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using DutchBlitzCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using DutchBlitzCP.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;

namespace DutchBlitzWPF.Views
{
    public class DutchBlitzMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly DutchBlitzVMData _model;
        private readonly BaseDeckWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;

        private readonly PublicPilesWPF _public1 = new PublicPilesWPF();
        private readonly BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF> _yourDiscard = new BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
        private readonly BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF> _yourStock = new BasicMultiplePilesWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();


        public DutchBlitzMainView(IEventAggregator aggregator,
            TestOptions test,
            DutchBlitzVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<DutchBlitzCardInformation, DutchBlitzGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _public1.Width = 700;
            _public1.Height = 500;
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(DutchBlitzMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_public1);
            otherStack.Children.Add(_score);
            _score.AddColumn("Stock Left", false, nameof(DutchBlitzPlayerItem.StockLeft));
            _score.AddColumn("Points Round", false, nameof(DutchBlitzPlayerItem.PointsRound));
            _score.AddColumn("Points Game", false, nameof(DutchBlitzPlayerItem.PointsGame));
            mainStack.Children.Add(otherStack);



            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;


            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var button = GetGamingButton("Dutch", nameof(DutchBlitzMainViewModel.DutchAsync));
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);


            

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DutchBlitzMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DutchBlitzMainViewModel.Status));
            firstInfo.AddRow("Error", nameof(DutchBlitzMainViewModel.ErrorMessage));
            otherStack.Children.Add(firstInfo.GetContent);


            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;

            otherStack.Children.Add(_yourDiscard);
            otherStack.Children.Add(_yourStock);
            mainStack.Children.Add(otherStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

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
            return this.RefreshBindingsAsync(_aggregator);
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
