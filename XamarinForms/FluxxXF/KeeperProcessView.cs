using FluxxCP.Containers;
using FluxxCP.ViewModels;

namespace FluxxXF
{
    public abstract class KeeperProcessView : KeeperBaseView
    {
        public KeeperProcessView(FluxxGameContainer gameContainer,
            KeeperContainer keeperContainer,
            ActionContainer actionContainer,
            FluxxVMData model) : base(gameContainer, keeperContainer, actionContainer, model)
        {
        }
        protected override EnumKeeperCategory KeeperCategory => EnumKeeperCategory.Process;
        protected override sealed string CommandText => nameof(KeeperActionViewModel.ProcessAsync);
    }
}