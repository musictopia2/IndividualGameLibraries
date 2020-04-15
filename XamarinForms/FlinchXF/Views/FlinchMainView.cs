using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FlinchCP.Cards;
using FlinchCP.Data;
using FlinchCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FlinchXF.Views
{
    public class FlinchMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FlinchVMData _model;
        private readonly BaseDeckXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF> _playerHandWPF;
        private readonly PublicPilesXF _publicGraphics;
        public FlinchMainView(IEventAggregator aggregator,
            TestOptions test,
            FlinchVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<FlinchCardInformation, FlinchGraphicsCP, CardGraphicsXF>();
            _publicGraphics = new PublicPilesXF();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(FlinchMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_playerHandWPF);
            _score.AddColumn("In Stock", false, nameof(FlinchPlayerItem.InStock));
            int x;
            for (x = 1; x <= 5; x++) //has to change for flinch.
            {
                var thisStr = "Discard" + x;
                _score.AddColumn(thisStr, false, thisStr);
            }
            _score.AddColumn("Stock Left", false, nameof(FlinchPlayerItem.StockLeft));
            _score.AddColumn("Cards Left", false, nameof(FlinchPlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("RS Cards", nameof(FlinchMainViewModel.CardsToShuffle));
            firstInfo.AddRow("Turn", nameof(FlinchMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FlinchMainViewModel.Status));


            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 60);
            AddLeftOverColumn(tempGrid, 40);
            mainStack.Children.Add(_publicGraphics);
            mainStack.Children.Add(tempGrid);

            StackLayout tempStack = new StackLayout();
            tempStack.Children.Add(otherStack);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(FlinchMainViewModel.PlayerPilesScreen));

            tempStack.Children.Add(parent);
            AddControlToGrid(tempGrid, tempStack, 0, 0);


            Button endButton = GetGamingButton("End Turn", nameof(FlinchMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            tempStack.Children.Add(endButton);
            tempStack = new StackLayout();
            AddControlToGrid(tempGrid, tempStack, 0, 1);
            tempStack.Children.Add(_score);

            tempStack.Children.Add(firstInfo.GetContent);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


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

            FlinchSaveInfo save = cons!.Resolve<FlinchSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _publicGraphics!.Init(_model.PublicPiles!); // i think
            _publicGraphics.StartAnimationListener("public");
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
            _playerHandWPF.Dispose(); //at least will help improve performance
            return Task.CompletedTask;
        }
    }
}
