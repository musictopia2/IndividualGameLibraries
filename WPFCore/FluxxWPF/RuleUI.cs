using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicDrawables.Dictionary;
using FluxxCP;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using SkiaSharp;
namespace FluxxWPF
{
    public class RuleUI : BaseFrameWPF
    {
        private DeckObservableDict<RuleCard>? _ruleList;
        private StackPanel? _thisStack;
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
            _thisStack = new StackPanel();
            Grid thisGrid = new Grid();
            Width = 200;
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(_thisStack, thisRect); //i think.
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
