using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using FluxxCP.Cards;
using FluxxCP.Containers;
using SkiaSharp;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF
{
    public class RuleUI : BaseFrameWPF
    {
        private DeckObservableDict<RuleCard>? _ruleList;
        private StackPanel? _stack;
        //private FluxxMainGameClass? _mainGame;
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
            _stack = new StackPanel();
            Grid grid = new Grid();
            Width = 200;
            SKRect rect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(_stack, rect); //i think.
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
