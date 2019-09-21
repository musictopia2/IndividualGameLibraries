using CommonBasicStandardLibraries.CollectionClasses;
namespace FluxxCP
{
    public interface IKeeper
    {
        void Init();
        void ShowKeepers();
        void LoadKeeperScreen();
        void ShowSelectedKeepers(CustomBasicList<KeeperPlayer> tempList);
        bool EntireVisible { get; }
        void VisibleChange();
        void LoadSavedGame();
    }
}