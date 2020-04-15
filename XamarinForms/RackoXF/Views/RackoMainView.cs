using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RackoCP.Cards;
using RackoCP.Data;
using RackoCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RackoXF.Views
{
    public class RackoMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RackoVMData _model;
        private readonly BaseDeckXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF> _currentWPF;
        private readonly RackoGameContainer _gameContainer;
        private readonly RackoUI _handWPF; //use this instead.

        public RackoMainView(IEventAggregator aggregator,
            TestOptions test,
            RackoVMData model,
            RackoGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _deckGPile = new BaseDeckXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _currentWPF = new BasePileXF<RackoCardInformation, RackoGraphicsCP, CardGraphicsXF>();
            _handWPF = new RackoUI();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(RackoMainViewModel.RestoreScreen));
            }

            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 2);
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            _score.AddColumn("Score Round", true, nameof(RackoPlayerItem.ScoreRound));
            _score.AddColumn("Score Game", true, nameof(RackoPlayerItem.TotalScore));
            int x;
            for (x = 1; x <= 10; x++)
                _score.AddColumn("Section" + x, false, "Value" + x, nameof(RackoPlayerItem.CanShowValues));// 2 bindings.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RackoMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RackoMainViewModel.Status));
            var stack = new StackLayout();
            stack.Children.Add(_deckGPile);
            stack.Children.Add(_discardGPile); // can reposition or not even have as well.
            stack.Children.Add(_currentWPF);
            var thisBut = GetSmallerButton("Discard Current Card", nameof(RackoMainViewModel.DiscardCurrentAsync));
            stack.Children.Add(thisBut);

            stack.Children.Add(firstInfo.GetContent);
            thisBut = GetSmallerButton("Racko", nameof(RackoMainViewModel.RackoAsync));
            stack.Children.Add(thisBut);
            AddControlToGrid(finalGrid, stack, 0, 0);

            AddControlToGrid(finalGrid, _handWPF, 0, 1); // first column
            AddControlToGrid(finalGrid, _score, 1, 0);
            Grid.SetColumnSpan(_score, 2);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                stack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            RackoSaveInfo save = cons!.Resolve<RackoSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _currentWPF!.Init(_model.OtherPile!, "");
            _currentWPF.StartAnimationListener("otherpile");

            Content = finalGrid;
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
            RackoMainViewModel vm = (RackoMainViewModel)BindingContext;
            _handWPF!.Init(vm, _model, _gameContainer);


            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _handWPF.Dispose();
            return Task.CompletedTask;
        }
    }
}
