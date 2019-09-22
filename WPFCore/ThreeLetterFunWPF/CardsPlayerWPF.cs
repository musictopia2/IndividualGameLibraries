using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using System.Windows.Controls;
using ThreeLetterFunCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunWPF
{
    public class CardsPlayerWPF : UserControl
    {
        public void Init()
        {
            StarterClass thisStart = Resolve<StarterClass>();
            var thisBind = GetVisibleBinding(nameof(CardsPlayerViewModel.Visible));
            SetBinding(VisibilityProperty, thisBind);
            ListChooserWPF thisList = new ListChooserWPF();
            thisList.Orientation = Orientation.Horizontal;
            thisList.ItemWidth = 400;
            thisList.ItemHeight = 100; // well see
            if (thisStart.Cards1 == null)
                thisStart.TestLoad();
            DataContext = thisStart.Cards1;
            thisList.LoadLists(thisStart.Cards1!.CardList1);
            StackPanel thisStack = new StackPanel();
            thisStack.Children.Add(thisList);
            var thisBut = GetGamingButton("Submit", nameof(CardsPlayerViewModel.SubmitCommand));
            thisBut.FontSize = 200; // can adjust as needed
            thisStack.Children.Add(thisBut);
            Content = thisStack;
        }
    }
}