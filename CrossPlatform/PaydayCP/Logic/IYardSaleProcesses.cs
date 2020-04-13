using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface IYardSaleProcesses
    {
        Task ProcessYardSaleAsync();
        Task FinishYardSaleAsync();
    }
}