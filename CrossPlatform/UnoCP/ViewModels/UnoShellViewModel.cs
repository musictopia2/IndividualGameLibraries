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
using UnoCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using UnoCP.Logic;
//i think this is the most common things i like to do
namespace UnoCP.ViewModels
{
    public class UnoShellViewModel : BasicMultiplayerShellViewModel<UnoPlayerItem>
    {
        public UnoShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test,
            UnoColorsDelegates delegates
            )
            : base(mainContainer, container, gameData, basicData, save, test)
        {
            delegates.OpenColorAsync = OpenColorAsync;
            delegates.CloseColorAsync = CloseColorAsync;
        }
        public ChooseColorViewModel? ColorScreen { get; set; }
        private async Task OpenColorAsync()
        {
            if (MainVM != null)
            {
                await CloseSpecificChildAsync(MainVM);
                MainVM = null;
            }
            if (ColorScreen != null)
            {
                await CloseSpecificChildAsync(ColorScreen);
                ColorScreen = null;
            }
            ColorScreen = MainContainer.Resolve<ChooseColorViewModel>();
            await LoadScreenAsync(ColorScreen);
        }
        private async Task CloseColorAsync()
        {
            if (ColorScreen == null && MainVM != null)
            {
                return; //because no need this time.
            }
            if (ColorScreen != null)
            {
                await CloseSpecificChildAsync(ColorScreen);
                ColorScreen = null;
            }
            ClearSubscriptions(); //try this too.
            await StartNewGameAsync(); //misleading because sometimes it can reload even if not new game like in cases like uno.
            
        }
        protected override async Task GetStartingScreenAsync()
        {
            await OpenColorAsync();
        }
        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<UnoMainViewModel>();
            return model;
        }
    }
}
