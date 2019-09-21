using AndyCristinaGamePackageCP.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using ThreeLetterFunCP;
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunXF
{
    public class CardsPlayerXF : ContentView
    {
        public void Init()
        {
            InputTransparent = false;
            StarterClass thisStart = Resolve<StarterClass>();
            var thisBind = new Binding(nameof(CardsPlayerViewModel.Visible));
            SetBinding(IsVisibleProperty, thisBind);
            ListChooserXF thisList = new ListChooserXF();
            thisList.Orientation = StackOrientation.Horizontal;
            
            if (thisStart.Cards1 == null)
                thisStart.TestLoad();
            BindingContext = thisStart.Cards1;
            
            StackLayout thisStack = new StackLayout();
            thisStack.Children.Add(thisList);
            var thisBut = GetGamingButton("Submit", nameof(CardsPlayerViewModel.SubmitCommand));
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                thisList.ItemWidth = 155;
                thisList.ItemHeight = 35; // well see
                thisBut.FontSize = 75; // can adjust as needed
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                thisList.ItemWidth = 200;
                thisList.ItemHeight = 50; // well see
                thisBut.FontSize = 150; // can adjust as needed
            }
            else
            {
                thisList.ItemWidth = 400;
                thisList.ItemHeight = 80; // well see
                thisBut.FontSize = 200; // can adjust as needed
            }
            thisList.LoadLists(thisStart.Cards1!.CardList1); //hopefully can have here instead (?)
            thisStack.Children.Add(thisBut);
            Content = thisStack;
        }
    }
}