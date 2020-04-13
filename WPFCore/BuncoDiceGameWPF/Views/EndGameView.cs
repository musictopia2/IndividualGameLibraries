using BuncoDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace BuncoDiceGameWPF.Views
{
    public class EndGameView : UserControl, IUIView
    {
        public EndGameView()
        {
            Button button = GetGamingButton("End Game", nameof(EndGameViewModel.EndGameAsync));
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
