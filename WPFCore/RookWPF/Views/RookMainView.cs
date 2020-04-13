using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RookCP.Cards;
using RookCP.Data;
using RookCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RookWPF.Views
{
    public class RookMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RookVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly SeveralPlayersTrickWPF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsWPF, RookPlayerItem> _trick1;
        private readonly BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF> _dummy1;

        public RookMainView(IEventAggregator aggregator,
            TestOptions test,
            RookVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF>();


            _trick1 = new SeveralPlayersTrickWPF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsWPF, RookPlayerItem>();
            _dummy1 = new BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(RookMainViewModel.RestoreScreen)
                };
            }
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);

            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(RookMainViewModel.NestScreen)
            };
            mainStack.Children.Add(parent);
            _score.AddColumn("Bid Amount", true, nameof(RookPlayerItem.BidAmount));
            _score.AddColumn("Tricks Won", false, nameof(RookPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(RookPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", false, nameof(RookPlayerItem.TotalScore));

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RookMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RookMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(RookMainViewModel.Status));


            mainStack.Children.Add(_playerHandWPF);

            mainStack.Children.Add(_dummy1);
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(RookMainViewModel.BidScreen)
            };
            otherStack.Children.Add(parent);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(RookMainViewModel.ColorScreen)
            };
            otherStack.Children.Add(parent);
            otherStack.Children.Add(_trick1);

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



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
