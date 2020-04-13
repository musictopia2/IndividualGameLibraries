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
using SnakesAndLaddersCP.Data;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.Dice;
//i think this is the most common things i like to do
namespace SnakesAndLaddersCP.ViewModels
{
    public class SnakesAndLaddersShellViewModel : BasicMultiplayerShellViewModel<SnakesAndLaddersPlayerItem>
    {
        public SnakesAndLaddersShellViewModel(IGamePackageResolver mainContainer,
            CommandContainer container, 
            IGameInfo gameData, 
            BasicData basicData,
            IMultiplayerSaveState save,
            TestOptions test)
            : base(mainContainer, container, gameData, basicData, save, test)
        {
        }
        //for dice, i have to do it this way.  this allows the most flexibility.  so if i have others that needs to be reset, that can be done.
        protected override CustomBasicList<Type> GetAdditionalObjectsToReset()
        {
            Type type = typeof(StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem>);
            CustomBasicList<Type> output = new CustomBasicList<Type>()
            {
                type
            };
            //for dice games, either will be a new shell for dice games or will be in the templates.
            return output;
        }
        
        protected override IMainScreen GetMainViewModel()
        {
            var model = MainContainer.Resolve<SnakesAndLaddersMainViewModel>();
            return model;
        }
    }
}
