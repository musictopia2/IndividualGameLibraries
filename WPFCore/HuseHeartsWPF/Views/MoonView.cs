using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using HuseHeartsCP.Data;
using HuseHeartsCP.ViewModels;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace HuseHeartsWPF.Views
{
    public class MoonView : BaseFrameWPF, IUIView
    {

        public MoonView()
        {
            Grid grid = new Grid();

            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Text = "Shoot Moon Options";
            SKRect rect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(stack, rect);
            var button = GetGamingButton("Give Other Players 26 Points", nameof(MoonViewModel.MoonAsync));
            button.CommandParameter = EnumMoonOptions.GiveEverybodyPlus;
            stack.Children.Add(button);
            button = GetGamingButton("Reduce your score by 26 points", nameof(MoonViewModel.MoonAsync));
            button.CommandParameter = EnumMoonOptions.GiveSelfMinus;
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
