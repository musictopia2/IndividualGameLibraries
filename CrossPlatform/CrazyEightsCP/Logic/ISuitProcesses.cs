using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System.Threading.Tasks;

namespace CrazyEightsCP.Logic
{
    public interface ISuitProcesses
    {
        Task SuitChosenAsync(EnumSuitList suit);
    }
}