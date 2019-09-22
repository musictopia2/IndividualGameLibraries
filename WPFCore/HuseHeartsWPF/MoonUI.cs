using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using HuseHeartsCP;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace HuseHeartsWPF
{
    public class MoonUI : BaseFrameWPF
    {
        public void LoadLists()
        {
            StackPanel thisStack = new StackPanel();
            Text = "Shoot Moon Options";
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            Binding thisBind = GetVisibleBinding(nameof(HuseHeartsViewModel.MoonVisible));
            SetBinding(VisibilityProperty, thisBind);
            thisStack.Orientation = Orientation.Horizontal;
            var thisBut = GetGamingButton("Give Other Players 26 Points", nameof(HuseHeartsViewModel.MoonOptionsCommand));
            thisBut.CommandParameter = EnumMoonOptions.GiveEverybodyPlus;
            thisStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Reduce your score by 26 points", nameof(HuseHeartsViewModel.MoonOptionsCommand));
            thisBut.CommandParameter = EnumMoonOptions.GiveSelfMinus;
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}