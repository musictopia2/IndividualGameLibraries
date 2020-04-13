using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RookCP.Data;
using RookCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace RookWPF.Views
{
    public class RookColorView : BaseFrameWPF, IUIView
    {
        public RookColorView(RookVMData model)
        {
            EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserWPF<EnumColorTypes>, EnumColorTypes> color;
            Text = "Trump Info";
            var rect = ThisFrame.GetControlArea();
            StackPanel stack = new StackPanel();
            SetUpMarginsOnParentControl(stack, rect);
            color = new EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserWPF<EnumColorTypes>, EnumColorTypes>();
            color.LoadLists(model.Color1!);
            stack.Children.Add(color);
            Button button = GetGamingButton("Choose Color", nameof(RookColorViewModel.TrumpAsync));
            stack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
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
