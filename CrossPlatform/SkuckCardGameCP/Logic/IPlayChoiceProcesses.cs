using System.Threading.Tasks;

namespace SkuckCardGameCP.Logic
{
    public interface IPlayChoiceProcesses
    {
        Task ChooseToPlayAsync();
        Task ChooseToPassAsync();
    }
}