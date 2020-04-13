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
using FroggiesCP.Logic;
using FroggiesCP.Data;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using FroggiesCP.ViewModels;
//should not need the view models though.  if i am wrong, rethink.
//i think this is the most common things i like to do
namespace FroggiesWPF
{
    public class GamePage : SinglePlayerShellView
    {

        public GamePage(IGameInfo gameData, BasicData basicData, IStartUp start) : base(gameData, basicData, start)
        {
            

        }

        protected override Task PopulateUIAsync()
        {
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(FroggiesShellViewModel.OpeningScreen)
            };
            AddMain(parent); //hopefully this simple (?)
            return Task.CompletedTask;
        }
    }
}
