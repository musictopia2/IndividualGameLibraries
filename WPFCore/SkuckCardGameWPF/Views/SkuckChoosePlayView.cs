using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace SkuckCardGameWPF.Views
{
    public class SkuckChoosePlayView : BaseFrameWPF, IUIView
    {
        public SkuckChoosePlayView()
        {
            Text = "Choose Play Or Pass Options";
            Grid grid = new Grid();
            var rect = ThisFrame.GetControlArea();
            StackPanel stack = new StackPanel();
            SetUpMarginsOnParentControl(stack, rect);
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
