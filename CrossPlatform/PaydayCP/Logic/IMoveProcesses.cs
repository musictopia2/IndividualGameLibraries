using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface IMoveProcesses
    {
        Task ResultsOfMoveAsync(int day);
    }
}
