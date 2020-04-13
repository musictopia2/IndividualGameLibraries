using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class KeeperShowViewModel : BasicKeeperScreen
    {
        private readonly IFluxxEvent _fluxxEvent;

        public KeeperShowViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent) : base(gameContainer, keeperContainer)
        {
            _fluxxEvent = fluxxEvent;
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task CloseKeeperAsync()
        {
            await _fluxxEvent.CloseKeeperScreenAsync();
        }
    }
}
