using System.Threading.Tasks;
using XactikaCP.Data;

namespace XactikaCP.Logic
{
    public interface IModeProcesses
    {
        Task EnableOptionsAsync();
        Task ProcessGameOptionChosenAsync(EnumGameMode optionChosen, bool doShow);
    }
}