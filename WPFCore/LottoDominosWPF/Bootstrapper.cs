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
using LottoDominosCP.ViewModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses;
using LottoDominosCP.Data;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGameFrameworkLibrary.GameGraphicsCP.Interfaces;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFrameworkLibrary.Dominos;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace LottoDominosWPF
{
    public class Bootstrapper : MultiplayerBasicBootstrapper <LottoDominosShellViewModel, GamePage>
    {
        public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
        {
        }
        //protected override Task RegisterTestsAsync()
        //{
        //    TestData!.AlwaysNewGame = true; //to test that part as well.
        //    return base.RegisterTestsAsync();
        //}
        protected override Task ConfigureAsync()
        {
            OurContainer!.RegisterNonSavedClasses<LottoDominosShellViewModel>();
            OurContainer!.RegisterType<BasicGameLoader<LottoDominosPlayerItem, LottoDominosSaveInfo>>();
            OurContainer.RegisterType<RetrieveSavedPlayers<LottoDominosPlayerItem, LottoDominosSaveInfo>>();
            OurContainer.RegisterType<MultiplayerOpeningViewModel<LottoDominosPlayerItem>>(true); //had to be set to true after all.

            OurContainer.RegisterType<StandardWidthHeight>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            //OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
            OurContainer.RegisterType<BasicGameContainer<LottoDominosPlayerItem, LottoDominosSaveInfo>>();
            //hopefully no need for the shuffler since i already created one.
            //anything that needs to be registered will be here.
            return Task.CompletedTask;
        }
    }
}