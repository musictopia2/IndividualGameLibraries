using System.Threading.Tasks;
using XactikaCP.Data;

namespace XactikaCP.Logic
{
    public interface IShapeProcesses
    {
        Task ShapeChosenAsync(EnumShapes shape);
        Task FirstCallShapeAsync(); //this is where you should show the proper shapes (?)
    }
}
