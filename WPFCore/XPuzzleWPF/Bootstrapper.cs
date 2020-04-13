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
using BasicGamingUIWPFLibrary.Bootstrappers;
using XPuzzleCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.AutoresumeClasses;
//i think this is the most common things i like to do
namespace XPuzzleWPF
{
    public class Bootstrapper : SinglePlayerBootstrapper<XPuzzleShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }

        protected override Task RegisterTestsAsync()
        {
            //SinglePlayerProductionSave.RecentOne = 1; //0 based is fine i think.
            return Task.CompletedTask;
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<XPuzzleShellViewModel>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
