using System.Threading.Tasks;
using ThreeLetterFunCP.Data;

namespace ThreeLetterFunCP.BeginningClasses
{
    public interface IFirstOptionProcesses
    {
        Task BeginningOptionSelectedAsync(EnumFirstOption firstOption);
    }
}