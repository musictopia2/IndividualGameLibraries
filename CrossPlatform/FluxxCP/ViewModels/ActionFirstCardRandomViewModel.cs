using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class ActionFirstCardRandomViewModel : BasicActionScreen
    {
        public ActionFirstCardRandomViewModel(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic)
        {
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task ChooseCardAsync()
        {
            if (ActionContainer.OtherHand!.ObjectSelected() == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose a card");
                return;
            }
            await BasicActionLogic.ShowMainScreenAgainAsync();
            await FluxxEvent.FirstCardRandomChosenAsync(ActionContainer.OtherHand.ObjectSelected());



            
        }
    }
}
