using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class KeeperExchangeViewModel : KeeperActionViewModel
    {
        public KeeperExchangeViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent) : base(gameContainer, keeperContainer, fluxxEvent)
        {
        }

        public override string ButtonText => "Exchange A Keeper";
    }
}
