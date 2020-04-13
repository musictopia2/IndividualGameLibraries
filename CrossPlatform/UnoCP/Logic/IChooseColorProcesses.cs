using BasicGameFrameworkLibrary.ColorCards;
using System.Threading.Tasks;

namespace UnoCP.Logic
{
    public interface IChooseColorProcesses
    {
        Task ColorChosenAsync(EnumColorTypes color);
    }
}
