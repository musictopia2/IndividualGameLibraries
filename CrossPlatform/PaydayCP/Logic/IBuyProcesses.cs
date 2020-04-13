using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface IBuyProcesses
    {
        Task BuyerSelectedAsync(int deck);
        //this is the start of the buy processes.
        Task ProcessBuyerAsync();

        //not sure if anything else is needed (?)
    }
}