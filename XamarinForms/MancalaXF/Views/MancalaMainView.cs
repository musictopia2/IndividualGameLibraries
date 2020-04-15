using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MancalaCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;

namespace MancalaXF.Views
{
    public class MancalaMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageRegister _register;
        readonly GameBoardXF _board = new GameBoardXF();
        public MancalaMainView(IEventAggregator aggregator,
            TestOptions test,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _register = register;
            _register.RegisterControl(_board.Element, "");
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(MancalaMainViewModel.RestoreScreen));
            }

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MancalaMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MancalaMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(MancalaMainViewModel.Instructions));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_board);
            otherStack.Children.Add(firstInfo.GetContent);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            _board.LoadBoard();
            return Task.CompletedTask;
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
