using System.Threading.Tasks;
namespace LifeBoardGameCP.Logic
{
    public interface IHouseProcesses
    {
        Task ChoseHouseAsync(int house);
        void LoadHouseList();
        Task ShowYourHouseAsync();

    }
}