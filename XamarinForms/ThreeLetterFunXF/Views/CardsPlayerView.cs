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

namespace ThreeLetterFunXF.Views
{
    public class CardsPlayerView : CustomControlBase, IHandleAsync<LoadEventModel>
    {

        private readonly ListChooserXF _list;
        private readonly IEventAggregator _aggregator;

        public CardsPlayerView(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _list = new ListChooserXF();
            _list.Orientation = StackOrientation.Horizontal;
            StackLayout stack = new StackLayout();
            stack.Children.Add(_list);
            var thisBut = GetGamingButton("Submit", nameof(CardsPlayerViewModel.SubmitAsync));

            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                _list.ItemWidth = 155;
                _list.ItemHeight = 35; // well see
                thisBut.FontSize = 75; // can adjust as needed
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                _list.ItemWidth = 200;
                _list.ItemHeight = 50; // well see
                thisBut.FontSize = 150; // can adjust as needed
            }
            else
            {
                _list.ItemWidth = 400;
                _list.ItemHeight = 80; // well see
                thisBut.FontSize = 200; // can adjust as needed
            }

            stack.Children.Add(thisBut);
            Content = stack;
            _aggregator = aggregator;
        }
        protected override Task TryActivateAsync()
        {
            CardsPlayerViewModel model = (CardsPlayerViewModel)BindingContext;
            _list.LoadLists(model.CardList1);
            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask; //needs this.  otherwise, could get error because it needs something that can load something.
        }
    }
}
