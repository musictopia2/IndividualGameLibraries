using LifeBoardGameCP.Data;
using System.Threading.Tasks;

namespace LifeBoardGameCP.Logic
{
    public interface IBoardProcesses
    {
        Task OpeningOptionAsync(EnumStart start);
        Task RetirementAsync(EnumFinal final);
        void SpaceDescription(int space);
        string GetSpaceDetails(int space);
        Task HumanChoseSpaceAsync();
        Task ComputerChoseSpaceAsync(int space);
        bool CanTrade4Tiles { get; }
        Task Trade4TilesAsync();
        bool CanPurchaseCarInsurance { get; }
        Task PurchaseCarInsuranceAsync();
        bool CanAttendNightSchool { get; }
        Task AttendNightSchoolAsync();
        bool CanPurchaseHouseInsurance { get; }
        Task PurchaseHouseInsuranceAsync();
        bool CanPurchaseStock { get; }
        Task PurchaseStockAsync();
        bool CanSellHouse { get; }
        Task SellHouseAsync();
        bool CanEndTurn { get; }
    }
}