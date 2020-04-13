using SolitaireBoardGameCP.Data;
using System.Threading.Tasks;
namespace SolitaireBoardGameCP.Logic
{
    public interface ISolitaireBoardEvents
    {
        Task PieceSelectedAsync(GameSpace space, SolitaireBoardGameMainGameClass game);
        Task PiecePlacedAsync(GameSpace space, SolitaireBoardGameMainGameClass game); //i think
    }
}