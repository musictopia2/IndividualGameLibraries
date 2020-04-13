using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using PaydayCP.Cards;
using System.Threading.Tasks;

namespace PaydayCP.Logic
{
    public interface IDealProcesses
    {
        Task ChoseWhetherToPurchaseDealAsync();
        //this has to do more than just purchase deal.  however, not sure how to separate them because of naming.
        //so this will handle all the deal processes.

        Task ProcessDealAsync(bool isYardSale);
        void SetUpDeal();
        void PopulateDeals();
        Task ContinueDealProcessesAsync();
        Task ReshuffleDealsAsync(DeckRegularDict<DealCard> list);
        Task<bool> ProcessShuffleDealsAsync();
    }
}