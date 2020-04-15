using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace SkuckCardGameWPF.Views
{
    public class SkuckChoosePlayView : FrameUIViewXF
    {
        public SkuckChoosePlayView()
        {
            Text = "Choose Play Or Pass Options";
            Grid grid = new Grid();
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack);
            Button button = GetGamingButton("Play", nameof(SkuckChoosePlayViewModel.FirstPlayAsync));
            button.CommandParameter = EnumChoiceOption.Play;
            stack.Children.Add(button);
            button = GetGamingButton("Pass", nameof(SkuckChoosePlayViewModel.FirstPlayAsync));
            button.CommandParameter = EnumChoiceOption.Pass;
            stack.Children.Add(button);
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
        }

    }
}
