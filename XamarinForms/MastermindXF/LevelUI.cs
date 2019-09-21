using AndyCristinaGamePackageCP.DataClasses;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using MastermindCP;
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
namespace MastermindXF
{
    public class LevelUI : BaseFrameXF
    {
        public void Init(MastermindViewModel thisMod)
        {
            Grid parentGrid = new Grid();
            ListChooserXF thisCon = new ListChooserXF();
            Text = "Level:";
            if (ScreenUsed == EnumScreen.LargeTablet)
                thisCon.ItemHeight = 60;
            else if (ScreenUsed == EnumScreen.SmallTablet)
                thisCon.ItemHeight = 40;
            else
                thisCon.ItemHeight = 25;
            thisCon.LoadLists(thisMod.LevelPicker!);
            SetUpMarginsOnParentControl(thisCon);
            parentGrid.Children.Add(ThisDraw);
            parentGrid.Children.Add(thisCon);
            VerticalOptions = LayoutOptions.Start;
            Content = parentGrid;
        }
    }
}