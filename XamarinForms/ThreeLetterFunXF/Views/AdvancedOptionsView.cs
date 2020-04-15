using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using ThreeLetterFunCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace ThreeLetterFunXF.Views
{
    public class AdvancedOptionsView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly ListChooserXF _list1;
        private readonly ListChooserXF _list2;
        private readonly IEventAggregator _aggregator;

        public AdvancedOptionsView(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _aggregator = aggregator;
            _list1 = new ListChooserXF();
            _list2 = new ListChooserXF();
            _list1.Orientation = StackOrientation.Horizontal;
            _list2.Orientation = StackOrientation.Horizontal;
            _list1.Margin = new Thickness(5);
            _list2.Margin = new Thickness(5);
            var thisBut = GetGamingButton("Submit", nameof(AdvancedOptionsViewModel.SubmitAsync));
            thisBut.FontSize = 200;
            thisBut.Margin = new Thickness(5);


            double buttonFontSize;
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                _list1.ItemWidth = 400; //well see when its on xamarin forms (lots of experimenting could be needed).
                _list2.ItemWidth = 400;
                _list1.ItemHeight = 50;
                _list2.ItemHeight = 50;
                buttonFontSize = 75;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                _list1.ItemWidth = 600; //well see when its on xamarin forms (lots of experimenting could be needed).
                _list2.ItemWidth = 600;
                _list1.ItemHeight = 90;
                _list2.ItemHeight = 90; //has to experiment
                buttonFontSize = 150;
            }
            else
            {
                _list1.ItemWidth = 800; //well see when its on xamarin forms (lots of experimenting could be needed).
                _list2.ItemWidth = 800;
                _list1.ItemHeight = 120;
                _list2.ItemHeight = 120;
                buttonFontSize = 200;
            }

            Grid grid = new Grid();
            AddLeftOverRow(grid, 50);
            AddLeftOverRow(grid, 50);
            AddAutoRows(grid, 1);
            AddControlToGrid(grid, _list1, 0, 0);
            AddControlToGrid(grid, _list2, 1, 0);
            AddControlToGrid(grid, thisBut, 2, 0);
            thisBut.FontSize = buttonFontSize;
            Content = grid;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask; //needs this.  otherwise, could get error because it needs something that can load something.
        }

        protected override Task TryActivateAsync()
        {
            AdvancedOptionsViewModel model = (AdvancedOptionsViewModel)BindingContext;
            _list1.LoadLists(model.Easy1);
            _list2.LoadLists(model.Game1);

            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }

    }
}
