using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FroggiesCP.Logic;
using FroggiesCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace FroggiesXF.Views
{
    public class FroggiesMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //private Button _button;
        public FroggiesMainView(IEventAggregator aggregator, FroggiesMainGameClass game)
        {
            //InputTransparent = true;
            //TestControl test = new TestControl();

            //Content = test;
            //GamePackageViewModelBinder.StopRun = true;
            //TestControl test = new TestControl();
            //Content = test;
            //_button = new Button()
            //{
            //    Text = "Main"
            //};
            //_button.Clicked += Button_Clicked;
            //Content = _button;
            aggregator.Subscribe(this);

            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 3);
            StackLayout stack = new StackLayout();
            GameBoardXF board = new GameBoardXF(aggregator, game, this);
            board.Margin = new Thickness(5, 5, 5, 5);
            stack.Margin = new Thickness(5, 5, 5, 5);

            AddControlToGrid(thisGrid, board, 0, 0);
            Button redoButton = GetGamingButton("Redo Game", nameof(FroggiesMainViewModel.RedoAsync));
            stack.Children.Add(redoButton);
            SimpleLabelGridXF thisLab = new SimpleLabelGridXF();
            thisLab.AddRow("Moves Left", nameof(FroggiesMainViewModel.MovesLeft));
            thisLab.AddRow("How Many Frogs Currently", nameof(FroggiesMainViewModel.NumberOfFrogs));
            thisLab.AddRow("How Many Frogs To Start", nameof(FroggiesMainViewModel.StartingFrogs));
            stack.Children.Add(thisLab.GetContent);
            AddControlToGrid(thisGrid, stack, 0, 1);
            TestControl test = new TestControl();
            AddControlToGrid(thisGrid, test, 0, 2);
            Content = thisGrid; //i think.
        }

        private void Button_Clicked(object sender, System.EventArgs e)
        {
            
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
            //IsEnabled = true;
            //_button = new Button()
            //{
            //    Text = "Main Test",
            //    IsEnabled = true
            //};
            //_button.Clicked += Button_Clicked;

            //StackLayout stack = new StackLayout();
            //stack.Children.Add(_button);
            //Content = stack;
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
