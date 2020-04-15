using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace PaydayXF
{
    public abstract class BasicPickerView : CustomControlBase
    {
        private readonly IEventAggregator _aggregator;

        public BasicPickerView(PaydayVMData model, IEventAggregator aggregator)
        {
            ListChooserXF list = new ListChooserXF();
            list.ItemHeight = 50; //iffy.
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            stack.Children.Add(list);
            list.LoadLists(model.PopUpList);
            Button button = GetSmallerButton("Submit", nameof(BasicSubmitViewModel.SubmitAsync));
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            //button.FontSize = 100; //make 100 instead of 200 now.
            stack.Children.Add(button); //can always adjust as needed anyways.
            Content = stack;
            _aggregator = aggregator;
        }
        protected override Task TryActivateAsync()
        {
            return this.RefreshBindingsAsync(_aggregator);
        }
    }
}
