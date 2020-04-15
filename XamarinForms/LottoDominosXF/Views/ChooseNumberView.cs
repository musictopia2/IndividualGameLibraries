using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using LottoDominosCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace LottoDominosXF.Views
{
    public class ChooseNumberView : CustomControlBase
    {
        private readonly NumberChooserXF _numberui;

        public ChooseNumberView()
        {
            StackLayout stack = new StackLayout();
            _numberui = new NumberChooserXF();
            _numberui.TotalRows = 1;
            stack.Children.Add(_numberui);
            Button button = GetGamingButton("Choose Number", nameof(ChooseNumberViewModel.ChooseNumberAsync));
            stack.Children.Add(button);
            Content = stack;
        }

        protected override Task TryActivateAsync()
        {
            ChooseNumberViewModel model = (ChooseNumberViewModel)BindingContext;
            _numberui.LoadLists(model.Number1);
            return base.TryActivateAsync();
        }
    }
}
