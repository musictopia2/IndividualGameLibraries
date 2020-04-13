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
using ThreeLetterFunCP.Logic;
using ThreeLetterFunCP.Data;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using ThreeLetterFunCP.ViewModels;
//should not need the view models though.  if i am wrong, rethink.
//i think this is the most common things i like to do
namespace ThreeLetterFunWPF
{
    public class GamePage : MultiplayerBasicShellView
    {
        public GamePage(IGameInfo gameData,
            BasicData basicData, 
            IStartUp start) : base(gameData, basicData, start)
        {
        }
        protected override Task PopulateUIAsync()
        {
            //if any exceptions to the shell, do here or override other things.
            ParentSingleUIContainer firsts = new ParentSingleUIContainer()
            {
                Name = nameof(ThreeLetterFunShellViewModel.FirstScreen)
            };
            AddMain(firsts);
            ParentSingleUIContainer advanced = new ParentSingleUIContainer()
            {
                Name = nameof(ThreeLetterFunShellViewModel.AdvancedScreen)
            };
            AddMain(advanced);
            ParentSingleUIContainer cards = new ParentSingleUIContainer()
            {
                Name = nameof(ThreeLetterFunShellViewModel.CardsScreen)
            };
            AddMain(cards);
            return Task.CompletedTask;
        }
    }
}
