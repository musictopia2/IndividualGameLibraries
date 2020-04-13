using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ThinkTwiceCP.Data;
using ThinkTwiceCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace ThinkTwiceWPF.Views
{
    public class ScoreView : BaseFrameWPF, IUIView
    {

        public ScoreView(ThinkTwiceVMData model)
        {
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            StackPanel stack = new StackPanel();
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
            StackPanel finalStack = new StackPanel();
            finalStack.Orientation = Orientation.Horizontal;
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
            thisBut.SetBinding(BackgroundProperty, thisBind);
            return thisBut;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
