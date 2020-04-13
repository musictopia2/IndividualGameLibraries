using System.Threading.Tasks;

namespace LifeBoardGameCP.Logic
{
    public interface ITradeSalaryProcesses
    {
        Task TradedSalaryAsync(string player);
        Task ComputerTradeAsync();
        void LoadOtherPlayerSalaries();
    }
}