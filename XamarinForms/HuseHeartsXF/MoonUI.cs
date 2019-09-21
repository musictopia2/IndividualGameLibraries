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
using Xamarin.Forms;
using HuseHeartsCP;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace HuseHeartsXF
{
    public class MoonUI : BaseFrameXF
    {
        public void LoadLists()
        {
            StackLayout thisStack = new StackLayout();
            Text = "Shoot Moon Options";
            SetUpMarginsOnParentControl(thisStack);
            Binding thisBind = new Binding(nameof(HuseHeartsViewModel.MoonVisible));
            SetBinding(IsVisibleProperty, thisBind);
            thisStack.Orientation = StackOrientation.Horizontal;
            var thisBut = GetSmallerButton($"Give Other{Constants.vbCrLf}Players 26 Points", nameof(HuseHeartsViewModel.MoonOptionsCommand));
            thisBut.CommandParameter = EnumMoonOptions.GiveEverybodyPlus;
            thisStack.Children.Add(thisBut);
            thisBut = GetSmallerButton($"Reduce your{Constants.vbCrLf}score by{Constants.vbCrLf}26 points", nameof(HuseHeartsViewModel.MoonOptionsCommand));
            thisBut.CommandParameter = EnumMoonOptions.GiveSelfMinus;
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}