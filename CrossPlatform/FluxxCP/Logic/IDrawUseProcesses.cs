using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IDrawUseProcesses
    {
        Task DrawUsedAsync(int deck);
    }
}