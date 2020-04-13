using System.Threading.Tasks;

namespace LifeBoardGameCP.Logic
{
    public interface IBasicSalaryProcesses
    {
        Task ChoseSalaryAsync(int salary);
        Task LoadSalaryListAsync();
    }
}