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
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using DummyRummyCP.Logic;
using DummyRummyCP.Data;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicGamingUIXFLibrary.Shells;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using Xamarin.Forms;
namespace DummyRummyXF
{
    public class GamePage : MultiplayerBasicShellView
    {
        public GamePage(
            IGamePlatform customPlatform,
            IGameInfo gameData,
            BasicData basicData,
            IStartUp start,
            IStandardScreen screen) : base(customPlatform, gameData, basicData, start, screen)
        {
            //TODO:  may need to think about if we need load or update (?)
        }

        

        protected override Task PopulateUIAsync()
        {
            //if any exceptions to the shell, do here or override other things.
            return Task.CompletedTask;
        }
    }
}
