using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.Logic
{
    public interface IMoveProcesses
    {
        Task PossibleAutomateMoveAsync();
        Task DoAutomateMoveAsync(int space);
        //hopefully the move processes does not need the trade salary or another overflow error.
    }
}