using FluxxCP.Containers;

namespace FluxxWPF.Views
{
    public class KeeperStealView : KeeperProcessView
    {
        public KeeperStealView(FluxxGameContainer gameContainer,
            KeeperContainer keeperContainer,
            ActionContainer actionContainer,
            FluxxVMData model) : base(gameContainer, keeperContainer, actionContainer, model)
        {
        }


    }
}