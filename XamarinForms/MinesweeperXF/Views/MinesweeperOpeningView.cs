using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MinesweeperCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MinesweeperXF.Views
{
    public class MinesweeperOpeningView : ContentView, IUIView
    {
        readonly ListChooserXF _picker;
        public MinesweeperOpeningView()
        {
            //this is the opening part.
            //only 2 parts needed are:
            //1.  listview control
            //2.  mines needed

            StackLayout stack = new StackLayout();
            _picker = new ListChooserXF();
            _picker.Margin = new Thickness(5);
            stack.Children.Add(_picker);
            SimpleLabelGridXF thisLab = new SimpleLabelGridXF();
            thisLab.AddRow("Mines Needed", nameof(MinesweeperOpeningViewModel.HowManyMinesNeeded));
            stack.Children.Add(thisLab.GetContent);
            Content = stack;
        }

        Task IUIView.TryActivateAsync()
        {
            MinesweeperOpeningViewModel model = (MinesweeperOpeningViewModel)BindingContext;
            _picker.LoadLists(model.LevelPicker);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }

        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

    }
}
