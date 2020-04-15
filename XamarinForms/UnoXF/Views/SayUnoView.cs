using BasicXFControlsAndPages.MVVMFramework.Controls;
using UnoCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace UnoXF.Views
{
    public class SayUnoView : CustomControlBase
    {
        public SayUnoView()
        {
            Button button = GetGamingButton("Uno", nameof(SayUnoViewModel.SayUnoAsync));
            Content = button; //hopefully this simple.
        }
    }
}
