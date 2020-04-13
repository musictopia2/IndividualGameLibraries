using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionTradeHandsViewModel : BasicActionScreen
    {
        public ActionTradeHandsViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
        }
        public bool CanChoosePlayer => ActionContainer.CanChoosePlayer();
        [Command(EnumCommandCategory.Plain)]
        public async Task ChoosePlayerAsync()
        {
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.TradeHandsAsync(ActionContainer.IndexPlayer);
        }
    }
}
