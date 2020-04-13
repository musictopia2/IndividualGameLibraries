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
using CandylandCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using CandylandCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.BasicDrawables.MiscClasses;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
//i think this is the most common things i like to do
namespace CandylandWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper <CandylandShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //has to test new game as well quickly.
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.AlwaysNewGame = true;
        //    //TestData!.SaveOption = BasicGameFrameworkLibrary.TestUtilities.EnumTestSaveCategory.NoSave;
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<CandylandShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<CandylandPlayerItem, CandylandSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CandylandPlayerItem, CandylandSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CandylandPlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterSingleton<IProportionBoard, ProportionWPF>("main"); //here too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("main");
            OurContainer.RegisterType<DrawShuffleClass<CandylandCardData, CandylandPlayerItem>>(); //hopefully this does not have to be replaced.
            OurContainer.RegisterType<GenericCardShuffler<CandylandCardData>>(); //this is iffy too.
            OurContainer.RegisterSingleton<IDeckCount, CandylandCount>();
            OurContainer.RegisterType<BasicGameContainer<CandylandPlayerItem, CandylandSaveInfo>>();

            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}