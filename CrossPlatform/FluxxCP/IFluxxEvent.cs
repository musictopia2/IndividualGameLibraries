using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxCP
{
    public interface IFluxxEvent
    {
        void CloseKeeperScreen();
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
        Task TradeHandsAsync(int SelectedIndex);
    }
}