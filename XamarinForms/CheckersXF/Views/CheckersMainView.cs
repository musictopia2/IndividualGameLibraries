using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CheckersCP.Logic;
using CheckersCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace CheckersXF.Views
{
    public class CheckersMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageRegister _register;
        private readonly GameBoardXF _board = new GameBoardXF();
        public CheckersMainView(IEventAggregator aggregator,
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
                restoreP = new ParentSingleUIContainer(nameof(CheckersMainViewModel.RestoreScreen));
            }

            _register.RegisterControl(_board.Element, "main");
            graphics.LinkBoard(); //this is always needed now.
            _board.Margin = new Thickness(3);
            var endButton = GetGamingButton("End Turn", nameof(CheckersMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CheckersMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(CheckersMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(CheckersMainViewModel.Status));
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            tempStack.Children.Add(endButton);
            Button other = GetGamingButton("Show Tie", nameof(CheckersMainViewModel.TieAsync));
            tempStack.Children.Add(other);
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(tempStack);
            finalStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_board);
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
