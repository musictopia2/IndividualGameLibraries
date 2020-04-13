using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using GolfCardGameCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using GolfCardGameCP.Logic;
//i think this is the most common things i like to do
namespace GolfCardGameCP.ViewModels
{
    public class GolfCardGameShellViewModel : BasicMultiplayerShellViewModel<GolfCardGamePlayerItem>
    {
        public GolfCardGameShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            GolfDelegates delegates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delegates.LoadMainScreenAsync = LoadMainScreenAsync;
        }
        public FirstViewModel? FirstScreen { get; set; }
        protected override async Task StartNewGameAsync()
        {
            if (MainVM != null)
            {
                await CloseSpecificChildAsync(MainVM);
                MainVM = null;
            }
            if (FirstScreen != null)
            {
                throw new BasicBlankException("First Screen should be null when loading First Screens");
            }
            FirstScreen = MainContainer.Resolve<FirstViewModel>();
            await LoadScreenAsync(FirstScreen);
        }
        private async Task LoadMainScreenAsync()
        {
            if (FirstScreen == null)
            {
                return; //because main screen is already loaded.
            }
            await CloseSpecificChildAsync(FirstScreen);
            FirstScreen = null;
            MainVM = MainContainer.Resolve<GolfCardGameMainViewModel>();
            await LoadScreenAsync(MainVM);
        }
        protected override IMainScreen GetMainViewModel()
        {
            throw new BasicBlankException("Needed to open first screen instead");
        }
    }
}
