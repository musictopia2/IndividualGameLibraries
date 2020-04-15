using BuncoDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace BuncoDiceGameXF.Views
{
    public class EndGameView : ContentView, IUIView
    {

        public EndGameView()
        {
            Button button = GetGamingButton("End Game", nameof(EndGameViewModel.EndGameAsync));
            Content = button;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
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
