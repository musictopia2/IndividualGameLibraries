using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using RookCP.Cards;
using System.Threading.Tasks;

namespace RookCP.Logic
{
    public interface INestProcesses
    {
        Task ProcessNestAsync(DeckRegularDict<RookCardInformation> list);
    }
}
