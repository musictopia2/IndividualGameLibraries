using BasicGameFrameworkLibrary.Attributes;
using FluxxCP.Containers;

namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class KeeperTrashViewModel : KeeperActionViewModel
    {
        public KeeperTrashViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent) : base(gameContainer, keeperContainer, fluxxEvent)
        {
        }

        public override string ButtonText => "Trash A Keeper";
    }
}