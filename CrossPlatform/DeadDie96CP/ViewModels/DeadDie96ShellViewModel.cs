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
using DeadDie96CP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
//i think this is the most common things i like to do
namespace DeadDie96CP.ViewModels
{
    public class DeadDie96ShellViewModel : BasicMultiplayerShellViewModel<DeadDie96PlayerItem>
    {
        public DeadDie96ShellViewModel(IGamePackageResolver mainContainer,
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
            var model = MainContainer.Resolve<DeadDie96MainViewModel>();
            return model;
        }
    }
}
