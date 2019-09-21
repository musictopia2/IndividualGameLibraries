using System.Threading.Tasks;
namespace SolitaireBoardGameCP
{
    public interface ISolitaireBoardEvents
    {
        Task PieceSelectedAsync(GameSpace ThisSpace);
        Task PiecePlacedAsync(GameSpace ThisSpace); //i think
        void MoveCompleted();
        Task WonAsync();
    }
}