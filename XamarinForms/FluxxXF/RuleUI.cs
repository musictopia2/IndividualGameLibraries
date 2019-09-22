using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicDrawables.Dictionary;
using FluxxCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FluxxXF
{
    public class RuleUI : BaseFrameXF
    {
        private DeckObservableDict<RuleCard>? _ruleList;
        private StackLayout? _thisStack;
        private FluxxMainGameClass? _mainGame;
        private void LoadRules()
        {
            _thisStack!.Children.Clear();
            _ruleList!.ForEach(thisRule =>
            {
                var thisLabel = GetDefaultLabel();
                thisLabel.Text = thisRule.Text();
                _thisStack.Children.Add(thisLabel);
            });
        }
        public void LoadControls()
        {
            _mainGame = Resolve<FluxxMainGameClass>();
            Text = "Rule Information";
            _ruleList = _mainGame.SaveRoot!.RuleList;
            _ruleList.CollectionChanged += RuleListChange;
            _thisStack = new StackLayout();
            Grid thisGrid = new Grid();
            WidthRequest = 200; //may have to be adjusted.  iffy.
            SetUpMarginsOnParentControl(_thisStack); //i think.
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(_thisStack);
            Content = thisGrid;
            LoadRules();
        }
        private void RuleListChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadRules();
        }
        public void UpdateControls()
        {
            _ruleList!.CollectionChanged -= RuleListChange;
            _ruleList = _mainGame!.SaveRoot!.RuleList;
            _ruleList.CollectionChanged += RuleListChange;
            LoadRules();
        }
    }
}
