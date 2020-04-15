using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using ThinkTwiceCP.Data;
using ThinkTwiceCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ThinkTwiceXF.Views
{
    public class ScoreView : FrameUIViewXF
    {
        public ScoreView(ThinkTwiceVMData model)
        {
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            StackLayout stack = new StackLayout();
            Text = model.DisplayScoreText;
            SetBinding(IsEnabledProperty, new Binding(nameof(ScoreViewModel.IsEnabled))); // this needs bindings

            var rect = ThisFrame.GetControlArea();
            stack.Margin = new Thickness((double)rect.Left + (float)3, (double)rect.Top + 10, 3, 3); // try this way.
            Button button;
            foreach (var thisItem in model.TextList)
            {
                button = GetButtonNormalButton(thisItem);
                stack.Children.Add(button);
            }
            StackLayout finalStack = new StackLayout();
            finalStack.Orientation = StackOrientation.Horizontal;
            button = GetGamingButton("Your Score", nameof(ScoreViewModel.CalculateScoreAsync));
            finalStack.Children.Add(button);
            button = GetGamingButton("Description", nameof(ScoreViewModel.ScoreDescriptionAsync));
            finalStack.Children.Add(button);
            stack.Children.Add(finalStack);
            grid.Children.Add(stack);
            Content = grid;
        }

        private Button GetButtonNormalButton(string text)
        {
            var thisBut = GetGamingButton(text, nameof(ScoreViewModel.ChangeSelection));
            thisBut.CommandParameter = text;
            ColorConverter thisConvert = new ColorConverter(); // this way i know what was chosen.
            Binding thisBind = new Binding(nameof(ScoreViewModel.ItemSelected));
            thisBind.Converter = thisConvert; // made a careless mistake.
            thisBind.ConverterParameter = text;
            thisBut.SetBinding(BackgroundColorProperty, thisBind);
            return thisBut;
        }


    }
}
