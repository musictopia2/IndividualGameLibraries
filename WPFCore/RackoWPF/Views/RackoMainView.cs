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
using RackoCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using RackoCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using RackoCP.Cards;

namespace RackoWPF.Views
{
    public class RackoMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RackoVMData _model;
        private readonly RackoGameContainer _gameContainer;
        private readonly BaseDeckWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;

        private readonly BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF> _currentWPF;
        private readonly RackoUI _handWPF; //use this instead.

        public RackoMainView(IEventAggregator aggregator,
            TestOptions test,
            RackoVMData model,
            RackoGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();

            _currentWPF = new BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>();
            _handWPF = new RackoUI();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(RackoMainViewModel.RestoreScreen)
                };
            }
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1);
            AddAutoColumns(finalGrid, 2);
            mainStack.Children.Add(_deckGPile);
            mainStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(_currentWPF);
            var thisBut = GetGamingButton("Discard Current Card", nameof(RackoMainViewModel.DiscardCurrentAsync));
            mainStack.Children.Add(thisBut);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RackoMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RackoMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            thisBut = GetGamingButton("Racko", nameof(RackoMainViewModel.RackoAsync));
            mainStack.Children.Add(thisBut);
            AddControlToGrid(finalGrid, mainStack, 0, 0);
            _score.AddColumn("Score Round", true, nameof(RackoPlayerItem.ScoreRound));
            _score.AddColumn("Score Game", true, nameof(RackoPlayerItem.TotalScore));
            int x;
            for (x = 1; x <= 10; x++)
                _score.AddColumn("Section" + x, false, "Value" + x, nameof(RackoPlayerItem.CanShowValues));// 2 bindings.
            mainStack.Children.Add(_score);
            _handWPF = new RackoUI();
            AddControlToGrid(finalGrid, _handWPF, 0, 1); // first column

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            RackoSaveInfo save = cons!.Resolve<RackoSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            RackoMainViewModel vm = (RackoMainViewModel)DataContext;
            _handWPF!.Init(vm, _model, _gameContainer);
            _currentWPF!.Init(_model.OtherPile!, "");
            _currentWPF.StartAnimationListener("otherpile");

            return this.RefreshBindingsAsync(_aggregator);
        }

        

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _handWPF.Dispose();
            return Task.CompletedTask;
        }
    }
}
