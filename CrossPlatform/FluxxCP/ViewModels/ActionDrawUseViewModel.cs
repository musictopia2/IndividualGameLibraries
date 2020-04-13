using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionDrawUseViewModel : BasicActionScreen
    {
        public ActionDrawUseViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task DrawUseAsync()
        {
            if (ActionContainer.TempHand!.ObjectSelected() == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose a card");
                return;
            }
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.CardToUseAsync(ActionContainer.TempHand.ObjectSelected());
        }
    }
}
