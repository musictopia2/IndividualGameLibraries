using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using RookCP.Data;
using RookCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace RookXF.Views
{
    public class RookColorView : FrameUIViewXF
    {
        public RookColorView(RookVMData model)
        {
            EnumPickerXF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserXF<EnumColorTypes>, EnumColorTypes> color;
            Text = "Trump Info";
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack);
            color = new EnumPickerXF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserXF<EnumColorTypes>, EnumColorTypes>();
            color.LoadLists(model.Color1!);
            stack.Children.Add(color);
            Button button = GetGamingButton("Choose Color", nameof(RookColorViewModel.TrumpAsync));
            stack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
        }

    }
}
