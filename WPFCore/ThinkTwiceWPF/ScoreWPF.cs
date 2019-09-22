using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ThinkTwiceCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ThinkTwiceWPF
{
    public class ScoreWPF : BaseFrameWPF
    {
        private ScoreViewModel? _thisMod;
        public void LoadList(ScoreViewModel mod)
        {
            _thisMod = mod;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            StackPanel thisStack = new StackPanel();
            Text = _thisMod.Text; // can do it this way.  the text won't change.
            var thisRect = ThisFrame.GetControlArea();
            thisStack.Margin = new Thickness((double)thisRect.Left + (float)3, (double)thisRect.Top + 10, 3, 3); // try this way.
            SetBinding(IsEnabledProperty, new Binding(nameof(ScoreViewModel.IsEnabled))); // this needs bindings
            DataContext = _thisMod;
            Button thisBut;
            foreach (var thisItem in _thisMod.TextList)
            {
                thisBut = GetButtonNormalButton(thisItem);
                thisStack.Children.Add(thisBut);
            }
            StackPanel finalStack = new StackPanel();
            finalStack.Orientation = Orientation.Horizontal;
            thisBut = GetGamingButton("Your Score", nameof(ScoreViewModel.ScoreCalculateCommand));
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Description", nameof(ScoreViewModel.ScoreDescriptionCommand));
            finalStack.Children.Add(thisBut);
            thisStack.Children.Add(finalStack);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
        private Button GetButtonNormalButton(string text)
        {
            var thisBut = GetGamingButton(text, nameof(ScoreViewModel.ChangeSelectionCommand));
            thisBut.CommandParameter = text;
            ColorConverter thisConvert = new ColorConverter(); // this way i know what was chosen.
            Binding thisBind = new Binding(nameof(ScoreViewModel.ItemSelected));
            thisBind.Converter = thisConvert; // made a careless mistake.
            thisBind.ConverterParameter = text;
            thisBut.SetBinding(Button.BackgroundProperty, thisBind);
            return thisBut;
        }
    }
}