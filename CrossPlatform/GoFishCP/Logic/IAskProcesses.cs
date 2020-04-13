using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System.Threading.Tasks;

namespace GoFishCP.Logic
{
    public interface IAskProcesses
    {
        void LoadAskList();
        Task NumberToAskAsync(EnumCardValueList asked);
    }
}