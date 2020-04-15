using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LottoDominosCP.Data;
using LottoDominosCP.Logic;
using LottoDominosCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;

namespace LottoDominosXF.Views
{
    public class LottoDominosMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly LottoDominosMainGameClass _mainGame;
        private readonly ScoreBoardXF _score1;
        public LottoDominosMainView(IEventAggregator aggregator,
            TestOptions test,
            LottoDominosMainGameClass mainGame
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _mainGame = mainGame;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(LottoDominosMainViewModel.RestoreScreen));
            }

            ParentSingleUIContainer choose = new ParentSingleUIContainer(nameof(LottoDominosMainViewModel.ChooseScreen));
            mainStack.Children.Add(choose);
            ParentSingleUIContainer board = new ParentSingleUIContainer(nameof(LottoDominosMainViewModel.BoardScreen));
            mainStack.Children.Add(board);

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(LottoDominosMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(LottoDominosMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            _score1 = new ScoreBoardXF();
            _score1.AddColumn("# Chosen", true, nameof(LottoDominosPlayerItem.NumberChosen));
            _score1.AddColumn("# Won", true, nameof(LottoDominosPlayerItem.NumberWon));
            mainStack.Children.Add(_score1);

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
            _score1!.LoadLists(_mainGame.SaveRoot!.PlayerList);
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
