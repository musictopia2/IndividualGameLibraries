using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using ThinkTwiceCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace ThinkTwiceXF
{
    public class ScoreXF : BaseFrameXF
    {
        private ScoreViewModel? _thisMod;
        public void LoadList(ScoreViewModel mod)
        {
            _thisMod = mod;
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            StackLayout thisStack = new StackLayout();
            Text = _thisMod.Text; // can do it this way.  the text won't change.
            var thisRect = ThisFrame.GetControlArea();
            thisStack.Margin = new Thickness((double)thisRect.Left + (float)3, (double)thisRect.Top + 2, 3, 3); // try this way.
            SetBinding(IsEnabledProperty, new Binding(nameof(ScoreViewModel.IsEnabled))); // this needs bindings
            BindingContext = _thisMod;
            Button thisBut;
            foreach (var thisItem in _thisMod.TextList)
            {
                thisBut = GetButtonNormalButton(thisItem);
                thisStack.Children.Add(thisBut);
            }
            StackLayout finalStack = new StackLayout();
            finalStack.Orientation = StackOrientation.Horizontal;
            thisBut = GetSmallerButton("Your Score", nameof(ScoreViewModel.ScoreCalculateCommand));
            finalStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Description", nameof(ScoreViewModel.ScoreDescriptionCommand));
            finalStack.Children.Add(thisBut);
            thisStack.Children.Add(finalStack);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
        private Button GetButtonNormalButton(string text)
        {
            var thisBut = GetSmallerButton(text, nameof(ScoreViewModel.ChangeSelectionCommand));
            thisBut.CommandParameter = text;
            ColorConverter thisConvert = new ColorConverter(); // this way i know what was chosen.
            Binding thisBind = new Binding(nameof(ScoreViewModel.ItemSelected));
            thisBind.Converter = thisConvert; // made a careless mistake.
            thisBind.ConverterParameter = text;
            thisBut.SetBinding(Button.BackgroundColorProperty, thisBind);
            return thisBut;
        }
    }
}