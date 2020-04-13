using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IAnalyzeProcesses
    {
        Task AnalyzeQueAsync();
        void AnalyzeRules();
    }
}