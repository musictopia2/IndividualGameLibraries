using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionDirectionViewModel : BasicActionScreen
    {
        public ActionDirectionViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic
            ) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
        }
        public bool CanDirection => ActionContainer.IndexDirection > -1;
        [Command(EnumCommandCategory.Plain)]
        public async Task DirectionAsync()
        {
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.DirectionChosenAsync(ActionContainer.IndexDirection);
        }

    }
}
