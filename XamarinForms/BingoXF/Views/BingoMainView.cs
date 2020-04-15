using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BingoCP.Data;
using BingoCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Collections;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace BingoXF.Views
{
    public class BingoMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly BingoBoardXF _board = new BingoBoardXF();

        public BingoMainView(IEventAggregator aggregator,
            TestOptions test,
            BingoSaveInfo saveInfo
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(BingoMainViewModel.RestoreScreen));
            }

            _board.Margin = new Thickness(5);
            SimpleLabelGridXF secondInfo = new SimpleLabelGridXF();
            secondInfo.FontSize = 30;
            secondInfo.AddRow("Number Called", nameof(BingoMainViewModel.NumberCalled));
            mainStack.Children.Add(secondInfo.GetContent);
            mainStack.Children.Add(_board);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Status", nameof(BingoMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            var endButton = GetGamingButton("Bingo", nameof(BingoMainViewModel.BingoAsync)); // its bingo instead.
            endButton.HorizontalOptions = LayoutOptions.Start;
            mainStack.Children.Add(endButton);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _board!.CreateControls(saveInfo.BingoBoard);
            Content = mainStack;


        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
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
