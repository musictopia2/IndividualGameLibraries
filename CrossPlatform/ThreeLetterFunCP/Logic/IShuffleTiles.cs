using System.Threading.Tasks;

namespace ThreeLetterFunCP.Logic
{
    public interface IShuffleTiles
    {
        Task StartShufflingAsync(ThreeLetterFunMainGameClass mainGame, int cardsToPassOut = 0);
    }
}