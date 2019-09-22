using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using ThreeLetterFunCP;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace ThreeLetterFunXF
{
    public class AdvancedXF : ContentView
    {
        public void Init()
        {
            InputTransparent = false;
            StarterClass thisStart = Resolve<StarterClass>();
            var thisBind = new Binding(nameof(AdvancedOptionsViewModel.Visible));
            SetBinding(IsVisibleProperty, thisBind);
            ListChooserXF list1 = new ListChooserXF();
            ListChooserXF list2 = new ListChooserXF();
            list1.Orientation = StackOrientation.Horizontal;
            list2.Orientation = StackOrientation.Horizontal;
            list1.Margin = new Thickness(5, 5, 5, 5); // so its obvious its a second list
            double buttonFontSize;
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                list1.ItemWidth = 400; //well see when its on xamarin forms (lots of experimenting could be needed).
                list2.ItemWidth = 400;
                list1.ItemHeight = 50;
                list2.ItemHeight = 50;
                buttonFontSize = 75;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                list1.ItemWidth = 600; //well see when its on xamarin forms (lots of experimenting could be needed).
                list2.ItemWidth = 600;
                list1.ItemHeight = 90;
                list2.ItemHeight = 90; //has to experiment
                buttonFontSize = 150;
            }
            else
            {
                list1.ItemWidth = 800; //well see when its on xamarin forms (lots of experimenting could be needed).
                list2.ItemWidth = 800;
                list1.ItemHeight = 120;
                list2.ItemHeight = 120;
                buttonFontSize = 200;
            }

            if (thisStart.Advanced1 == null)
                thisStart.TestLoad();
            BindingContext = thisStart.Advanced1;
            // the bad news is the adjustments to make has to be multiplayer only (so lots of testing is involved)'
            list1.LoadLists(thisStart.Advanced1!.Easy1);
            list2.LoadLists(thisStart.Advanced1.Game1); // i think
            list2.Margin = new Thickness(5, 5, 5, 5);
            Grid grid = new Grid();
            AddLeftOverRow(grid, 50);
            AddLeftOverRow(grid, 50);
            AddAutoRows(grid, 1);
            AddControlToGrid(grid, list1, 0, 0);
            AddControlToGrid(grid, list2, 1, 0);
            var thisBut = GetGamingButton("Submit", nameof(AdvancedOptionsViewModel.SubmitCommand));
            thisBut.Margin = new Thickness(5, 5, 5, 5);
            AddControlToGrid(grid, thisBut, 2, 0);
            thisBut.FontSize = buttonFontSize;
            Content = grid;
        }
    }
}