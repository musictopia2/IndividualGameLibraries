using System.Threading.Tasks;

namespace RookCP.Logic
{
    public interface IBidProcesses
    {
        Task BeginBiddingAsync();
        Task<bool> CanPassAsync();
        Task PassBidAsync();
        Task ProcessBidAsync();
    }
}
