using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class KeeperStealViewModel : KeeperActionViewModel
    {
        public KeeperStealViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent) : base(gameContainer, keeperContainer, fluxxEvent)
        {
        }

        public override string ButtonText => "Steal A Keeper";
    }
}
