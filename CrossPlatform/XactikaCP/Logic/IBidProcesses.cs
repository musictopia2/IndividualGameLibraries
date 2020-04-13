using System.Threading.Tasks;

namespace XactikaCP.Logic
{
    public interface IBidProcesses
    {
        Task ProcessBidAsync();
        Task BeginBiddingAsync();
        Task EndBidAsync();
        Task PopulateBidAmountsAsync();
    }
}
