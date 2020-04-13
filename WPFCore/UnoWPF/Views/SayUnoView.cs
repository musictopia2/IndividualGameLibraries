using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows.Controls;
using UnoCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace UnoWPF.Views
{
    public class SayUnoView : UserControl, IUIView
    {
        public SayUnoView()
        {
            Button button = GetGamingButton("Uno", nameof(SayUnoViewModel.SayUnoAsync));
            Content = button; //hopefully this simple.
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
