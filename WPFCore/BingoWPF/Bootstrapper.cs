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
using BingoCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using BingoCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
//i think this is the most common things i like to do
namespace BingoWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper <BingoShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.AllowAnyMove = true; //only for testing.
        //    return Task.CompletedTask;
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<BingoShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<BingoPlayerItem, BingoSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<BingoPlayerItem, BingoSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<BingoPlayerItem>>(true); //i think its false for singleton.
            OurContainer.RegisterType<BasicGameContainer<BingoPlayerItem, BingoSaveInfo>>();
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}
