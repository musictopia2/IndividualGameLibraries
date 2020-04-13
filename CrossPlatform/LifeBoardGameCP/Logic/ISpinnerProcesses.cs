using LifeBoardGameCP.Data;
using System.Threading.Tasks;

namespace LifeBoardGameCP.Logic
{
    public interface ISpinnerProcesses
    {
        Task StartSpinningAsync(SpinnerPositionData position);
        Task StartSpinningAsync();
    }
}