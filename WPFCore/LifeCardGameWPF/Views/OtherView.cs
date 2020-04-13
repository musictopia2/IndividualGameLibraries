using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeCardGameCP.Data;
using LifeCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeCardGameWPF.Views
{
    public class OtherView : UserControl, IUIView
    {
        public OtherView(LifeCardGameVMData model)
        {
            Button button = GetGamingButton(model.OtherText, nameof(OtherViewModel.ProcessOtherAsync));
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
