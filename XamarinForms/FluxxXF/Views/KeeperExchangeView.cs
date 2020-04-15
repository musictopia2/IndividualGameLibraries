using FluxxCP.Containers;

namespace FluxxXF.Views
{
    public class KeeperExchangeView : KeeperProcessView
    {
        public KeeperExchangeView(FluxxGameContainer gameContainer,
            KeeperContainer keeperContainer,
            ActionContainer actionContainer,
            FluxxVMData model) : base(gameContainer, keeperContainer, actionContainer, model)
        {
        }
    }
}