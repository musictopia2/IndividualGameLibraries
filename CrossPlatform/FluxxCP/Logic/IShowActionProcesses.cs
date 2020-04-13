using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks;

namespace FluxxCP.Logic
{
    public interface IShowActionProcesses
    {
        //this should only focus on showing the action.
        //this can use the basic action logic.  that is probably best.


        Task ChoseOtherCardSelectedAsync(int deck);
        Task ShowRuleTrashedAsync(int selectedIndex);
        Task ShowLetsDoAgainAsync(int selectedIndex);
        Task ShowRulesSimplifiedAsync(CustomBasicList<int> list);
        Task ShowDirectionAsync(int selectedIndex);
        Task ShowTradeHandAsync(int selectedIndex);
        Task ShowPlayerForCardChosenAsync(int selectedIndex);
        Task ShowChosenForEverybodyGetsOneAsync(CustomBasicList<int> selectedList, int selectedIndex);
        Task ShowCardUseAsync(int deck);


    }
}
