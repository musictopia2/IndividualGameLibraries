using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using FluxxCP.Cards;
using FluxxCP.Containers;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace FluxxXF
{
    public class RuleUI : BaseFrameXF
    {

        private DeckObservableDict<RuleCard>? _ruleList;
        private StackLayout? _stack;



        private void LoadRules()
        {
            _stack!.Children.Clear();
            _ruleList!.ForEach(thisRule =>
            {
                var label = GetDefaultLabel();
                label.Text = thisRule.Text();
                _stack.Children.Add(label);
            });
        }

        public void LoadControls(FluxxGameContainer gameContainer)
        {
            Text = "Rule Information";
            _ruleList = gameContainer.SaveRoot!.RuleList;
            _ruleList.CollectionChanged += RuleListChange;
            _stack = new StackLayout();
            Grid grid = new Grid();
            WidthRequest = 200;
            SetUpMarginsOnParentControl(_stack); //i think.
            grid.Children.Add(ThisDraw);
            grid.Children.Add(_stack);
            Content = grid;
            LoadRules();
        }
        private void RuleListChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadRules();
        }

    }
}
