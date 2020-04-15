using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using HuseHeartsCP.Data;
using HuseHeartsCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace HuseHeartsXF.Views
{
    public class MoonView : FrameUIViewXF
    {
        public MoonView()
        {
            Grid grid = new Grid();
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            Text = "Shoot Moon Options";
            SetUpMarginsOnParentControl(stack);
            var button = GetSmallerButton($"Give Other{Constants.vbCrLf}Players 26 Points", nameof(MoonViewModel.MoonAsync));
            button.CommandParameter = EnumMoonOptions.GiveEverybodyPlus;
            stack.Children.Add(button);
            button = GetSmallerButton($"Reduce your{Constants.vbCrLf}score by{Constants.vbCrLf}26 points", nameof(MoonViewModel.MoonAsync));
            button.CommandParameter = EnumMoonOptions.GiveSelfMinus;
            stack.Children.Add(button);
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
        }
    }
}
