using BuncoDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace BuncoDiceGameXF.Views
{
    public class BuncoNewRoundView : ContentView, IUIView
    {
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
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
