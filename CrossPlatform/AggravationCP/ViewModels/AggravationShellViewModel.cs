using AggravationCP.Data;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.ViewModels;
//i think this is the most common things i like to do
namespace AggravationCP.ViewModels
{
    public class AggravationShellViewModel : BasicBoardGamesShellViewModel<AggravationPlayerItem>
    {
        public AggravationShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container,
            IGameInfo gameData,
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test)
            : base(mainContainer, container, gameData, basicData, save, test)
        {
        }

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<AggravationMainViewModel>();
            return model;
        }
    }
}
