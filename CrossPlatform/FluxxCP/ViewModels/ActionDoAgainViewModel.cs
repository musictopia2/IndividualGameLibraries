using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionDoAgainViewModel : BasicActionScreen
    {
        public ActionDoAgainViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
        }
        private bool CanDoAction => ActionContainer.IndexCard > -1;

        public bool CanViewCard => CanDoAction;
        [Command(EnumCommandCategory.Plain)]
        public void ViewCard()
        {
            var thisCard = ActionContainer.GetCardToDoAgain(ActionContainer.IndexCard);
            ActionContainer.CurrentDetail!.ShowCard(thisCard);
        }

        public bool CanSelectCard => CanDoAction;
        [Command(EnumCommandCategory.Plain)]
        public async Task SelectCardAsync()
        {
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.DoAgainSelectedAsync(ActionContainer.IndexCard);
        }

    }
}
