using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MastermindCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace MastermindXF.Views
{
    public class MastermindOpeningView : ContentView, IUIView
    {
        readonly ListChooserXF _picker;
        public MastermindOpeningView()
        {
            StackLayout stack = new StackLayout();
            Label label = GetDefaultLabel();
            label.FontSize = 50;
            label.Text = "Choose Level:";
            label.Margin = new Thickness(0, 10, 0, 10);
            stack.Children.Add(label);
            _picker = new ListChooserXF();
            _picker.Margin = new Thickness(5);

            if (ScreenUsed == EnumScreen.LargeTablet)
                _picker.ItemHeight = 60;
            else if (ScreenUsed == EnumScreen.SmallTablet)
                _picker.ItemHeight = 40;
            else
                _picker.ItemHeight = 25;

            stack.Children.Add(_picker);
            Button button = GetGamingButton("Level Information", nameof(MastermindOpeningViewModel.LevelInformationAsync));
            stack.Children.Add(button);
            Content = stack;
            //this could be another view model (?)

        }
        Task IUIView.TryActivateAsync()
        {
            MastermindOpeningViewModel model = (MastermindOpeningViewModel)BindingContext;
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
