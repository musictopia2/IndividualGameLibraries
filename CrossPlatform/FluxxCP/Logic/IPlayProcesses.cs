using FluxxCP.Cards;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IPlayProcesses
    {
        Task SendPlayAsync(int deck);
        Task PlayCardAsync(int deck);
        Task PlayCardAsync(FluxxCardInformation card);
        //i think this should not matter if its a random one.  separate method though.
        Task PlayRandomCardAsync(int deck, int player);
        Task PlayRandomCardAsync(FluxxCardInformation thisCard, int player);
        Task PlayRandomCardAsync(int deck);

        Task PlayUseTakeAsync(int deck, int player); //hopefully this works too.
    }
}