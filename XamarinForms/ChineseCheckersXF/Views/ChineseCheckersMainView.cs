using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using ChineseCheckersCP.Logic;
using ChineseCheckersCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ChineseCheckersXF.Views
{
    public class ChineseCheckersMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        public ChineseCheckersMainView(IEventAggregator aggregator,
            TestOptions test, GameBoardGraphicsCP tempBoard, IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ChineseCheckersMainViewModel.RestoreScreen));
            }
            register.RegisterControl(_board.ThisElement, "main");
            tempBoard.LinkBoard(); //i think
            _board.HorizontalOptions = LayoutOptions.Start;
            _board.VerticalOptions = LayoutOptions.Start;
            mainStack.Children.Add(_board);


            var endButton = GetGamingButton("End Turn", nameof(ChineseCheckersMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ChineseCheckersMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ChineseCheckersMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(ChineseCheckersMainViewModel.Status));


            mainStack.Children.Add(endButton);
            mainStack.Children.Add(firstInfo.GetContent);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            _board.LoadBoard();
            return this.RefreshBindingsAsync(_aggregator); //try this way
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
