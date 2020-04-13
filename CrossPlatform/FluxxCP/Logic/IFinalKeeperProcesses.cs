using FluxxCP.Data;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IFinalKeeperProcesses
    {
        Task ProcessTrashStealKeeperAsync(KeeperPlayer thisKeeper, bool isTrashed);
        Task ProcessExchangeKeepersAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo);
    }
}