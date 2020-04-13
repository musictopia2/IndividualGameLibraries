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
using XactikaCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using XactikaCP.Logic;
//i think this is the most common things i like to do
namespace XactikaCP.ViewModels
{
    public class XactikaShellViewModel : BasicTrickShellViewModel<XactikaPlayerItem>
    {
        public XactikaShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            XactikaDelegates delegates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delegates.LoadModeAsync = LoadModeAsync;
            delegates.CloseModeAsync = CloseModeAsync;
        }
        public XactikaModeViewModel? ModeScreen { get; set; }
        protected override async Task GetStartingScreenAsync()
        {
            await LoadModeAsync(); //always open this one  no matter what.
        }

        private async Task LoadModeAsync()
        {
            if (ModeScreen != null)
            {
                return;
            }
            if (MainVM != null)
            {
                await CloseMainAsync("Cannot close game from mode.  Rethink");
            }
            ModeScreen = MainContainer.Resolve<XactikaModeViewModel>();
            await LoadScreenAsync(ModeScreen);
        }
        private async Task CloseModeAsync()
        {
            if (ModeScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ModeScreen);
            ModeScreen = null;
            await StartNewGameAsync();
        }

        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<XactikaMainViewModel>();
            return model;
        }
    }
}
