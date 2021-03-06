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
using BasicGamingUIWPFLibrary.Shells;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using MinesweeperCP.Logic;
using MinesweeperCP.Data;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using MinesweeperCP.ViewModels;
//should not need the view models though.  if i am wrong, rethink.
//i think this is the most common things i like to do
namespace MinesweeperWPF
{
    public class GamePage : SinglePlayerShellView
    {
        //we may have to think about loading information.
        //until i have some experience, not sure what to do (?)
        
        public GamePage(IGameInfo gameData, BasicData basicData, IStartUp start) : base(gameData, basicData, start)
        {

            //TODO:  may need to think about if we need load or update (?)


        }

        protected override Task PopulateUIAsync()
        {
            //if any exceptions to the shell, do here or override other things.
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(MinesweeperShellViewModel.OpeningScreen)
            };
            AddMain(parent); //hopefully this works.  since its one or the other but never both the same time.

            return Task.CompletedTask;
        }
    }
}
