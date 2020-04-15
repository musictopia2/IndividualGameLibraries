using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Data;
using PaydayCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace PaydayXF.Views
{
    public class RollerView : CustomControlBase
    {
        private readonly IEventAggregator _aggregator;

        public RollerView(PaydayVMData model, IEventAggregator aggregator)
        {
            StackLayout stack = new StackLayout();


            var thisRoll = GetSmallerButton("Roll Dice", nameof(RollerViewModel.RollDiceAsync));
            stack.Children.Add(thisRoll);
            DiceListControlXF<SimpleDice> dice = new DiceListControlXF<SimpleDice>();
            dice.LoadDiceViewModel(model.Cup!);
            stack.Children.Add(dice);
            Content = stack;
            _aggregator = aggregator;
        }
        protected override Task TryActivateAsync()
        {
            return this.RefreshBindingsAsync(_aggregator); //hopefully this simple (?)
        }
    }
}