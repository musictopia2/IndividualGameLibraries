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
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using FluxxCP;
using Xamarin.Forms;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace FluxxXF
{
    public class CardListUI : BaseFrameXF
    {
        public void LoadControls(ActionViewModel thisAction)
        {
            Text = "Card List";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack); //i think.
            ListChooserXF thisList = new ListChooserXF();
            thisList.LoadLists(thisAction.CardList1!); // i think
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Vertical;
            thisScroll.Content = thisList;
            thisScroll.HeightRequest = 500; // well see.
            thisStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(thisScroll);
            StackLayout finalStack = new StackLayout();
            thisStack.Children.Add(finalStack);
            var thisBut = GetGamingButton("Select Card", nameof(ActionViewModel.SelectCardCommand)); // i think
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("View Card", nameof(ActionViewModel.ViewCardCommand));
            finalStack.Children.Add(thisBut);
            thisBut = ActionUI.GetKeeperButton();
            finalStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}
