using FluxxCP.Containers;

namespace FluxxXF.Views
{
    public class KeeperTrashView : KeeperProcessView
    {
        public KeeperTrashView(FluxxGameContainer gameContainer,
            KeeperContainer keeperContainer,
            ActionContainer actionContainer,
            FluxxVMData model) : base(gameContainer, keeperContainer, actionContainer, model)
        {
        }


    }
}
