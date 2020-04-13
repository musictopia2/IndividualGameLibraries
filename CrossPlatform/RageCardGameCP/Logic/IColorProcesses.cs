using System.Threading.Tasks;

namespace RageCardGameCP.Logic
{
    public interface IColorProcesses
    {
        Task ChooseColorAsync();
        Task ColorChosenAsync();
        void ShowLeadColor();
        Task LoadColorListsAsync();
    }
}
