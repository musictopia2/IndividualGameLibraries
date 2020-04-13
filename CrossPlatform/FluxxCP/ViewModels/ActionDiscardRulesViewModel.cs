using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ChooserClasses;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionDiscardRulesViewModel : BasicActionScreen
    {

        public int RulesToDiscard { get; set; }

        public ActionDiscardRulesViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
            RulesToDiscard = actionContainer.RulesToDiscard;
        }

        public bool CanViewRuleCard()
        {
            if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
                return ActionContainer.IndexRule > -1;
            return ActionContainer.TempRuleList!.Count == 1;
        }
        [Command(EnumCommandCategory.Plain)]
        public void ViewRuleCard()
        {
            if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
            {
                ActionContainer.CurrentDetail!.ShowCard(GameContainer.SaveRoot!.RuleList[ActionContainer.IndexRule + 1]);
                return;
            }
            ActionContainer.CurrentDetail!.ShowCard(GameContainer.SaveRoot!.RuleList[ActionContainer.TempRuleList.Single() + 1]);
        }

        public bool CanDiscardRules()
        {
            if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
                return ActionContainer.IndexRule > -1;
            return ActionContainer.TempRuleList!.Count <= ActionContainer.RulesToDiscard;
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task DiscardRulesAsync()
        {
            await BasicActionLogic.ShowMainScreenAgainAsync();
            if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
            {
                await FluxxEvent.RuleTrashedAsync(ActionContainer.IndexRule);
                return;
            }
            await FluxxEvent.RulesSimplifiedAsync(ActionContainer.TempRuleList!);
        }

    }
}
