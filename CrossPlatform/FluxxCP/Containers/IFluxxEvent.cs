using CommonBasicStandardLibraries.CollectionClasses;
using FluxxCP.Data;
using System.Threading.Tasks;

namespace FluxxCP.Containers
{
    //i propose having a container handling this part.
    public interface IFluxxEvent
    {
        Task CloseKeeperScreenAsync();
        Task KeepersExchangedAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo);
        Task StealTrashKeeperAsync(KeeperPlayer currentKeeper, bool isTrashed);
        Task CardChosenToPlayAtAsync(int deck, int selectedIndex);
        Task CardToUseAsync(int deck);
        Task ChoseForEverybodyGetsOneAsync(CustomBasicList<int> selectedList, int selectedIndex);
        Task ChosePlayerForCardChosenAsync(int selectedIndex);
        Task DirectionChosenAsync(int selectedIndex);
        Task DoAgainSelectedAsync(int selectedIndex);
        Task FirstCardRandomChosenAsync(int deck);
        Task RulesSimplifiedAsync(CustomBasicList<int> simpleList);
        Task RuleTrashedAsync(int selectedIndex);
        Task TradeHandsAsync(int selectedIndex);
    }
}
