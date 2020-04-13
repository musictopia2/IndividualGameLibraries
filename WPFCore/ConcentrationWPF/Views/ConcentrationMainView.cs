using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ConcentrationCP.Data;
using ConcentrationCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace ConcentrationWPF.Views
{
    public class ConcentrationMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ConcentrationVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _board;
        public ConcentrationMainView(IEventAggregator aggregator,
            TestOptions test,
            ConcentrationVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _score = new ScoreBoardWPF();
            _board = new BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ConcentrationMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_board);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Pairs", true, nameof(ConcentrationPlayerItem.Pairs)); //very common.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ConcentrationMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConcentrationMainViewModel.Status));

            StackPanel finalStack = new StackPanel();
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_score);
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            ConcentrationSaveInfo save = cons!.Resolve<ConcentrationSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _board.Init(_model.GameBoard1, ts.TagUsed);
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
