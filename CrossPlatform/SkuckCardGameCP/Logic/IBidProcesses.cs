using System.Threading.Tasks;

namespace SkuckCardGameCP.Logic
{
    public interface IBidProcesses
    {
        Task ProcessBidAmountAsync(int id);
    }
}
