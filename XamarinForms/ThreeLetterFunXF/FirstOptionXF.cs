using AndyCristinaGamePackageCP.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using ThreeLetterFunCP;
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunXF
{
    public class FirstOptionXF : ContentView
    {
        public void Init()
        {
            InputTransparent = false;
            StarterClass thisStart = Resolve<StarterClass>();
            var thisBind = new Binding(nameof(CardsPlayerViewModel.Visible));
            SetBinding(IsVisibleProperty, thisBind);
            ListChooserXF thisList = new ListChooserXF();
            thisList.HorizontalOptions = LayoutOptions.Start;
            thisList.VerticalOptions = LayoutOptions.Start;
            if (thisStart.FirstOption1 == null)
                thisStart.TestLoad();
            BindingContext = thisStart.FirstOption1;
            StackLayout thisStack = new StackLayout();
            thisStack.Children.Add(thisList);
            var thisBut = GetGamingButton("Submit", nameof(FirstOptionViewModel.SubmitCommand));
            double buttonFontSize;
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                thisList.ItemWidth = 400;
                thisList.ItemHeight = 100;
                buttonFontSize = 50;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                thisList.ItemWidth = 550;
                thisList.ItemHeight = 150;
                buttonFontSize = 75;
            }
            else
            {
                thisList.ItemWidth = 800;
                thisList.ItemHeight = 175;
                buttonFontSize = 125;
            }
            thisBut.FontSize = buttonFontSize;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Description", nameof(FirstOptionViewModel.DescriptionCommand));
            thisBut.FontSize = buttonFontSize;
            thisList.LoadLists(thisStart.FirstOption1!.Option1); // i think
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            Content = thisStack;
        }
    }
}