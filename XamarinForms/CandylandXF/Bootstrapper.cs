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
using CandylandCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGamingUIXFLibrary.Bootstrappers;
using BasicGameFrameworkLibrary.StandardImplementations.XamarinForms.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using CandylandCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.BasicDrawables.MiscClasses;
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;

namespace CandylandXF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper<CandylandShellViewModel, GamePage>
    {
        public Bootstrapper(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode)
        {
        }

        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<CandylandShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<CandylandPlayerItem, CandylandSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<CandylandPlayerItem, CandylandSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<CandylandPlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterSingleton<IProportionBoard, ProportionXF>("main"); //here too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("main");
            OurContainer.RegisterType<DrawShuffleClass<CandylandCardData, CandylandPlayerItem>>(); //hopefully this does not have to be replaced.
            OurContainer.RegisterType<GenericCardShuffler<CandylandCardData>>(); //this is iffy too.
            OurContainer.RegisterSingleton<IDeckCount, CandylandCount>();
            OurContainer.RegisterType<BasicGameContainer<CandylandPlayerItem, CandylandSaveInfo>>();
            return Task.CompletedTask;
        }
    }
}
