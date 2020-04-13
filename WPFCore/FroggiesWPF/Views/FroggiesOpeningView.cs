using BasicGameFrameworkLibrary.Attributes;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FroggiesCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace FroggiesWPF.Views
{
    public class FroggiesOpeningView : UserControl, IUIView
    {

        private readonly NumberChooserWPF _picker;
        public FroggiesOpeningView()
        {
            _picker = new NumberChooserWPF();
            _picker.Columns = 15; //we can increase (?)
            StackPanel stack = new StackPanel();
            TextBlock label = GetDefaultLabel();
            label.FontSize = 50;
            label.Text = "Choose How Many Frogs:";
            label.Margin = new Thickness(0, 10, 0, 10);
            stack.Children.Add(label);
            _picker.Margin = new Thickness(5);
            stack.Children.Add(_picker);
            Content = stack;
        }

        Task IUIView.TryActivateAsync()
        {
            FroggiesOpeningViewModel model = (FroggiesOpeningViewModel)DataContext;
            _picker.LoadLists(model.LevelPicker);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
