using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IEverybodyOneProcesses
    {
        Task EverybodyGetsOneAsync(CustomBasicList<int> thisList, int selectedIndex);
    }
}