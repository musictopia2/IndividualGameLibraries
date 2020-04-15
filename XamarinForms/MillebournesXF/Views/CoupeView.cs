using BasicXFControlsAndPages.MVVMFramework.Controls;
using MillebournesCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace MillebournesXF.Views
{
    public class CoupeView : CustomControlBase
    {
        public CoupeView()
        {
            Button button = GetGamingButton("Coupe Foure", nameof(CoupeViewModel.CoupeAsync));
            Content = button;
        }
    }
}
