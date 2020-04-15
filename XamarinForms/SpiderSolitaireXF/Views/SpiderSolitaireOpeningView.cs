using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using SpiderSolitaireCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace SpiderSolitaireXF.Views
{
    public class SpiderSolitaireOpeningView : CustomControlBase
    {
        readonly ListChooserXF _picker;
        public SpiderSolitaireOpeningView()
        {
            _picker = new ListChooserXF();
            if (ScreenUsed != EnumScreen.SmallPhone)
                _picker.ItemWidth /= 2;
            else
            {
                _picker.ItemWidth /= 4;
                _picker.ItemHeight = 20;
            }
            _picker.Margin = new Thickness(5);
            Content = _picker;
        }
        protected override Task TryActivateAsync()
        {
            SpiderSolitaireOpeningViewModel model = (SpiderSolitaireOpeningViewModel)BindingContext;
            _picker.LoadLists(model.LevelPicker);
            return Task.CompletedTask;
        }
    }
}
