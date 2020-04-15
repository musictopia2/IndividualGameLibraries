using BasicXFControlsAndPages.MVVMFramework.Controls;
using LifeCardGameCP.Data;
using LifeCardGameCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeCardGameXF.Views
{
    public class OtherView : CustomControlBase
    {
        public OtherView(LifeCardGameVMData model)
        {
            Button button = GetGamingButton(model.OtherText, nameof(OtherViewModel.ProcessOtherAsync));
            Content = button;
        }
    }
}
