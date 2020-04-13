using System.Threading.Tasks;

namespace RookCP.Logic
{
    public interface ITrumpProcesses
    {
        Task ProcessTrumpAsync();
        void ResetTrumps();
    }
}