using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RookCP.Cards;
using RookCP.Data;
using RookCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RookXF.Views
{
    public class RookMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RookVMData _model;
        private readonly BaseDeckXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly SeveralPlayersTrickXF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsXF, RookPlayerItem> _trick1;
        private readonly BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF> _dummy1;
        public RookMainView(IEventAggregator aggregator,
            TestOptions test,
            RookVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>();
            _trick1 = new SeveralPlayersTrickXF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsXF, RookPlayerItem>();
            _dummy1 = new BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(RookMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);

            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(RookMainViewModel.NestScreen));
            mainStack.Children.Add(parent);
            _score.AddColumn("Bid Amount", true, nameof(RookPlayerItem.BidAmount));
            _score.AddColumn("Tricks Won", false, nameof(RookPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(RookPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", false, nameof(RookPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RookMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RookMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(RookMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(_dummy1);
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            parent = new ParentSingleUIContainer(nameof(RookMainViewModel.BidScreen));
            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer(nameof(RookMainViewModel.ColorScreen));
            otherStack.Children.Add(parent);
            otherStack.Children.Add(_trick1);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            RookSaveInfo save = cons!.Resolve<RookSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, "");
            _dummy1!.LoadList(_model.Dummy1!, "");
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
            _trick1.Dispose();
            _playerHandWPF.Dispose(); //at least will help improve performance.
            return Task.CompletedTask;
        }
    }
}
