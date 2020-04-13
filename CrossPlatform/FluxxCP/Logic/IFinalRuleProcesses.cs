using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IFinalRuleProcesses
    {
        Task TrashNewRuleAsync(int index);
        Task SimplifyRulesAsync(CustomBasicList<int> thisList);
    }
}