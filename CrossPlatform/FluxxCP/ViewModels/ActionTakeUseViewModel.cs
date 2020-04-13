using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionTakeUseViewModel : BasicActionScreen
    {
        public ActionTakeUseViewModel(FluxxGameContainer gameContainer,
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
            await FluxxEvent.ChosePlayerForCardChosenAsync(ActionContainer.IndexPlayer);
        }
        public bool CanChooseCard => ActionContainer.OtherHand.ObjectSelected() > 0;
        [Command(EnumCommandCategory.Plain)]
        public async Task ChooseCardAsync()
        {
            if (ActionContainer.IndexPlayer == -1)
            {
                throw new BasicBlankException("Must have the player chosen in order to use what you take from another player");
            }
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.CardChosenToPlayAtAsync(ActionContainer.OtherHand.ObjectSelected(), ActionContainer.IndexPlayer);
        }



    }
}