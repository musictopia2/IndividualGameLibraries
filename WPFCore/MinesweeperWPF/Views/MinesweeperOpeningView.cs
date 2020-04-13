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
using MinesweeperCP.ViewModels;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
//i think this is the most common things i like to do
namespace MinesweeperWPF.Views
{
    public class MinesweeperOpeningView : UserControl, IUIView
    {
        readonly ListChooserWPF _picker;
        public MinesweeperOpeningView()
        {
            //this is the opening part.
            //only 2 parts needed are:
            //1.  listview control
            //2.  mines needed

            StackPanel stack = new StackPanel();
            _picker = new ListChooserWPF();
            _picker.Margin = new Thickness(5);
            stack.Children.Add(_picker);
            SimpleLabelGrid thisLab = new SimpleLabelGrid();
            thisLab.AddRow("Mines Needed", nameof(MinesweeperOpeningViewModel.HowManyMinesNeeded));
            stack.Children.Add(thisLab.GetContent);
            Content = stack;
        }

        Task IUIView.TryActivateAsync()
        {
            MinesweeperOpeningViewModel model = (MinesweeperOpeningViewModel)DataContext;
            _picker.LoadLists(model.LevelPicker);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
