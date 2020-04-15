using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using XactikaCP.Data;
using XactikaCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace XactikaWPF.Views
{
    public class XactikaModeView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        public XactikaModeView(IEventAggregator aggregator, XactikaVMData model)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout stack = new StackLayout();
            ListChooserXF list = new ListChooserXF();



            Button button = GetGamingButton("Submit Game Option", nameof(XactikaModeViewModel.ModeAsync));

            if (ScreenUsed == EnumScreen.LargeTablet)
            {
                list.ItemHeight = 130;
                list.ItemWidth = 500;
                button.FontSize = 125;
            }
            else
            {
                list.ItemHeight = 80;
                list.ItemWidth = 300;
                button.FontSize = 100;
            }

            stack.Children.Add(list);
            stack.Children.Add(button);
            Content = stack;
            list.LoadLists(model.ModeChoose1);
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }

    }
}