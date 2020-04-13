using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MahJongSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace MahJongSolitaireWPF.Views
{
    public class MahJongSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        public MahJongSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);


            Grid grid = new Grid();
            AddLeftOverColumn(grid, 1);
            AddAutoColumns(grid, 1);

            StackPanel stack = new StackPanel();
            stack.Margin = new Thickness(5);
            GameBoardWPF thisControl = new GameBoardWPF(aggregator);
            thisControl.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(grid, thisControl, 0, 0);
            AddControlToGrid(grid, stack, 0, 1);

            var thisMove = GetGamingButton("Undo Move", nameof(MahJongSolitaireMainViewModel.UndoMove));
            stack.Children.Add(thisMove);
            var thisHint = GetGamingButton("Hint", nameof(MahJongSolitaireMainViewModel.GetHint));
            stack.Children.Add(thisHint);

            Grid.SetColumnSpan(thisControl, 2);
            SimpleLabelGrid thisHelps = new SimpleLabelGrid();
            thisHelps.AddRow("Tiles Gone", nameof(MahJongSolitaireMainViewModel.TilesGone));
            var otherGrid = thisHelps.GetContent;
            stack.Children.Add(otherGrid);
            Content = grid;
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
