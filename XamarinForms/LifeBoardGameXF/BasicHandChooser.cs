using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.ViewModels;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeBoardGameXF
{
    public abstract class BasicHandChooser : CustomControlBase
    {
        readonly Button _button;
        private readonly IEventAggregator _aggregator;

        public BasicHandChooser(LifeBoardGameVMData model, IEventAggregator aggregator)
        {
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            Button button = GetGamingButton("", nameof(BasicSubmitViewModel.SubmitAsync));
            _button = button;

            //button.FontSize = 50;
            button.VerticalOptions = LayoutOptions.Start;
            LifeHandXF hand = new LifeHandXF();
            hand.HandType = HandObservable<LifeBaseCard>.EnumHandList.Vertical;

            stack.Children.Add(hand);
            StackLayout seconds = new StackLayout();
            seconds.Children.Add(button);

            stack.Children.Add(button);
            hand.LoadList(model.HandList, "");
            Content = stack;
            _aggregator = aggregator;
        }

        

        protected override Task TryActivateAsync()
        {
            BasicSubmitViewModel model = (BasicSubmitViewModel)BindingContext;
            _button.Text = model.Text;
            return this.RefreshBindingsAsync(_aggregator);
        }

    }
}
