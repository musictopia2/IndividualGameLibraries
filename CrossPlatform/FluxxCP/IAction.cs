using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxCP
{
    public interface IAction
    {
        void LoadSavedGame();
        void LoadActionScreen();
        void Init();
        void SetUpGoals();
        void SetUpFrames(); // i think this is still needed since this will start the bindings.
        Task ChoseOtherCardSelectedAsync(int deck);
        int GetPlayerIndex(int selectedIndex);
        Task ShowRulesSimplifiedAsync(CustomBasicList<int> tempList);
        Task ShowRuleTrashedAsync(int selectedIndex);
        FluxxCardInformation GetCardToDoAgain(int selectedIndex);
        Task ShowLetsDoAgainAsync(int selectedIndex);
        Task ShowDirectionAsync(int selectedIndex);
        Task ShowTradeHandAsync(int selectedIndex);
        Task ShowPlayerForCardChosenAsync(int selectedIndex);
        Task ShowChosenForEverybodyGetsOneAsync(CustomBasicList<int> selectedList, int selectedIndex);
        bool CanLoadEverybodyGetsOne();
        Task ShowCardUseAsync(int deck);
        CustomBasicList<int> GetTempPlayerList();
        CustomBasicList<int> GetTempRuleList();
        CustomBasicList<int> GetTempCardList();
        int MaxRulesToDiscard();
        void ResetPlayers();
        bool EntireVisible { get; }
        void VisibleChange();
        int IndexPlayer { get; set; }
    }
}