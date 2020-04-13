using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MillebournesCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace MillebournesWPF.Views
{
    public class CoupeView : UserControl, IUIView
    {
        public CoupeView()
        {
            Button button = GetGamingButton("Coupe Foure", nameof(CoupeViewModel.CoupeAsync));
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
