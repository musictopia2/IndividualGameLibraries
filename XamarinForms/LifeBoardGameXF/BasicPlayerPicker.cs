using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeBoardGameXF
{
    public abstract class BasicPlayerPicker : CustomControlBase
    {
        private readonly IEventAggregator _aggregator;

        public BasicPlayerPicker(LifeBoardGameVMData model, IEventAggregator aggregator)
        {
            //i think just the listview and submit
            //hopefully this one does not need end turn (?)
            //this is desktop anyways.
            ListChooserXF list = new ListChooserXF();
            list.ItemHeight = 50; // try this.
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            stack.Children.Add(list);
            list.LoadLists(model.PlayerPicker);
            Button button = GetGamingButton("Submit", nameof(BasicSubmitViewModel.SubmitAsync));
            button.FontSize = 80; //could be iffy.
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
