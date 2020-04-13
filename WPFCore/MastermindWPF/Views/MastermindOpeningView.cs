using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using System.Windows;
using MastermindCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace MastermindWPF.Views
{
    public class MastermindOpeningView : UserControl, IUIView
    {
        readonly ListChooserWPF _picker;
        public MastermindOpeningView()
        {
            StackPanel stack = new StackPanel();
            TextBlock label = GetDefaultLabel();
            label.FontSize = 50;
            label.Text = "Choose Level:";
            label.Margin = new Thickness(0, 10, 0, 10);
            stack.Children.Add(label);
            _picker = new ListChooserWPF();
            _picker.Margin = new Thickness(5);
            stack.Children.Add(_picker);
            Button button = GetGamingButton("Level Information", nameof(MastermindOpeningViewModel.LevelInformationAsync));
            stack.Children.Add(button);
            Content = stack;
            //this could be another view model (?)

        }
        Task IUIView.TryActivateAsync()
        {
            MastermindOpeningViewModel model = (MastermindOpeningViewModel)DataContext;
            _picker.LoadLists(model.LevelPicker);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}