using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using ChessCP.Logic;
using ChessCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ChessXF.Views
{
    public class ChessMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageRegister _register;
        private readonly GameBoardXF _board = new GameBoardXF();
        public ChessMainView(IEventAggregator aggregator,
            TestOptions test, IGamePackageRegister register,
            GameBoardGraphicsCP graphics)
        {
            _aggregator = aggregator;
            _register = register;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ChessMainViewModel.RestoreScreen));
            }
            _register.RegisterControl(_board.Element, "main");
            graphics.LinkBoard(); //this is always needed now.
            _board.Margin = new Thickness(3);

            mainStack.Children.Add(_board);
            var endButton = GetGamingButton("End Turn", nameof(ChessMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            Button other = GetGamingButton("Show Tie", nameof(ChessMainViewModel.TieAsync));
            other.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ChessMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ChessMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(ChessMainViewModel.Status));
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(other);
            tempStack.Children.Add(firstInfo.GetContent);

            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(tempStack);
            finalStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(finalStack);

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
            _board.LoadBoard();
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
