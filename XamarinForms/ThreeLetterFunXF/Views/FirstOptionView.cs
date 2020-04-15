using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using ThreeLetterFunCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace ThreeLetterFunXF.Views
{
    public class FirstOptionView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly ListChooserXF _list;
        private readonly IEventAggregator _aggregator;

        public FirstOptionView(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _list = new ListChooserXF();
            StackLayout stack = new StackLayout();
            stack.Children.Add(_list);

            double buttonFontSize;
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                _list.ItemWidth = 400;
                _list.ItemHeight = 100;
                buttonFontSize = 50;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                _list.ItemWidth = 550;
                _list.ItemHeight = 150;
                buttonFontSize = 75;
            }
            else
            {
                _list.ItemWidth = 800;
                _list.ItemHeight = 175;
                buttonFontSize = 115;
            }
            //GamePackageViewModelBinder.ManuelElements.Clear();
            var thisBut = GetGamingButton("Submit", nameof(FirstOptionViewModel.SubmitAsync));
            thisBut.FontSize = buttonFontSize;
            //GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Description", nameof(FirstOptionViewModel.DescriptionAsync));
            //GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            thisBut.FontSize = buttonFontSize;
            otherStack.Children.Add(thisBut);
            stack.Children.Add(otherStack);
            Content = stack;
            _aggregator = aggregator;
        }


        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask; //needs this.  otherwise, could get error because it needs something that can load something.
        }

        protected override Task TryActivateAsync()
        {
            FirstOptionViewModel first = (FirstOptionViewModel)BindingContext;
            _list.LoadLists(first.Option1);
            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }

    }
}
