using BuncoDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace BuncoDiceGameWPF.Views
{
    public class BuncoNewRoundView : UserControl, IUIView
    {
        public BuncoNewRoundView()
        {
            Button button = GetGamingButton("New Round", nameof(BuncoNewRoundViewModel.NewRoundAsync));
            Content = button;
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
