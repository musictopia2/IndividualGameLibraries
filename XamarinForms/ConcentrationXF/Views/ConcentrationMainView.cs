using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ConcentrationCP.Data;
using ConcentrationCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace ConcentrationXF.Views
{
    public class ConcentrationMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ConcentrationVMData _model;

        private readonly ScoreBoardXF _score;
        private readonly BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _board;
        public ConcentrationMainView(IEventAggregator aggregator,
            TestOptions test,
            ConcentrationVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _board = new BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _score = new ScoreBoardXF();
            _score.AddColumn("Pairs", true, nameof(ConcentrationPlayerItem.Pairs)); //very common.
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ConcentrationMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_board);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", false, nameof(ConcentrationPlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ConcentrationMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConcentrationMainViewModel.Status));

            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_score);

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

            ConcentrationSaveInfo save = cons!.Resolve<ConcentrationSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _board.Init(_model.GameBoard1, ts.TagUsed);
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
