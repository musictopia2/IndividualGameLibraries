using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using System.Windows;
using System.Windows.Controls;
using ThreeLetterFunCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunWPF
{
    public class AdvancedWPF : UserControl
    {
        public void Init()
        {
            StarterClass thisStart = Resolve<StarterClass>();
            var thisBind = GetVisibleBinding(nameof(AdvancedOptionsViewModel.Visible));
            SetBinding(VisibilityProperty, thisBind);
            ListChooserWPF list1 = new ListChooserWPF();
            ListChooserWPF list2 = new ListChooserWPF();
            list1.Orientation = Orientation.Horizontal;
            list2.Orientation = Orientation.Horizontal;
            list1.Margin = new Thickness(0, 0, 0, 0); // so its obvious its a second list
            list1.ItemWidth = 800; //well see when its on xamarin forms (lots of experimenting could be needed).
            list2.ItemWidth = 800;
            list1.ItemHeight = 150;
            list2.ItemHeight = 150;
            if (thisStart.Advanced1 == null)
                thisStart.TestLoad();
            DataContext = thisStart.Advanced1;
            // the bad news is the adjustments to make has to be multiplayer only (so lots of testing is involved)'
            list1.LoadLists(thisStart.Advanced1!.Easy1);
            list2.LoadLists(thisStart.Advanced1.Game1); // i think
            list2.Margin = new Thickness(10, 50, 0, 20);
            StackPanel thisStack = new StackPanel();
            thisStack.Children.Add(list1);
            thisStack.Children.Add(list2);
            var thisBut = GetGamingButton("Submit", nameof(AdvancedOptionsViewModel.SubmitCommand));
            thisBut.Margin = new Thickness(5, 50, 5, 5);
            thisBut.FontSize = 200;
            thisStack.Children.Add(thisBut);
            Content = thisStack;
        }
    }
}