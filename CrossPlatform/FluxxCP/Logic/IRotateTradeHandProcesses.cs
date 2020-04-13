using FluxxCP.Data;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IRotateTradeHandProcesses
    {
        Task RotateHandAsync(EnumDirection direction);
        Task TradeHandAsync(int selectedIndex);
    }
}