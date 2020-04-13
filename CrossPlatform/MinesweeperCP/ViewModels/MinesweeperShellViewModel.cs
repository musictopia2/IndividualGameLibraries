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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
//i think this is the most common things i like to do
namespace MinesweeperCP.ViewModels
{
    public class MinesweeperShellViewModel : SinglePlayerShellViewModel
    {
        //TODO: needs another screen here called OpeningScreen.
        //because at first, it needs the opening screen
        //so you can do the first options.
        public MinesweeperShellViewModel(IGamePackageResolver mainContainer
            , CommandContainer container
            , IGameInfo GameData
            , ISaveSinglePlayerClass saves
            ) : base(mainContainer, container, GameData, saves)
        {
            
        }

        protected override bool AlwaysNewGame => false;
        protected override bool AutoStartNewGame => false;

        public IScreen? OpeningScreen { get; set; }

        protected override async Task OpenStartingScreensAsync()
        {
            OpeningScreen = MainContainer.Resolve<MinesweeperOpeningViewModel>(); //i think has to be this way so its fresh everytime.
            await LoadScreenAsync(OpeningScreen); //try this way.
            await ShowNewGameAsync();
            FinishInit();
        }

        protected override Task NewGameRequestedAsync()
        {
            if (OpeningScreen == null)
            {
                throw new BasicBlankException("There was no opening screen.  Rethink");
            }
            return CloseSpecificChildAsync(OpeningScreen);
        }
        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<MinesweeperMainViewModel>();
            return model;
        }
    }
}
