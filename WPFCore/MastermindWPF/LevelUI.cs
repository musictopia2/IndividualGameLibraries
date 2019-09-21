using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using MastermindCP;
using System.Windows;
using System.Windows.Controls;
namespace MastermindWPF
{
    public class LevelUI : BaseFrameWPF
    {
        public void Init(MastermindViewModel thisMod)
        {
            Grid parentGrid = new Grid();
            ListChooserWPF thisCon = new ListChooserWPF();
            Text = "Level:";
            thisCon.ItemHeight = 60;
            thisCon.LoadLists(thisMod.LevelPicker!);
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisCon, thisRect);
            parentGrid.Children.Add(ThisDraw);
            parentGrid.Children.Add(thisCon);
            VerticalAlignment = VerticalAlignment.Top;
            Content = parentGrid;
        }
    }
}
