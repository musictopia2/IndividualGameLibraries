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
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using FluxxCP;
using System.Windows.Controls;
using SkiaSharp;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using System.Windows;
namespace FluxxWPF
{
    public class DirectionUI : BaseFrameWPF
    {
        public void LoadList(ActionViewModel actionMod)
        {
            Text = "Direction";
            StackPanel thisStack = new StackPanel();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect); //i think.
            ListChooserWPF thisList = new ListChooserWPF();
            thisList.ItemHeight = 60;
            thisList.LoadLists(actionMod.Direction1!);
            thisStack.Children.Add(thisList);
            var thisBut = GetGamingButton("Choose Direction", nameof(ActionViewModel.DirectionCommand));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            thisBut.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(thisBut);
            thisBut = ActionUI.GetKeeperButton();
            otherStack.Children.Add(thisBut);
            Grid ThisGrid = new Grid();
            ThisGrid.Children.Add(ThisDraw);
            ThisGrid.Children.Add(thisStack);
            Content = ThisGrid;
        }
    }
}