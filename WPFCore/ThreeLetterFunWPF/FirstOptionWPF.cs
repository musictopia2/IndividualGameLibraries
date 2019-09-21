using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using System.Windows.Controls;
using ThreeLetterFunCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunWPF
{
    public class FirstOptionWPF : UserControl
    {
        public void Init()
        {
            StarterClass thisStart = Resolve<StarterClass>();
            var thisBind = GetVisibleBinding(nameof(CardsPlayerViewModel.Visible));
            SetBinding(VisibilityProperty, thisBind);
            ListChooserWPF thisList = new ListChooserWPF();
            thisList.ItemWidth = 700;
            thisList.ItemHeight = 200;
            if (thisStart.FirstOption1 == null)
                thisStart.TestLoad();
            DataContext = thisStart.FirstOption1;
            thisList.LoadLists(thisStart.FirstOption1!.Option1); // i think
            StackPanel thisStack = new StackPanel();
            thisStack.Children.Add(thisList);
            var thisBut = GetGamingButton("Submit", nameof(FirstOptionViewModel.SubmitCommand));
            thisBut.FontSize = 150;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Description", nameof(FirstOptionViewModel.DescriptionCommand));
            thisBut.FontSize = 150;
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            Content = thisStack;
        }
    }
}