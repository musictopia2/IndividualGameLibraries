using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MahJongSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace MahJongSolitaireXF.Views
{
    public class MahJongSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        public MahJongSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);


            Grid grid = new Grid();
            AddLeftOverColumn(grid, 75);
            AddLeftOverColumn(grid, 25);

            StackLayout stack = new StackLayout();
            stack.Margin = new Thickness(5);
            GameBoardXF thisControl = new GameBoardXF(aggregator);
            thisControl.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(grid, thisControl, 0, 0);
            AddControlToGrid(grid, stack, 0, 1);

            var thisMove = GetGamingButton("Undo Move", nameof(MahJongSolitaireMainViewModel.UndoMove));
            stack.Children.Add(thisMove);
            var thisHint = GetGamingButton("Hint", nameof(MahJongSolitaireMainViewModel.GetHint));
            stack.Children.Add(thisHint);

            Grid.SetColumnSpan(thisControl, 2);
            SimpleLabelGridXF thisHelps = new SimpleLabelGridXF();
            thisHelps.AddRow("Tiles Gone", nameof(MahJongSolitaireMainViewModel.TilesGone));
            var otherGrid = thisHelps.GetContent;
            stack.Children.Add(otherGrid);
            Content = grid;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
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